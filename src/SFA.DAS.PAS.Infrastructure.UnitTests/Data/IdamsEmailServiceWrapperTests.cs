using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.PAS.Infrastructure.UnitTests.Data
{
    [TestFixture]
    public class IdamsEmailServiceWrapperTests
    {
        private IdamsEmailServiceWrapper _sut;
        private Mock<IHttpClientWrapper> _mockHttpClientWrapper;

        [SetUp]
        public void SetUp()
        {
            var config = new ProviderApprenticeshipsServiceConfiguration
            {
                CommitmentNotification = new ProviderNotificationConfiguration
                {
                    IdamsListUsersUrl =
                        "https://url.to/users/ukprn={0}",
                    Audience = "some-audience",
                    ClientSecret = "some-secret",
                    Issuer = "some-issuer"
                },
                
            };
            _mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
            _sut = new IdamsEmailServiceWrapper(Mock.Of<ILog>(), config, _mockHttpClientWrapper.Object, new NoopExecutionPolicy());
        }

        [Test]
        public async Task ShouldReturnEmpltyListIfResponseIsEmpty()
        {
            var res = await _sut.GetEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ShouldReturnEmptyListIfResponseNotInCorrectFormat()
        {

            var mockResponse = "{\"result\": {\"name.family.name\": [\"James\"],\"name.given.name\": [\"Sally\"],\"name.title\": [\"Miss\"]}}";
            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ShouldReturnEmailFromResult()
        {
            var mockResponse = "{\"ukprn\":\"10005143\",\"users\":[{\"email\":\"test.user1@education.gov.uk\",\"firstName\":\"test\",\"lastName\":\"user1\",\"roles\":[\"DAA\",\"FAA\"]}]}";
            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
            var res = await _sut.GetEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res[0], Is.EqualTo("test.user1@education.gov.uk"));
        }

        [Test, Ignore("Fixing")]
        public async Task ShouldReturnEmailsFromResult()
        {
            var mockResponse = "{\"ukprn\":\"10005143\",\"users\":[{\"email\":\"test.user1@education.gov.uk\",\"firstName\":\"test\",\"lastName\":\"user1\",\"roles\":[\"DAA\",\"FAA\"]},{\"email\":\"test.user2@education.gov.uk\",\"firstName\":\"test\",\"lastName\":\"user2\",\"roles\":[\"DAA\"]}]}";

            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(2));
            Assert.That(res[0], Is.EqualTo("test.user1@education.gov.uk"));
            Assert.That(res[1], Is.EqualTo("test.user2@education.gov.uk"));
        }

        [Test]
        public async Task ShouldReturnEmptyListIfResponseIsEmptyForSuperUser()
        {
            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ShouldReturnEmptyListIfResponseNotInCorrectFormatSuperUser()
        {

            var mockResponse = "{\"result\": {\"name.family.name\": [\"James\"],\"name.given.name\": [\"Sally\"],\"name.title\": [\"Miss\"]}}";
            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ShouldReturnEmailFromResultSuperUser()
        {
            var mockResponse = "{\"ukprn\":\"10005143\",\"users\":[{\"email\":\"test.user1@education.gov.uk\",\"firstName\":\"test\",\"lastName\":\"user1\",\"roles\":[\"DAA\",\"FAA\"]}]}";
            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res[0], Is.EqualTo("test.user1@education.gov.uk"));
        }

        [Test]
        public async Task ShouldReturnEmailsFromResultSuperUser()
        {
            var mockResponse = "{\"ukprn\":\"10005143\",\"users\":[{\"email\":\"test.user1@education.gov.uk\",\"firstName\":\"test\",\"lastName\":\"user1\",\"roles\":[\"DAA\",\"FAA\"]},{\"email\":\"test.user2@education.gov.uk\",\"firstName\":\"test\",\"lastName\":\"user2\",\"roles\":[\"DAA\"]}]}";

            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(2));
            Assert.That(res[0], Is.EqualTo("test.user1@education.gov.uk"));
            Assert.That(res[1], Is.EqualTo("test.user2@education.gov.uk"));
        }
    }
}
