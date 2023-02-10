using System;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Training;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.UnitTests
{
    [TestFixture]
    public class TrainingTests
    {
        private const TrainingType trainingType = TrainingType.Framework;
        private const string title = "Test Title";
        private const int id = 123;
        private const int level = 3;
        private const int pathwayCode = 78;

        [Test]
        public void BasicTest()
        {
            var training = new Training(trainingType, title, id, level, pathwayCode);

            Assert.That(training.TrainingType, Is.EqualTo(trainingType));
            Assert.That(training.Title, Is.EqualTo(title));
            Assert.That(training.Code, Is.EqualTo($"{id}-{level}-{pathwayCode}"));
        }

        [Test]
        public void DecodeFrameworkTest()
        {
            var code = $"{id}-{level}-{pathwayCode}";

            var training = Training.Decode(code, title);

            Assert.That(training.TrainingType, Is.EqualTo(trainingType));
            Assert.That(training.Title, Is.EqualTo(title));
            Assert.That(training.Id, Is.EqualTo(id));
            Assert.That(training.Level, Is.EqualTo(level));
            Assert.That(training.PathwayCode, Is.EqualTo(pathwayCode));
        }

        [Test]
        public void DecodeStandardTest()
        {
            var code = $"{id}";

            var training = Training.Decode(code, title);

            Assert.That(training.TrainingType, Is.EqualTo(TrainingType.Standard));
            Assert.That(training.Title, Is.EqualTo(title));
            Assert.That(training.Id, Is.EqualTo(id));
            Assert.That(training.Level, Is.EqualTo(0));
            Assert.That(training.PathwayCode, Is.EqualTo(0));
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
            Assert.Throws<InvalidOperationException>(() => Training.Decode(invalidCode, title));
        }
    }
}
