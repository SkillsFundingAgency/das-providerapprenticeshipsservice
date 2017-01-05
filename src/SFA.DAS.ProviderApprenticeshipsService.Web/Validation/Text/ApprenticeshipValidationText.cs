namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text
{
    public class ApprenticeshipValidationText
    {
        public ValidationMessage CohortRef01 =>
            new ValidationMessage("The Cohort reference must be entered", "CohortRef_01");
        public ValidationMessage CohortRef02 =>
            new ValidationMessage("The Cohort Reference must be entered and must not be more than 20 characters in length", "CohortRef_02");
        public ValidationMessage CohortRef03 =>
            new ValidationMessage("The Cohort Reference must be the same for all learners in the file", "CohortRef_03");
        public ValidationMessage CohortRef04 =>
            new ValidationMessage("The Cohort reference must match one of the current cohorts", "CohortRef_04");

        public ValidationMessage Uln01 =>
            new ValidationMessage("The Unique Learner number must be entered and must not be more than 10 characters in length", "ULN_01");
        public ValidationMessage Uln02 =>
            new ValidationMessage("The Unique Learner number of 9999999999 is not valid", "ULN_02");
        public ValidationMessage Uln03PassChecksum =>
            new ValidationMessage("The Unique Learner number is not in the correct format", "ULN_03");
        public ValidationMessage Uln04AlreadyInUse =>
            new ValidationMessage("The Unique Learner number is already in use on another record for this Learning Start Date", "ULN_04");

        public ValidationMessage FamilyName01 =>
            new ValidationMessage("The Family name must be entered", "FamilyName_01");
        public ValidationMessage FamilyName02 =>
            new ValidationMessage("The Family name must be entered and must not be more than 100 characters in length", "FamilyName_02");
        public ValidationMessage GivenNames01 =>
            new ValidationMessage("The Given names must be entered", "GivenNames_01");
        public ValidationMessage GivenNames02 =>
            new ValidationMessage("The Given names must be entered and must not be more than 100 characters in length", "GivenNames_02");

        public ValidationMessage DateOfBirth01 =>
            new ValidationMessage("The Date of birth must be entered", "DateOfBirth_01");

        public ValidationMessage DateOfBirth02 =>
            new ValidationMessage("The Date of birth must be entered and be in the format yyyy-mm-dd", "DateOfBirth_02");

        public ValidationMessage NINumber01 =>
            new ValidationMessage("National insurance number cannot be empty", "NINumber_01");
        public ValidationMessage NINumber02 =>
            new ValidationMessage("National insurance number needs to be 10 characters long", "NINumber_02");
        public ValidationMessage NINumber03 =>
            new ValidationMessage("Enter a valid national insurance number", "NINumber_03");

        public ValidationMessage LearnStartDate01 =>
            new ValidationMessage("The Learning start date must be entered", "LearnStartDate_02");
        public ValidationMessage LearnStartDate02 =>
            new ValidationMessage("The Learning start date must be entered and be in the format yyyy-mm-dd", "LearnStartDate_02");

        public ValidationMessage LearnPlanEndDate01 =>
            new ValidationMessage("The Learning planned end date must be entered", "LearnPlanEndDate_01");
        public ValidationMessage LearnPlanEndDate02 =>
            new ValidationMessage("The Learning planned end date must be entered and be in the format yyyy-mm-dd", "LearnPlanEndDate_02");
        public ValidationMessage LearnPlanEndDate03 =>
            new ValidationMessage("The Learning planned end date must not be on or before the Learning start date", "LearnPlanEndDate_03");
        public ValidationMessage LearnPlanEndDate06 =>
            new ValidationMessage("The Learning planned end date must not be in the past", "LearnPlanEndDate_06");

        public ValidationMessage TrainingPrice01 =>
            new ValidationMessage("The Training price must be entered", "TrainingPrice_01");
        public ValidationMessage TrainingPrice02 =>
            new ValidationMessage("The Training price must be entered and must not be more than 6 characters in length", "TrainingPrice_02");

        public ValidationMessage EPAOrgID01 =>
            new ValidationMessage("The End point assessment id must not be more than 7 characters in length", "EPAOrgID_01");
        public ValidationMessage EPAOrgID02 =>
            new ValidationMessage("The End point assessment id must not be more than 7 characters in length", "EPAOrgID_02");

        public ValidationMessage ProviderRef01 =>
            new ValidationMessage("The Provider reference must not be more than 20 characters in length", "ProviderRef_01");

        // trainging type valifation messages

        public ValidationMessage ProgType01 =>
              new ValidationMessage("The Programme type must be entered and must not be more than 2 characters in length", "ProgType_01");

        public ValidationMessage ProgType02 =>
              new ValidationMessage("The Programme type is not a valid value", "ProgType_02");

        public ValidationMessage FworkCode01 =>
              new ValidationMessage("The Framework code must not be more than 3 characters in length", "FworkCode_01");

        public ValidationMessage FworkCode02MustFworkCode =>
              new ValidationMessage("The Framework code must be entered where the Programme Type is a framework", "FworkCode_02");

        public ValidationMessage FworkCode03MustNotFworkCode =>
              new ValidationMessage("The Framework code must not be entered where the Programme Type is a standard", "FworkCode_03");

        public ValidationMessage FworkCode04 =>
              new ValidationMessage("The Framework code is not a valid value", "FworkCode_04");

        public ValidationMessage PwayCode01 =>
              new ValidationMessage("The Pathway code must not be more than 3 characters in length", "PwayCode_01");

        public ValidationMessage PwayCode02 =>
              new ValidationMessage("The Pathway code must be entered where the Programme Type is a framework", "PwayCode_02");

        public ValidationMessage PwayCode03 =>
              new ValidationMessage("The Pathway code must not be entered where the Programme Type is a standard", "PwayCode_03");

        public ValidationMessage PwayCode04 =>
              new ValidationMessage("The Pathway code is not a valid value", "PwayCode_04");

        public ValidationMessage StdCode01 =>
              new ValidationMessage("The Standard code must not be more than 5 characters in length", "StdCode_01");

        public ValidationMessage StdCode02 =>
              new ValidationMessage("The Standard code must be entered where the Programme Type is a standard", "StdCode_02");

        public ValidationMessage StdCode03 =>
              new ValidationMessage("The Standard code must not be entered where the Programme Type is a framework", "StdCode_03");

        public ValidationMessage StdCode04 =>
              new ValidationMessage("The Standard code is not a valid value", "StdCode_04");

        // 
        /*
         * 


         */

    }
}