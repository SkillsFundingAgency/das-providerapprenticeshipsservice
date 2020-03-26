using System;
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
                    ClientToken = "AbbA-Rules-4.Ever"
                },
                
            };
            _mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
            _sut = new IdamsEmailServiceWrapper(Mock.Of<ILog>(), config, _mockHttpClientWrapper.Object);
        }

        [Test]
        public void ShouldThrowIfGetUsersResponseIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _sut.GetEmailsAsync(10005143L));
        }

        [Test]
        public void ShouldThrowIfGetUsersResponseIsInvalid()
        {
            var mockResponse = "THIS-IS-NOT-JSON";
            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
            Assert.ThrowsAsync<ArgumentException>(() => _sut.GetEmailsAsync(10005143L));
        }

        [Test]
        public async Task ShouldReturnEmailFromResult()
        {
            var mockResponse = "{\"result\": {\"name.familyname\": [\"James\"],\"emails\": [\"abba@email.uk\"],\"name.givenname\": [\"Sally\"],\"Title\": [\"Miss\"]}}";
            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
            var res = await _sut.GetEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res[0], Is.EqualTo("abba@email.uk"));
        }

        [Test]
        public async Task ShouldReturnEmailFromResultWithMultiResult()
        {
            var mockResponse = "{\"result\": [{\"name.familyname\": [\"James\"],\"emails\": [\"abba@email.uk\"],\"name.givenname\": [\"Sally\"],\"Title\": [\"Miss\"]}]}";
            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
            var res = await _sut.GetEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res[0], Is.EqualTo("abba@email.uk"));
        }

        [Test, Ignore("Fixing")]
        public async Task ShouldReturnEmailsFromResult()
        {
            var mockResponse = "{\"result\": {\"name.familyname\": [\"James\", \"Octavo\"],\"emails\": "
                               + "[\"abba@email.uk\", \"test@email.uk\"],\"name.givenname\": [\"Sally\", \"Chris\"],\"Title\": [\"Miss\", \"Mr\"]}}";

            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(2));
            Assert.That(res[0], Is.EqualTo("abba@email.uk"));
            Assert.That(res[1], Is.EqualTo("test@email.uk"));
        }

        [Test]
        public void ShouldThrowIfGetSupersUsersResponseIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _sut.GetSuperUserEmailsAsync(10005143L));
        }

        [Test]
        public void ShouldThrowIGetSuperUserResponseIsInvalid()
        {
            var mockResponse = "THIS-IS-NOT-JSON";
            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
            Assert.ThrowsAsync<ArgumentException>(() => _sut.GetSuperUserEmailsAsync(10005143L));
        }

        [Test]
        public async Task ShouldReturnEmailFromResultSuperUser()
        {
            var mockResponse = "{\"result\": {\"name.familyname\": [\"James\"],\"emails\": [\"abba@email.uk\"],\"name.givenname\": [\"Sally\"],\"Title\": [\"Miss\"]}}";
            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res[0], Is.EqualTo("abba@email.uk"));
        }

        [Test]
        public async Task ShouldReturnEmailsFromResultSuperUser()
        {
            var mockResponse = "{\"result\": {\"name.familyname\": [\"James\", \"Octavo\"],\"emails\": "
                               + "[\"abba@email.uk\", \"test@email.uk\"],\"name.givenname\": [\"Sally\", \"Chris\"],\"Title\": [\"Miss\", \"Mr\"]}}";

            _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            Assert.That(res.Count, Is.EqualTo(2));
            Assert.That(res[0], Is.EqualTo("abba@email.uk"));
            Assert.That(res[1], Is.EqualTo("test@email.uk"));
        }
    }
}
