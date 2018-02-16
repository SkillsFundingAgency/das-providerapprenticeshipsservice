using System;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text
{
    public class BulkUploadApprenticeshipValidationText : IApprenticeshipValidationErrorText
    {
        private readonly IAcademicYearDateProvider _academicYear;

        public BulkUploadApprenticeshipValidationText(IAcademicYearDateProvider academicYear)
        {
            _academicYear = academicYear;
        }

       public ValidationMessage AcademicYearStartDate01 =>
            new ValidationMessage($"<strong>Start dates</strong> can't be in the previous academic year. The earliest date you can use is { _academicYear.CurrentAcademicYearStartDate.ToGdsFormatShortMonthWithoutDay()}", "AcademicYear_01");

        public ValidationMessage CohortRef01 =>
            new ValidationMessage("The <strong>cohort reference</strong> must be the same for all apprentices in your upload file", "CohortRef_01");
        public ValidationMessage CohortRef02 =>
            new ValidationMessage("The <strong>cohort reference</strong> does not match your current cohort", "CohortRef_02");

        public ValidationMessage Uln01 =>
            new ValidationMessage("You must enter a 10-digit <strong>unique learner number</strong>", "ULN_01");
        public ValidationMessage Uln02 =>
            new ValidationMessage("The <strong>unique learner number</strong> of 9999999999 isn't valid", "ULN_02");
        public ValidationMessage Uln03 =>
                new ValidationMessage("You must enter a valid <strong>unique learner number</strong>", "ULN_03");
        public ValidationMessage Uln04 =>
            new ValidationMessage("The <strong>unique learner number</strong> must be unique within the cohort", "ULN_04");
        public ValidationMessage FamilyName01 =>
            new ValidationMessage("<strong>Last name</strong> must be entered", "FamilyName_01");
        public ValidationMessage FamilyName02 =>
            new ValidationMessage("You must enter a <strong>last name</strong> that's no longer than 100 characters", "FamilyName_02");
        public ValidationMessage GivenNames01 =>
            new ValidationMessage("<strong>First name</strong> must be entered", "GivenNames_01");
        public ValidationMessage GivenNames02 =>
            new ValidationMessage("You must enter a <strong>first name</strong> that's no longer than 100 characters", "GivenNames_02");

        public ValidationMessage DateOfBirth01 =>
            new ValidationMessage("You must enter the apprentice's <strong>date of birth</strong> using the format yyyy-mm-dd, for example 2001-04-23", "DateOfBirth_01");
        public ValidationMessage DateOfBirth02 =>
            new ValidationMessage("The apprentice's <strong>date of birth</strong> must show that they're at least 15 years old at the start of their training", "DateOfBirth_02");
        public ValidationMessage DateOfBirth06 =>
                    new ValidationMessage("Enter a valid year - the apprentice must be younger than 115 at the start of the current teaching year", "DateOfBirth_06");

        public ValidationMessage LearnStartDate01 =>
            new ValidationMessage("You must enter the <strong>start date</strong>, for example 2017-09", "LearnStartDate_01");
        public ValidationMessage LearnStartDate02 =>
            new ValidationMessage("The <strong>start date</strong> must not be earlier than May 2017", "LearnStartDate_02");
        public ValidationMessage LearnStartDate05 =>
            new ValidationMessage("The start date must be no later than one year after the end of the current teaching year", "LearnStartDate_05");
        public ValidationMessage LearnStartDate06 =>
            new ValidationMessage("The start date can't be earlier than May 2018", "LearnStartDate_06");

        public ValidationMessage LearnPlanEndDate01 =>
            new ValidationMessage("You must enter the <strong>end date</strong>, for example 2019-02", "LearnPlanEndDate_01");
        public ValidationMessage LearnPlanEndDate02 =>
            new ValidationMessage("You must not enter an <strong>end date</strong> that's earlier than the start date", "LearnPlanEndDate_02");
        public ValidationMessage LearnPlanEndDate03 =>
            new ValidationMessage("You must not enter an <strong>end date</strong> that's earlier than today's date", "LearnPlanEndDate_03");

        public ValidationMessage TrainingPrice01 =>
            new ValidationMessage("You must enter the <strong>total cost of training</strong> in whole pounds - don't include any symbols, characters or letters", "TrainingPrice_01");
        public ValidationMessage TrainingPrice02 =>
            new ValidationMessage("The <strong>total cost</strong> must be £100,000 or less", "TrainingPrice_02");

        public ValidationMessage ProviderRef01 =>
            new ValidationMessage("The <strong>Provider reference</strong> must be 20 characters or fewer", "ProviderRef_01");

        // training type validation messages

        public ValidationMessage ProgType01 =>
              new ValidationMessage("You must enter a <strong>Programme type</strong> - you can add up to 2 characters", "ProgType_01");
        public ValidationMessage ProgType02 =>
              new ValidationMessage("The <strong>Programme type</strong> you've added isn't valid", "ProgType_02");

        public ValidationMessage FworkCode01 =>
              new ValidationMessage("The <strong>Framework code</strong> must be 3 characters or fewer", "FworkCode_01");
        public ValidationMessage FworkCode02 =>
              new ValidationMessage("You must enter a <strong>Framework code</strong> - you can add up to 3 characters", "FworkCode_02");
        public ValidationMessage FworkCode03 =>
              new ValidationMessage("You must not enter a <strong>Framework code</strong> when you've entered a Standard programme type", "FworkCode_03");

        public ValidationMessage PwayCode01 =>
              new ValidationMessage("The <strong>Pathway code</strong> must be 3 characters or fewer", "PwayCode_01");
        public ValidationMessage PwayCode02 =>
              new ValidationMessage("You must enter a <strong>Pathway code</strong> = you can add up to 3 characters", "PwayCode_02");
        public ValidationMessage PwayCode03 =>
              new ValidationMessage("You must not enter a <strong>Pathway code</strong> when you've entered a Standard programme type", "PwayCode_03");

        public ValidationMessage StdCode01 =>
              new ValidationMessage("The <strong>Standard code</strong> must be 5 characters or fewer", "StdCode_01");
        public ValidationMessage StdCode02 =>
              new ValidationMessage("You must enter a <strong>Standard code</strong> - you can add up to 5 characters", "StdCode_02");
        public ValidationMessage StdCode03 =>
              new ValidationMessage("You must not enter a <strong>Standard code</strong> when you've entered a Framework programme type", "StdCode_03");

        public ValidationMessage TrainingCode01 =>
            new ValidationMessage("You must enter a valid <strong>Standard code</strong> or <strong>Framework code</strong>", "DefaultErrorCode");

        public ValidationMessage EPAOrgID01 { get { throw new NotImplementedException(); } }
        public ValidationMessage EPAOrgID02 { get { throw new NotImplementedException(); } }

    }
}