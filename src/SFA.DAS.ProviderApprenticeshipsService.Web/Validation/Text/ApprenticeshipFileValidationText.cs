namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text
{
    public static class ApprenticeshipFileValidationText
    {
        public static ValidationMessage FilenameFormat => 
            new ValidationMessage("Filename must be in the correct format, eg. APPDATA-20161212-201530.csv", "Filename_01");

        public static ValidationMessage FilenameFormatDate =>
            new ValidationMessage("The file date/time must be on or before today's date/time", "Filename_02");

        public static string NoRecords => "File contains no records";

        public static string MaxFileSizeMessage(int maxFileSize) => $"File size cannot be larger then {maxFileSize / 1024} kb";
    }
}