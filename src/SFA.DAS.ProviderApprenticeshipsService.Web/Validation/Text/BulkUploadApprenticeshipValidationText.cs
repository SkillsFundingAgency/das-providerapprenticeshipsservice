using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text
{
    public class BulkUploadApprenticeshipValidationText : IApprenticeshipValidationErrorText
    {
        public ValidationMessage CohortRef01 =>
            new ValidationMessage("The cohort reference must be the same for all apprentices in your upload file", "CohortRef_01");
        public ValidationMessage CohortRef02 =>
            new ValidationMessage("The cohort reference does not match your current cohort", "CohortRef_02");

        public ValidationMessage Uln01 =>
            new ValidationMessage("You must enter a <strong>Unique Learner number</strong> of up to 10 characters", "ULN_01");
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
            new ValidationMessage("You must enter the apprentice's <strong>date of birth</strong>, for example 2001-04-23", "DateOfBirth_01");
        public ValidationMessage DateOfBirth02 =>
            new ValidationMessage("The apprentice must be at least 15 years old at the start of the programme", "DateOfBirth_02");

        public ValidationMessage LearnStartDate01 =>
            new ValidationMessage("You must enter the <strong>planned training start date</strong>, for example 2017-09", "LearnStartDate_01");
        public ValidationMessage LearnStartDate02 =>
            new ValidationMessage("The <strong>start date</strong> must not be earlier than 1 May 2017", "LearnStartDate_02");

        public ValidationMessage LearnPlanEndDate01 =>
            new ValidationMessage("You must enter the <strong>planned training finish date</strong>, for example 2019-02", "LearnPlanEndDate_01");
        public ValidationMessage LearnPlanEndDate02 =>
            new ValidationMessage("You must not enter a <strong>planned training finish date</strong> that's earlier than the <strong>planned training start date</strong>", "LearnPlanEndDate_02");
        public ValidationMessage LearnPlanEndDate03 =>
            new ValidationMessage("You must not enter a <strong>planned training finish date</strong> that's earlier than today's date", "LearnPlanEndDate_03");

        public ValidationMessage TrainingPrice01 =>
            new ValidationMessage("You must enter the <strong>total cost</strong> of training", "TrainingPrice_01");
        public ValidationMessage TrainingPrice02 =>
            new ValidationMessage("The <strong>Training price</strong> must not be more than £100,000", "TrainingPrice_02");

        public ValidationMessage ProviderRef01 =>
            new ValidationMessage("You must enter a valid <strong>Provider reference code</strong> - you can add up to 20 characters", "ProviderRef_01");

        // training type validation messages

        public ValidationMessage ProgType01 =>
              new ValidationMessage("You must enter a <strong>Programme type</strong> - you can add up to 2 characters", "ProgType_01");
        public ValidationMessage ProgType02 =>
              new ValidationMessage("The <strong>Programme type</strong> you've added isn't valid", "ProgType_02");

        public ValidationMessage FworkCode01 =>
              new ValidationMessage("You must enter a <strong>Framework code</strong> - you can add up to 3 characters", "FworkCode_01");
        public ValidationMessage FworkCode02 =>
              new ValidationMessage("You must enter a <strong>Framework code</strong> - you can add up to 3 characters", "FworkCode_02");
        public ValidationMessage FworkCode03 =>
              new ValidationMessage("You must enter a <strong>Framework code</strong> - you can add up to 3 characters", "FworkCode_03");

        public ValidationMessage PwayCode01 =>
              new ValidationMessage("You must enter a <strong>Pathway code</strong> - you can add up to 3 characters", "PwayCode_01");
        public ValidationMessage PwayCode02 =>
              new ValidationMessage("You must enter a <strong>Pathway code</strong> - you can add up to 3 characters", "PwayCode_02");
        public ValidationMessage PwayCode03 =>
              new ValidationMessage("You must enter a <strong>Pathway code</strong> - you can add up to 3 characters", "PwayCode_03");

        public ValidationMessage StdCode01 =>
              new ValidationMessage("You must enter a <strong>Standard code</strong> - you can add up to 5 characters", "StdCode_01");

        public ValidationMessage StdCode02 =>
              new ValidationMessage("You must enter a <strong>Standard code</strong> - you can add up to 5 characters", "StdCode_02");
        public ValidationMessage StdCode03 =>
              new ValidationMessage("You've added a <strong>Standard code</strong>, but chosen a <strong>Programme Type</strong> of 'Framework'", "StdCode_03");

        public ValidationMessage TrainingCode01 =>
            new ValidationMessage("<strong>Training code</strong> cannot be empty", "DefaultErrorCode");

        public ValidationMessage EPAOrgID01 { get { throw new NotImplementedException(); } }
        public ValidationMessage EPAOrgID02 { get { throw new NotImplementedException(); } }
    }
}