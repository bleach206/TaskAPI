using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using Common.Interface;
using Model;
using Service.Interface;
using TaskAPI.Controllers;
using TaskTests.Interface;

namespace TaskTests
{
    [TestFixture]
    public class GetTest
    {
        Mock<ITaskService> _mockService;
        ILogger<TaskController> _mockLogger;
        IETagCache _mockCache;
        IModelValidation _mockModelValidation;
        TaskDTO _taskDTO;
        GetTasksDTO _getTaskDTO;
        int taskId = 2548;
        int toDoId = 193827;
        readonly int userId = 13241;
    
        [OneTimeSetUp]
        public void OneSetUp()
        {
            _mockLogger = new Mock<ILogger<TaskController>>().Object;
            _mockCache = new Mock<IETagCache>().Object;
            _mockModelValidation = new Mock<ModelValidation>().Object;
        }

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<ITaskService>();
            _taskDTO = new TaskDTO
            {
                Id = taskId,
                Name = "Dragon Ball Z",
                IsCompleted = false,
                RowVersion = new byte[] {4,2 }
            };
            _getTaskDTO = new GetTasksDTO
            {
                Id = toDoId
            };
        }

        /// <summary>
        /// send task id of negative number should response with bad request
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetTaskByIdReturnBadRequst()
        {
            //Arrange
            var id = -1;
            var controller = new TaskController(_mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Get(id, taskId);
            //Assert
            Assert.IsNotNull(id);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        /// <summary>
        /// send user id of negative number should response with bad request
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetTaskByIdUserIdNegtiveNumberReturnBadRequst()
        {
            //Arrange
            var id = 1;
            var userIdNumber = -1;
            var controller = new TaskController(_mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Get(userIdNumber, id);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        /// <summary>
        /// Get User task return 200 response after cache is set
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserTaskTwoHundredResponseCache()
        {
            //Arrange                 
            var cacheName = $"task-{taskId}";
            var mockCache = new Mock<IETagCache>();           
            mockCache.Setup(
               cache => cache.SetCachedObject(It.IsAny<string>(), It.IsAny<TaskDTO>(), It.IsAny<byte[]>(), It.IsAny<int>())
               ).Returns(true);
            mockCache.Setup(
               cache => cache.GetCachedObject<TaskDTO>(cacheName)
               ).Returns((TaskDTO)null);

            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.GetTaskById(It.IsAny<int>(), It.IsAny<int>())
                ).ReturnsAsync(_taskDTO);
            //Act
            var controller = new TaskController(mockService.Object, _mockLogger, mockCache.Object);
            var result = await controller.Get(userId, taskId);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        /// <summary>
        /// Get User task return 404 response 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetTaskFourHundredFourResponseCache()
        {
            //Arrange                 
            var cacheName = $"task-{taskId}";
            var mockCache = new Mock<IETagCache>();
            mockCache.Setup(
               cache => cache.SetCachedObject(It.IsAny<string>(), It.IsAny<TaskDTO>(), It.IsAny<byte[]>(), It.IsAny<int>())
               ).Returns(true);
            mockCache.Setup(
               cache => cache.GetCachedObject<TaskDTO>(cacheName)
               ).Returns((TaskDTO)null);

            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.GetTaskById(It.IsAny<int>(), It.IsAny<int>())
                ).ReturnsAsync((TaskDTO)null);
            //Act
            var controller = new TaskController(mockService.Object, _mockLogger, mockCache.Object);
            var result = await controller.Get(userId, taskId);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// Return 304 response because we have a cache version to do
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserTaskCacheThreeHundredFourResponse()
        {
            //Arrange
            var expected = StatusCodes.Status304NotModified;
            var mockCache = new Mock<IETagCache>();
            mockCache.Setup(
               cache => cache.GetCachedObject<TaskDTO>($"task-{taskId}")
               ).Returns(_taskDTO);
            //Act
            var controller = new TaskController(new Mock<ITaskService>().Object, _mockLogger, mockCache.Object);
            var result = await controller.Get(userId, taskId) as StatusCodeResult;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(expected, result.StatusCode);
        }

        /// <summary>
        /// Get User tasks return 200 response after cache is set
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserTasksTwoHundredResponseCache()
        {
            //Arrange                 
            var cacheName = $"task-{_getTaskDTO.Id}";
            var list = new List<TaskDTO>();
            var mockRow = new byte[] { 0, 4 };
            list.Add(new TaskDTO
            {                
                Name = "Protect Queen of the south",
                Id = 12154,
                RowVersion = mockRow
            });
            var mockCache = new Mock<IETagCache>();
            mockCache.Setup(
               cache => cache.SetCachedObject(It.IsAny<string>(), It.IsAny<IEnumerable<TaskDTO>>(), It.IsAny<byte[]>(), It.IsAny<int>())
               ).Returns(true);
            mockCache.Setup(
               cache => cache.GetCachedObject<IEnumerable<TaskDTO>>(cacheName)
               ).Returns((IEnumerable<TaskDTO>)null);

            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.GetTaskByPaging(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())
                ).ReturnsAsync(list);
            //Act
            var controller = new TaskController(mockService.Object, _mockLogger, mockCache.Object);
            var result = await controller.Get(userId, _getTaskDTO);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        /// <summary>
        /// Get User tasks return 404
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserTasksFourHundredFourResponseCache()
        {
            //Arrange                 
            var cacheName = $"task-{_getTaskDTO.Id}";
            var list = new List<TaskDTO>();
            var mockRow = new byte[] { 0, 4 };
            list.Add(new TaskDTO
            {
                Name = "Protect Queen of the south",
                Id = 12154,
                RowVersion = mockRow
            });
            var mockCache = new Mock<IETagCache>();
            mockCache.Setup(
               cache => cache.SetCachedObject(It.IsAny<string>(), It.IsAny<IEnumerable<TaskDTO>>(), It.IsAny<byte[]>(), It.IsAny<int>())
               ).Returns(false);          

            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.GetTaskByPaging(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())
                ).ReturnsAsync((IEnumerable<TaskDTO>)null);
            //Act
            var controller = new TaskController(mockService.Object, _mockLogger, mockCache.Object);
            var result = await controller.Get(userId, _getTaskDTO);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// Return 304 response because we have a cache version
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserTaskListCacheThreeHundredFourResponse()
        {
            //Arrange
            var expected = StatusCodes.Status304NotModified;
            var list = new List<TaskDTO>();
            list.Add(new TaskDTO
            {        
                Name = "Protect Queen of the south",
                Id = 12154
            });
            var mockCache = new Mock<IETagCache>();
            mockCache.Setup(
               cache => cache.GetCachedObject<IEnumerable<TaskDTO>>($"task-{_getTaskDTO.Id}")
               ).Returns(list);
            //Act
            var controller = new TaskController(new Mock<ITaskService>().Object, _mockLogger, mockCache.Object);
            var result = await controller.Get(userId, _getTaskDTO) as StatusCodeResult;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(expected, result.StatusCode);
        }

        #region Model Test

        /// <summary>
        /// Task DTO id can't be negative 
        /// </summary>
        [Test]
        public void TaskDtoFourHundredWhenIdIsNegative()
        {
            //Arrange
            _taskDTO.Id = -1;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_taskDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Id must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Task name can't exceed 255 in length
        /// </summary>
        [Test]
        public void TaskDtoFourHundredWhenNameExceedMax()
        {
            //Arrange           
            _taskDTO.Name = "In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act
            var errors = _mockModelValidation.ValidateModels(_taskDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Name must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Task name can't be null
        /// </summary>
        [Test]
        public void TaskDtoFourHundredWhenNameNotNull()
        {
            //Arrange           
            _taskDTO.Name = null;
            //Act
            var errors = _mockModelValidation.ValidateModels(_taskDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// GetTasks DTO id can't be negative 
        /// </summary>
        [Test]
        public void GetTasksDtoFourHundredWhenIdIsNegative()
        {
            //Arrange
            _getTaskDTO.Id = -1;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_getTaskDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Id must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// GetTasks DTO limit can't exceed 50 
        /// </summary>
        [Test]
        public void GetTasksDtoFourHundredWhenLimitExceed()
        {
            //Arrange
            _getTaskDTO.Limit = 51;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_getTaskDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Limit must be between 1 and 50."));
            //Assert
            Assert.IsTrue(actual);
        }
        #endregion
    }
}
