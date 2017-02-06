namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text
{
    public class ApprenticeshipValidationText
    {
        private readonly bool _withHtml;

        public ApprenticeshipValidationText(bool withHtml = false)
        {
            _withHtml = withHtml;
        }

        public ValidationMessage CohortRef01 =>
            new ValidationMessage("The <strong>Cohort reference</strong> must be entered", "CohortRef_01", _withHtml);
        public ValidationMessage CohortRef02 =>
            new ValidationMessage("The <strong>Cohort Reference</strong> must be entered and must not be more than 20 characters in length", "CohortRef_02", _withHtml);
        public ValidationMessage CohortRef03 =>
            new ValidationMessage("The <strong>Cohort Reference</strong> must be the same for all learners in the file", "CohortRef_03", _withHtml);
        public ValidationMessage CohortRef04 =>
            new ValidationMessage("The <strong>Cohort reference</strong> must match one of the current cohorts", "CohortRef_04", _withHtml);

        public ValidationMessage Uln01 =>
            new ValidationMessage("You must enter a number that's 10 digits", "ULN_01", _withHtml);
        public ValidationMessage Uln02 =>
            new ValidationMessage("The <strong>Unique Learner number</strong> of 9999999999 is not valid", "ULN_02", _withHtml);
        public ValidationMessage Uln03PassChecksum =>
            new ValidationMessage("The <strong>Unique Learner number</strong> is not in the correct format", "ULN_03", _withHtml);
        public ValidationMessage Uln04AlreadyInUse =>
            new ValidationMessage("The <strong>Unique Learner number</strong> is already in use on another record for this Learning Start Date", "ULN_04", _withHtml);

        public ValidationMessage FamilyName01 =>
            new ValidationMessage("The <strong>Family name</strong> must be entered", "FamilyName_01", _withHtml);
        public ValidationMessage FamilyName02 =>
            new ValidationMessage("You must enter a <strong>Family name</strong> that's no longer than 100 characters", "FamilyName_02", _withHtml);
        public ValidationMessage GivenNames01 =>
            new ValidationMessage("The <strong>Given names</strong> must be entered", "GivenNames_01", _withHtml);
        public ValidationMessage GivenNames02 =>
            new ValidationMessage("You must enter <strong>Given names</strong> that are no longer than 100 characters", "GivenNames_02", _withHtml);

        public ValidationMessage DateOfBirth01 =>
            new ValidationMessage("The <strong>Date of birth</strong> must be entered", "DateOfBirth_01", _withHtml);

        public ValidationMessage DateOfBirth02 =>
            new ValidationMessage("The <strong>Date of birth</strong> must be entered and be in the format yyyy-mm-dd", "DateOfBirth_02", _withHtml);

        public ValidationMessage NINumber01 =>
            new ValidationMessage("<strong>National insurance number</strong> cannot be empty", "NINumber_01", _withHtml);

        public ValidationMessage NINumber02 =>
            new ValidationMessage("The <strong>National Insurance number</strong> must be entered and must not be more than 9 characters in length", "NINumber_02", _withHtml);

        public ValidationMessage NINumber03 =>
            new ValidationMessage("Enter a valid <strong>National insurance number</strong>", "NINumber_03", _withHtml);

        public ValidationMessage LearnStartDate01 =>
            new ValidationMessage("The <strong>Learning start date</strong> must be entered", "LearnStartDate_02", _withHtml);
        public ValidationMessage LearnStartDate02 =>
            new ValidationMessage("The <strong>Learning start date</strong> must be entered and be in the format yyyy-mm-dd", "LearnStartDate_02", _withHtml);

        public ValidationMessage LearnPlanEndDate01 =>
            new ValidationMessage("The <strong>Learning planned end date</strong> must be entered", "LearnPlanEndDate_01", _withHtml);
        public ValidationMessage LearnPlanEndDate02 =>
            new ValidationMessage("The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd", "LearnPlanEndDate_02", _withHtml);
        public ValidationMessage LearnPlanEndDate03 =>
            new ValidationMessage("The <strong>Learning planned end date</strong> must not be on or before the Learning start date", "LearnPlanEndDate_03", _withHtml);
        public ValidationMessage LearnPlanEndDate06 =>
            new ValidationMessage("The <strong>Learning planned end date</strong> must not be in the past", "LearnPlanEndDate_06", _withHtml);

        public ValidationMessage TrainingPrice01 =>
            new ValidationMessage("The <strong>Training price</strong> must be entered", "TrainingPrice_01", _withHtml);
        public ValidationMessage TrainingPrice02 =>
            new ValidationMessage("The <strong>Training price</strong> must be entered and must not be more than 6 characters in length", "TrainingPrice_02", _withHtml);

        public ValidationMessage EPAOrgID01 =>
            new ValidationMessage("The <strong>End point assessment</strong> id must not be more than 7 characters in length", "EPAOrgID_01", _withHtml);
        public ValidationMessage EPAOrgID02 =>
            new ValidationMessage("The <strong>End point assessment</strong> id must not be more than 7 characters in length", "EPAOrgID_02", _withHtml);

        public ValidationMessage ProviderRef01 =>
            new ValidationMessage("The <strong>Reference</strong> must be 20 characters or fewer", "ProviderRef_01", _withHtml);

        // trainging type validation messages

        public ValidationMessage ProgType01 =>
              new ValidationMessage("The <strong>Programme type</strong> must be entered and must not be more than 2 characters in length", "ProgType_01", _withHtml);

        public ValidationMessage ProgType02 =>
              new ValidationMessage("The <strong>Programme type</strong> is not a valid value", "ProgType_02", _withHtml);

        public ValidationMessage FworkCode01 =>
              new ValidationMessage("The <strong>Framework code</strong> must not be more than 3 characters in length", "FworkCode_01", _withHtml);

        public ValidationMessage FworkCode02MustFworkCode =>
              new ValidationMessage("The <strong>Framework code</strong> must be entered where the Programme Type is a framework", "FworkCode_02", _withHtml);

        public ValidationMessage FworkCode03MustNotFworkCode =>
              new ValidationMessage("The <strong>Framework code</strong> must not be entered where the Programme Type is a standard", "FworkCode_03", _withHtml);

        public ValidationMessage FworkCode04 =>
              new ValidationMessage("The <strong>Framework code</strong> is not a valid value", "FworkCode_04", _withHtml);

        public ValidationMessage PwayCode01 =>
              new ValidationMessage("The <strong>Pathway code</strong> must not be more than 3 characters in length", "PwayCode_01", _withHtml);

        public ValidationMessage PwayCode02 =>
              new ValidationMessage("The <strong>Pathway code</strong> must be entered where the Programme Type is a framework", "PwayCode_02", _withHtml);

        public ValidationMessage PwayCode03 =>
              new ValidationMessage("The <strong>Pathway code</strong> must not be entered where the Programme Type is a standard", "PwayCode_03", _withHtml);

        public ValidationMessage PwayCode04 =>
              new ValidationMessage("The <strong>Pathway code</strong> is not a valid value", "PwayCode_04", _withHtml);

        public ValidationMessage StdCode01 =>
              new ValidationMessage("The <strong>Standard code</strong> must not be more than 5 characters in length", "StdCode_01", _withHtml);

        public ValidationMessage StdCode02 =>
              new ValidationMessage("The <strong>Standard code</strong> must be entered where the Programme Type is a standard", "StdCode_02", _withHtml);

        public ValidationMessage StdCode03 =>
              new ValidationMessage("The <strong>Standard code</strong> must not be entered where the Programme Type is a framework", "StdCode_03", _withHtml);

        public ValidationMessage StdCode04 =>
              new ValidationMessage("The <strong>Standard code</strong> is not a valid value", "StdCode_04", _withHtml);

        public ValidationMessage TrainingCode01 =>
            new ValidationMessage("<strong>Training code</strong> cannot be empty", "DefaultErrorCode", _withHtml);

        // 
        /*
         * 


         */

    }
}