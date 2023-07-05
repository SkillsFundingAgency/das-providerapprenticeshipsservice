using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.AcademicYear;

[TestFixture]
public class WhenDeterminingTheAcademicYear
{
    private Mock<ICurrentDateTime> _currentDateTime;
    private IAcademicYearDateProviderService _academicYear;

    [SetUp]
    public void Arrange()
    {
        _currentDateTime = new Mock<ICurrentDateTime>();
    }

    [TestCase("2017-08-01", "2017-08-01", "2018-07-31", "2017-10-19 18:00")]
    [TestCase("2018-03-01", "2017-08-01", "2018-07-31", "2017-10-19 18:00")]
    [TestCase("2018-07-31", "2017-08-01", "2018-07-31", "2017-10-19 18:00")]
    [TestCase("2018-10-01", "2018-08-01", "2019-07-31", "2018-10-19 18:00")]
    [TestCase("2018-01-01", "2017-08-01", "2018-07-31", "2017-10-19 18:00")]
    public void ThenAcademicYearRunsAugustToJuly(DateTime currentDate, DateTime expectedYearStart, DateTime expectedYearEnd, DateTime expectedLastAcademicYearFundingPeriod)
    {
        //Arrange
        _currentDateTime.Setup(x => x.Now).Returns(currentDate);
        _academicYear = new Infrastructure.Services.AcademicYearDateProviderService(_currentDateTime.Object);

        //Act
        var actualStart = _academicYear.CurrentAcademicYearStartDate;
        var actualEnd = _academicYear.CurrentAcademicYearEndDate;
        var actualLastAcademicYearFundingPeriod = _academicYear.LastAcademicYearFundingPeriod;

        //Assert
        Assert.That(expectedYearStart, Is.EqualTo(actualStart));
        Assert.That(expectedYearEnd, Is.EqualTo(actualEnd));
        Assert.That(expectedLastAcademicYearFundingPeriod, Is.EqualTo(actualLastAcademicYearFundingPeriod));
    }
}