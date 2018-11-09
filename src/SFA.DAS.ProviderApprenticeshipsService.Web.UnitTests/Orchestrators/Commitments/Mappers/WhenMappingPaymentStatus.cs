using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingPaymentStatus
    {
        private Mock<ICurrentDateTime> _mockCurrentDateTime;

        private PaymentStatusMapper _mapper;

        [SetUp]
        public void Arrange()
        {
            _mockCurrentDateTime = new Mock<ICurrentDateTime>();
            _mockCurrentDateTime.Setup(x => x.Now).Returns(DateTime.UtcNow);
            _mapper = new PaymentStatusMapper(_mockCurrentDateTime.Object);
        }

        [Test]
        public void ShouldHaveStatusTextForFututreStart()
        {
            var result = _mapper.Map(PaymentStatus.Active, DateTime.UtcNow.AddDays(1));

            result.Should().Be("Waiting to start");
        }

        [Test]
        public void ShouldHaveStatusTextWhenStarted()
        { 
            var result = _mapper.Map(PaymentStatus.Active, DateTime.UtcNow.AddMonths(-5));

            result.Should().Be("Live");
        }

        [Test]
        public void ShouldHaveStatusTextWhenCanceled()
        {
            var result = _mapper.Map(PaymentStatus.Withdrawn, DateTime.UtcNow.AddMonths(-5));

            result.Should().Be("Stopped");
        }

        [Test]
        public void ShouldHaveStatusTextWhenPaused()
        {
            var result = _mapper.Map(PaymentStatus.Paused, DateTime.UtcNow.AddMonths(-5));

            result.Should().Be("Paused");
        }

        [Test]
        public void ShouldHaveStatusTextWhenCompleted()
        {
            var result = _mapper.Map(PaymentStatus.Completed, DateTime.UtcNow.AddMonths(-5));

            result.Should().Be("Finished");
        }
    }
}