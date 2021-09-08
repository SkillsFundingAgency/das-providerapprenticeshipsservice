namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text
{
    public interface IApprenticeshipValidationErrorText
    {
        ValidationMessage AcademicYearStartDate01 { get; }
        ValidationMessage CohortRef01 { get; }
        ValidationMessage CohortRef02 { get; }
        ValidationMessage DateOfBirth01 { get; }
        ValidationMessage DateOfBirth02 { get; }
        ValidationMessage DateOfBirth06 { get; }
        ValidationMessage DateOfBirth07 { get; }
        ValidationMessage FamilyName01 { get; }
        ValidationMessage FamilyName02 { get; }     
        ValidationMessage GivenNames01 { get; }
        ValidationMessage GivenNames02 { get; }
        ValidationMessage LearnPlanEndDate01 { get; }
        ValidationMessage LearnPlanEndDate02 { get; }
        ValidationMessage LearnPlanEndDate03 { get; }
        ValidationMessage EndDateBeforeOrIsCurrentMonth { get; }
        ValidationMessage LearnStartDate01 { get; }
        ValidationMessage LearnStartDate02 { get; }
        ValidationMessage LearnStartDate05 { get; }
        ValidationMessage LearnStartDate06 { get; }
        ValidationMessage LearnStartDateNotValidForTrainingCourse { get; }     
        ValidationMessage ProviderRef01 { get; }     
        ValidationMessage StdCode01 { get; }
        ValidationMessage StdCode02 { get; }
        ValidationMessage StdCode03 { get; }
        ValidationMessage StdCode04 { get; }
        ValidationMessage CourseCode01 { get; }
        ValidationMessage TrainingPrice01 { get; }
        ValidationMessage TrainingPrice02 { get; }
        ValidationMessage Uln01 { get; }
        ValidationMessage Uln02 { get; }
        ValidationMessage Uln03 { get; }
        ValidationMessage Uln04 { get; }
        ValidationMessage EmailAddressBlank { get; }
        ValidationMessage EmailAddressNotValid { get; }
        ValidationMessage EmailAddressLength { get; }
        ValidationMessage EmailAddressRepeat { get; }
        ValidationMessage AgreementIdBlank { get; }
        ValidationMessage AgreementIdMismatch { get; }        
    }
}