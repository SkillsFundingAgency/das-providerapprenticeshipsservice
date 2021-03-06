﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class DataLockMapper : IDataLockMapper
    {
        private readonly IMediator _mediator;

        public DataLockMapper(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<DataLockViewModel> MapDataLockStatus(DataLockStatus dataLock)
        {
            var training = await GetTrainingProgramme(dataLock.IlrTrainingCourseCode);
            return MapDataLockStatus(dataLock, training);
        }

        public async Task<List<DataLockViewModel>> MapDataLockStatusList(List<DataLockStatus> datalocks)
        {
            var trainingProgrammes = await GetTrainingProgrammes();

            var result = new List<DataLockViewModel>();

            foreach (var dataLock in datalocks)
            {
                var training = trainingProgrammes.Single(x => x.CourseCode == dataLock.IlrTrainingCourseCode);
                result.Add(MapDataLockStatus(dataLock, training));
            }

            return result;
        }

        public IEnumerable<PriceHistoryViewModel> MapPriceDataLock(IEnumerable<PriceHistory> apprenticeshipPriceHistory, IOrderedEnumerable<DataLockViewModel> dataLockWithOnlyPriceMismatch)
        {
            var priceHistorViewModels = apprenticeshipPriceHistory
                .Select(history => new PriceHistoryViewModel
                {
                    ApprenticeshipId = history.ApprenticeshipId,
                    Cost = history.Cost,
                    FromDate = history.FromDate,
                    ToDate = history.ToDate
                });

            var datalocks = dataLockWithOnlyPriceMismatch
                .OrderBy(x => x.IlrEffectiveFromDate);

            return datalocks.Select(
                datalock =>
                    {
                        var s = priceHistorViewModels
                            .OrderByDescending(x => x.FromDate)
                            .First(x => x.FromDate <= datalock.IlrEffectiveFromDate.Value);
                        s.IlrEffectiveFromDate = datalock.IlrEffectiveFromDate;
                        s.IlrEffectiveToDate = datalock.IlrEffectiveToDate;
                        s.IlrTotalCost = datalock.IlrTotalCost;
                        return s;
                    }
               
            );
        }

        public IEnumerable<CourseDataLockViewModel> MapCourseDataLock(ApprenticeshipViewModel dasRecordViewModel, IEnumerable<DataLockViewModel> dataLockWithCourseMismatch, Apprenticeship apprenticeship, IEnumerable<PriceHistory> apprenticeshipPriceHistory)
        {
            if (apprenticeship.HasHadDataLockSuccess)
                return new CourseDataLockViewModel[0];

            var priceHistorViewModels = apprenticeshipPriceHistory
                .Select(history => new PriceHistoryViewModel
                {
                    ApprenticeshipId = history.ApprenticeshipId,
                    Cost = history.Cost,
                    FromDate = history.FromDate,
                    ToDate = history.ToDate
                });

            var result = new List<CourseDataLockViewModel>();

            foreach (var datalock in dataLockWithCourseMismatch)
            {
                var s = priceHistorViewModels
                    .OrderByDescending(x => x.FromDate)
                    .First(x => x.FromDate <= datalock.IlrEffectiveFromDate.Value);

                result.Add(new CourseDataLockViewModel
                {
                    FromDate = s.FromDate,
                    ToDate = s.ToDate,
                    TrainingName = dasRecordViewModel.CourseName,
                    ApprenticeshipStartDate = dasRecordViewModel.StartDate,
                    IlrTrainingName = datalock.IlrTrainingCourseName,
                    IlrEffectiveFromDate = datalock.IlrEffectiveFromDate,
                    IlrEffectiveToDate = datalock.IlrEffectiveToDate
                });
            }

            return result;
        }

        public async Task<DataLockSummaryViewModel> MapDataLockSummary(DataLockSummary source, bool hasHadDataLockSuccess)
        {
            var result = new DataLockSummaryViewModel
            {
                DataLockWithCourseMismatch = new List<DataLockViewModel>(),
                DataLockWithOnlyPriceMismatch = new List<DataLockViewModel>()
            };

            var trainingProgrammes = await GetTrainingProgrammes();

            foreach (var dataLock in source.DataLockWithCourseMismatch)
            {
                var training = trainingProgrammes.SingleOrDefault(x => x.CourseCode == dataLock.IlrTrainingCourseCode);
                if (training == null)
                {
                    throw new InvalidOperationException(
                        $"Datalock {dataLock.DataLockEventId} IlrTrainingCourseCode {dataLock.IlrTrainingCourseCode} not found; possible expiry");
                }
                result.DataLockWithCourseMismatch.Add(MapDataLockStatus(dataLock, training));
            }

            foreach (var dataLock in source.DataLockWithOnlyPriceMismatch)
            {
                var training = trainingProgrammes.SingleOrDefault(x => x.CourseCode == dataLock.IlrTrainingCourseCode);
                if (training == null)
                {
                    throw new InvalidOperationException(
                        $"Datalock {dataLock.DataLockEventId} IlrTrainingCourseCode {dataLock.IlrTrainingCourseCode} not found; possible expiry");
                }
                result.DataLockWithOnlyPriceMismatch.Add(MapDataLockStatus(dataLock, training));
            }

            result.ShowChangesRequested =
                result.DataLockWithCourseMismatch.Any(
                    x => x.TriageStatusViewModel == TriageStatusViewModel.RestartApprenticeship);

            result.ShowChangesPending =
                result.DataLockWithOnlyPriceMismatch.Any(
                    x => x.TriageStatusViewModel == TriageStatusViewModel.ChangeApprenticeship)
                ||
                    result.DataLockWithCourseMismatch.Any(
                    x => x.TriageStatusViewModel == TriageStatusViewModel.ChangeApprenticeship);

            //Can triage a course datalock if there is one that has not been triaged, and if there isn't one that
            //has been triaged but is pending approval by employer (dealt with one at a time)
            result.ShowCourseDataLockTriageLink =
                result.DataLockWithCourseMismatch.Any(x => x.TriageStatusViewModel == TriageStatusViewModel.Unknown)
                && result.DataLockWithCourseMismatch.All(x => x.TriageStatusViewModel != TriageStatusViewModel.RestartApprenticeship);

            // Show ChangeLink if any not triaged PriceOnly AND NO Course+Price DL
            result.ShowPriceDataLockTriageLink =
                result.DataLockWithOnlyPriceMismatch.Any(x => x.TriageStatusViewModel == TriageStatusViewModel.Unknown);

            if (result.DataLockWithCourseMismatch.Any(m => m.DataLockErrorCode.HasFlag(DataLockErrorCode.Dlock07)))
            {
                result.ShowPriceDataLockTriageLink = false;
            }
            result.ShowPriceDataLockTriageLink = result.ShowPriceDataLockTriageLink || result.ShowCourseDataLockTriageLink && !hasHadDataLockSuccess;

            return result;
        }



        private DataLockViewModel MapDataLockStatus(DataLockStatus dataLock, TrainingProgramme training)
        {
            return new DataLockViewModel
            {
                DataLockEventId = dataLock.DataLockEventId,
                DataLockEventDatetime = dataLock.DataLockEventDatetime,
                PriceEpisodeIdentifier = dataLock.PriceEpisodeIdentifier,
                ApprenticeshipId = dataLock.ApprenticeshipId,
                IlrTrainingCourseCode = dataLock.IlrTrainingCourseCode,
                IlrTrainingType = (TrainingType)dataLock.IlrTrainingType,
                IlrTrainingCourseName = training.Name,
                IlrActualStartDate = dataLock.IlrActualStartDate,
                IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate,
                IlrEffectiveToDate = dataLock.IlrEffectiveToDate,
                IlrTotalCost = dataLock.IlrTotalCost,
                TriageStatusViewModel = (TriageStatusViewModel)dataLock.TriageStatus,
                DataLockErrorCode = dataLock.ErrorCode
            };
        }


        private async Task<TrainingProgramme> GetTrainingProgramme(string courseCode)
        {
            return (await GetTrainingProgrammes()).Single(x => x.CourseCode == courseCode);
        }

        private async Task<List<TrainingProgramme>> GetTrainingProgrammes()
        {
            var programmes = await _mediator.Send(new GetTrainingProgrammesQueryRequest
            {
                IncludeFrameworks = true
            });
            return programmes.TrainingProgrammes;
        }
    }
}