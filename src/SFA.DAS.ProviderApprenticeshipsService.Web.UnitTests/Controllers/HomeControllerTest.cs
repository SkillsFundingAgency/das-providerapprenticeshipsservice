using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using FluentAssertions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        private Web.Controllers.HomeController _sut;

        [TestCase(true)]
        [TestCase(false)]

        public void ShouldReturnValidViewModel(bool useDfESignIn)
        {
            // arrange
            _sut = new Web.Controllers.HomeController(new ProviderApprenticeshipsServiceConfiguration
            {
                UseDfESignIn = useDfESignIn
            });

            // sut
            var result = _sut.Index();

            var vr = result as ViewResult;

            // assert
            vr.Should().NotBeNull();

            var vm = vr.Model as HomeViewModel;
            vm.Should().NotBeNull();
            vm?.UseDfESignIn.Should().Be(useDfESignIn);
        }
    }
}
