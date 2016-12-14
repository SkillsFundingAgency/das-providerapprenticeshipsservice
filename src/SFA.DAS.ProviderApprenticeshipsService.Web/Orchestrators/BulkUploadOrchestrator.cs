using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Razor.Text;

using CsvHelper;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class BulkUploadOrchestrator
    {
        public IEnumerable<string> UploadFile(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            // Validate file
            var validationErrors = ValidateFile(uploadApprenticeshipsViewModel.Attachment).ToList();
            if (validationErrors.Any())
            {
                // ToDo: Log errors?
                return validationErrors;
            }

            // Validate data
            var data = CreateFile(uploadApprenticeshipsViewModel.Attachment);
            var validateErrors = ValidateDate(data).ToList();
            if (validateErrors.Any())
            {
                // ToDo: Log errors?
                return validateErrors;
            }

            // Send date to commitment
            //_repository.uploadData(data);

            // Create new view Model with errors if any
            return new List<string>(); // return view model
        }

        private IEnumerable<string> ValidateDate(string data)
        {
            return new List<string>();
        }


        private string CreateFile(HttpPostedFileBase attachment)
        {
            string result = new StreamReader(attachment.InputStream).ReadToEnd();
            // ToDo: test with null and "" data
            using (TextReader tr = new StringReader(result))
            {
                var csvReader = new CsvReader(tr);
                var records = csvReader.GetRecords<CsvRecords>();
                // Validate record -> Like input format
                // Validate that training exsists? Do we have that info somewhere already. Cached?
                // Map to view Model
                // Validate view model 
                // Validate view model for approval
            }
            return result;
        }

        private IEnumerable<string> ValidateFile(HttpPostedFileBase attachment)
        {
            var errors = new List<string>();
            var maxFileSize = 512 * 1000; // ToDo: Move to config
            var fileEnding = ".csv";
            var fileStart = "APPDATA";

            var regex = new Regex(@"\d{8}-\d{6}");
            var dateMatch = regex.Match(attachment.FileName);
            DateTime outDateTime;
            var dateParseSuccess = DateTime.TryParseExact(dateMatch.Value, "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime);
            if (!dateMatch.Success)
                errors.Add($"File name must include the date with fomat: yyyyMMdd-HHmmss");
            else if(!dateParseSuccess)
                errors.Add($"Date in file name is not valid");
            
            if (!attachment.FileName.EndsWith(fileEnding))
                errors.Add($"File name must end with {fileEnding}");
            if (!attachment.FileName.StartsWith(fileStart))
                errors.Add($"File name must start with {fileStart}");
            if (attachment.ContentLength > maxFileSize)
                errors.Add($"File size cannot be larger then {maxFileSize}");

            return errors;
        }
    }

    internal class CsvRecords
    {
        public string GivenName{ get; set; }

        public string FamilyName { get; set; }

        // Validation of DateTime Format?
        public DateTime DateOfBirth { get; set; }

        public int? FrameworkCode { get; set; }

        public int? PathwaCode { get; set; }

        public int? ProgType { get; set; }

        public int? StandardCode { get; set; }

        // Validation of DateTime Format?
        public DateTime LearnerStartDate { get; set; }

        // Validation of DateTime Format?
        public DateTime LearnerEndDate { get; set; }

        public int TrainingPrice { get; set; }

        // What is this?
        public string EPAPrice  { get; set; }

        public string EpaOrgId { get; set; }

        public string EmployerRef  { get; set; }

        public string ProviderRef { get; set; }

        public int Ulr { get; set; }
    }
}