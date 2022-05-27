using System;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.Account.Api.Client.ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new PasAccountApiConfiguration 
            { 
                ApiBaseUrl = "https://at-provideraccounts.apprenticeships.education.gov.uk/", 
                IdentifierUri = "https://citizenazuresfabisgov.onmicrosoft.com/das-at-pasapi-as-ar"
            };

            var client = new PasAccountApiClient(config);
            
            var accountUsers = await client.GetAccountUsers(10004772);
            foreach(var user in accountUsers)
            {
                Console.WriteLine($"{user.DisplayName} {user.EmailAddress}");
            }

            Console.ReadKey();
        }
    }
}
