using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text
{
    public class BulkUploadApprenticeshipValidationText : IApprenticeshipValidationErrorText
    {
        public ValidationMessage CohortRef01 =>
            new ValidationMessage("You must enter a <strong>cohort reference</strong> number", "CohortRef_01");
        public ValidationMessage CohortRef02 =>
            new ValidationMessage("You must enter a <strong>cohort reference</strong> number of up to 20 characters", "CohortRef_02");
        public ValidationMessage CohortRef03 =>
            new ValidationMessage("The cohort reference must be the same for all apprentices in your upload file", "CohortRef_03");
        public ValidationMessage CohortRef04 =>
            new ValidationMessage("The cohort reference does not match your current cohort", "CohortRef_04");

        public ValidationMessage Uln01 =>
            new ValidationMessage("You must enter a <strong>cohort reference</strong> number of up to 10 characters", "ULN_01");
        public ValidationMessage Uln02 =>
            new ValidationMessage("The <strong>Unique Learner number</strong> of 9999999999 is not valid", "ULN_02");

        public ValidationMessage FamilyName01 =>
            new ValidationMessage("You must enter the apprentice's <strong>last name</strong> - you can add up to 100 characters", "FamilyName_01");
        public ValidationMessage FamilyName02 =>
            new ValidationMessage("You must enter the apprentice's <strong>last name</strong> - you can add up to 100 characters", "FamilyName_02");
        public ValidationMessage GivenNames01 =>
            new ValidationMessage("You must enter the apprentice's <strong>first name</strong> - you can add up to 100 characters", "GivenNames_01");
        public ValidationMessage GivenNames02 =>
            new ValidationMessage("You must enter the apprentice's <strong>first name</strong> - you can add up to 100 characters", "GivenNames_02");

        public ValidationMessage DateOfBirth01 =>
            new ValidationMessage("The <strong>Date of birth</strong> must be entered", "DateOfBirth_01");

        public ValidationMessage DateOfBirth02 =>
            new ValidationMessage("The <strong>Date of birth</strong> must be entered and be in the format yyyy-mm-dd", "DateOfBirth_02");

        public ValidationMessage DateOfBirth03 =>
            new ValidationMessage("The apprentice must be at least 15 years old at the start of the programme", "DateOfBirth_03");

        public ValidationMessage NINumber01 =>
            new ValidationMessage("<strong>National insurance number</strong> cannot be empty", "NINumber_01");

        public ValidationMessage NINumber02 =>
            new ValidationMessage("The <strong>National Insurance number</strong> must be entered and must not be more than 9 characters in length", "NINumber_02");

        public ValidationMessage NINumber03 =>
            new ValidationMessage("Enter a valid <strong>National insurance number</strong>", "NINumber_03");

        public ValidationMessage LearnStartDate01 =>
            new ValidationMessage("The <strong>Learning start date</strong> must be entered", "LearnStartDate_01");
        public ValidationMessage LearnStartDate02 =>
            new ValidationMessage("The <strong>Learning start date</strong> must be entered and be in the format yyyy-mm-dd", "LearnStartDate_02");
        public ValidationMessage LearnStartDate03 =>
            new ValidationMessage("The <strong>start date</strong> must not be earlier than 1 May 2017", "LearnStartDate_03");

        public ValidationMessage LearnPlanEndDate01 =>
            new ValidationMessage("The <strong>Learning planned end date</strong> must be entered", "LearnPlanEndDate_01");
        public ValidationMessage LearnPlanEndDate02 =>
            new ValidationMessage("The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd", "LearnPlanEndDate_02");
        public ValidationMessage LearnPlanEndDate03 =>
            new ValidationMessage("The <strong>Learning planned end date</strong> must not be on or before the Learning start date", "LearnPlanEndDate_03");
        public ValidationMessage LearnPlanEndDate06 =>
            new ValidationMessage("The <strong>Learning planned end date</strong> must not be in the past", "LearnPlanEndDate_06");

        public ValidationMessage TrainingPrice01 =>
            new ValidationMessage("The <strong>Training price</strong> must be entered", "TrainingPrice_01");
        public ValidationMessage TrainingPrice02 =>
            new ValidationMessage("The <strong>Training price</strong> must be entered and must not be more than 6 characters in length", "TrainingPrice_02");
        public ValidationMessage TrainingPrice03 =>
            new ValidationMessage("The <strong>Training price</strong> must not be more than £100,000", "TrainingPrice_03");

        public ValidationMessage ProviderRef01 =>
            new ValidationMessage("The <strong>Reference</strong> must be 20 characters or fewer", "ProviderRef_01");

        // training type validation messages

        public ValidationMessage ProgType01 =>
              new ValidationMessage("The <strong>Programme type</strong> must be entered and must not be more than 2 characters in length", "ProgType_01");

        public ValidationMessage ProgType02 =>
              new ValidationMessage("The <strong>Programme type</strong> is not a valid value", "ProgType_02");

        public ValidationMessage FworkCode01 =>
              new ValidationMessage("The <strong>Framework code</strong> must not be more than 3 characters in length", "FworkCode_01");

        public ValidationMessage FworkCode02MustFworkCode =>
              new ValidationMessage("The <strong>Framework code</strong> must be entered where the Programme Type is a framework", "FworkCode_02");

        public ValidationMessage FworkCode03MustNotFworkCode =>
              new ValidationMessage("The <strong>Framework code</strong> must not be entered where the Programme Type is a standard", "FworkCode_03");

        public ValidationMessage FworkCode04 =>
              new ValidationMessage("The <strong>Framework code</strong> is not a valid value", "FworkCode_04");

        public ValidationMessage PwayCode01 =>
              new ValidationMessage("The <strong>Pathway code</strong> must not be more than 3 characters in length", "PwayCode_01");

        public ValidationMessage PwayCode02 =>
              new ValidationMessage("The <strong>Pathway code</strong> must be entered where the Programme Type is a framework", "PwayCode_02");

        public ValidationMessage PwayCode03 =>
              new ValidationMessage("The <strong>Pathway code</strong> must not be entered where the Programme Type is a standard", "PwayCode_03");

        public ValidationMessage PwayCode04 =>
              new ValidationMessage("The <strong>Pathway code</strong> is not a valid value", "PwayCode_04");

        public ValidationMessage StdCode01 =>
              new ValidationMessage("The <strong>Standard code</strong> must not be more than 5 characters in length", "StdCode_01");

        public ValidationMessage StdCode02 =>
              new ValidationMessage("The <strong>Standard code</strong> must be entered where the Programme Type is a standard", "StdCode_02");

        public ValidationMessage StdCode03 =>
              new ValidationMessage("The <strong>Standard code</strong> must not be entered where the Programme Type is a framework", "StdCode_03");

        public ValidationMessage StdCode04 =>
              new ValidationMessage("The <strong>Standard code</strong> is not a valid value", "StdCode_04");

        public ValidationMessage TrainingCode01 =>
            new ValidationMessage("<strong>Training code</strong> cannot be empty", "DefaultErrorCode");

        public ValidationMessage EPAOrgID01 { get { throw new NotImplementedException(); } }

        public ValidationMessage EPAOrgID02 { get { throw new NotImplementedException(); } }
    }
}