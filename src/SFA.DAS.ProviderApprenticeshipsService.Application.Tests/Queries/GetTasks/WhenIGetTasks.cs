using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTasks;
using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Tests.Queries.GetTasks
{
    [TestFixture]
    public class WhenIGetTasks
    {
        private GetTasksQueryHandler _handler;
        private Mock<ITasksApi> _tasksApi;

        [SetUp]
        public void Setup()
        {
            _tasksApi = new Mock<ITasksApi>();
            _handler = new GetTasksQueryHandler(_tasksApi.Object);
        }

        [Test]
        public async Task EmptyListReturnedIfNoTasks()
        {
            var expected = new List<Tasks.Api.Types.Task>();

            _tasksApi.Setup(x => x.GetTasks(It.IsAny<string>())).ReturnsAsync(expected);

            var request = new GetTasksQueryRequest
            {
                ProviderId = 23L
            };

            var response = await _handler.Handle(request);

            Assert.That(response.Tasks.Count, Is.EqualTo(expected.Count));
        }
    }
}