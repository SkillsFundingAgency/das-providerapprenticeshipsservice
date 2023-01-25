using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.Infrastructure.UnitTests.Services.CookieStorageTests
{
    public class WhenIUseTheCookieStorageService
    {
        private CookieStorageService<TestStorageClass> _cookieStorageService;
        private Mock<ICookieService<TestStorageClass>> _cookieService;
        private Mock<HttpContextAccessor> _httpContextAccessor;
        public const string ExpectedCookieName = "CookieTestName";

        [SetUp]
        public void Arrange()
        {
            _cookieService = new Mock<ICookieService<TestStorageClass>>();

            _httpContextAccessor = new Mock<HttpContextAccessor>();

            _cookieStorageService = new CookieStorageService<TestStorageClass>(_cookieService.Object,
                _httpContextAccessor.Object);
        }

        [Test]
        public void ThenTheInformationIsStoredByCallingTheCookieService()
        {
            //Arrange
            var expectedExpiryDays = 1;

            //Act
            _cookieStorageService.Create(new TestStorageClass(), ExpectedCookieName, expectedExpiryDays);

            //Assert
            _cookieService.Verify(
                x =>
                    x.Create(It.IsAny<HttpContextAccessor>(), ExpectedCookieName, It.IsAny<TestStorageClass>(),
                        expectedExpiryDays));
        }

        [Test]
        public void ThenTheInformationIsReadFromTheCookieService()
        {
            //Arrange
            _cookieService.Setup(x => x.Get(It.IsAny<HttpContextAccessor>(), ExpectedCookieName))
                .Returns(new TestStorageClass());

            //Act
            var actual = _cookieStorageService.Get(ExpectedCookieName);

            //Assert
            Assert.IsAssignableFrom<TestStorageClass>(actual);
        }

        [Test]
        public void ThenTheCookieIsDeletedWhenCalledByName()
        {
            //Act
            _cookieStorageService.Delete(ExpectedCookieName);

            //Assert
            _cookieService.Verify(x => x.Delete(It.IsAny<HttpContextAccessor>(), ExpectedCookieName));
        }

        [Test]
        public void ThenTheCookieIsUpdatedIfItExists()
        {
            //Act
            _cookieStorageService.Update(ExpectedCookieName, new TestStorageClass());

            //Assert
            _cookieService.Verify(
                x => x.Update(It.IsAny<HttpContextAccessor>(), ExpectedCookieName, It.IsAny<TestStorageClass>()));
        }

        public class TestStorageClass
        {

        }
    }
}
