using System;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Training;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.UnitTests;

[TestFixture]
public class TrainingTests
{
    private const TrainingType TrainingType = Enums.TrainingType.Framework;
    private const string Title = "Test Title";
    private const int Id = 123;
    private const int Level = 3;
    private const int PathwayCode = 78;

    [Test]
    public void BasicTest()
    {
        var training = new Training(TrainingType, Title, Id, Level, PathwayCode);

        Assert.Multiple(() =>
        {
            Assert.That(training.TrainingType, Is.EqualTo(TrainingType));
            Assert.That(training.Title, Is.EqualTo(Title));
            Assert.That(training.Code, Is.EqualTo($"{Id}-{Level}-{PathwayCode}"));
        });
    }

    [Test]
    public void DecodeFrameworkTest()
    {
        var code = $"{Id}-{Level}-{PathwayCode}";

        var training = Training.Decode(code, Title);

        Assert.Multiple(() =>
        {
            Assert.That(training.TrainingType, Is.EqualTo(TrainingType));
            Assert.That(training.Title, Is.EqualTo(Title));
            Assert.That(training.Id, Is.EqualTo(Id));
            Assert.That(training.Level, Is.EqualTo(Level));
            Assert.That(training.PathwayCode, Is.EqualTo(PathwayCode));
        });
    }

    [Test]
    public void DecodeStandardTest()
    {
        var code = $"{Id}";

        var training = Training.Decode(code, Title);

        Assert.Multiple(() =>
        {
            Assert.That(training.TrainingType, Is.EqualTo(TrainingType.Standard));
            Assert.That(training.Title, Is.EqualTo(Title));
            Assert.That(training.Id, Is.EqualTo(Id));
            Assert.That(training.Level, Is.EqualTo(0));
            Assert.That(training.PathwayCode, Is.EqualTo(0));
        });
    }

    [TestCase("")]
    [TestCase("Fred")]
    [TestCase("123-3")]
    [TestCase("123.1-3-67")]
    [TestCase("123-Fred-3")]
    [TestCase("123-3-Fred")]
    [TestCase("123-3-78-1")]
    [TestCase("-3-67")]
    [TestCase("123--67")]
    [TestCase("123-3-")]
    public void DecodeFailureTest(string invalidCode)
    {
        Assert.Throws<InvalidOperationException>(() => Training.Decode(invalidCode, Title));
    }
}