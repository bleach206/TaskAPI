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
using Model.Interface;
using Service.Interface;
using TaskAPI.Controllers;
using TaskTests.Interface;

namespace TaskTests
{
    [TestFixture]
    public class PostTest
    {
        Mock<ITaskService> _mockService;
        ILogger<TaskController> _mockLogger;
        IETagCache _mockCache;
        IModelValidation _mockModelValidation;
        ICreateDTO _mockCreateDTO;
        ICreateTasksDTO _mockCreateTasksDTO;
        List<CreateTasksDTO> _mockCreateTasks;
        readonly int userId = 13241;
        int toDoId = 193827;

        [OneTimeSetUp]
        public void OneSetUp()
        {
            _mockLogger = new Mock<ILogger<TaskController>>().Object;
            _mockCache = new Mock<IETagCache>().Object;
            _mockModelValidation = new Mock<ModelValidation>().Object;
            _mockCreateTasks = new List<CreateTasksDTO>();
        }

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<ITaskService>();
            _mockCreateDTO = new Mock<CreateDTO>().Object;

            _mockCreateDTO.Name = "Protect Daenerys Stormborn";

            _mockCreateTasksDTO = new Mock<CreateTasksDTO>().Object;
            _mockCreateTasksDTO.Name = "Protect Daenerys Stormborn";
        }

        /// <summary>
        /// create task with created route
        /// </summary>
        [Test]
        public async Task CreateTaskReturnCreatedRoute()
        {
            //Arrange  
            var toDoId = 41574;
            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.CreateTask(It.IsAny<int>(), It.IsAny<CreateDTO>())
                ).ReturnsAsync(toDoId);
            var controller = new TaskController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Create(userId, _mockCreateDTO as CreateDTO);
            var resultResponse = result as CreatedAtRouteResult;
            //Assert
            Assert.IsInstanceOf<CreatedAtRouteResult>(resultResponse);
            Assert.AreEqual("Get", resultResponse.RouteName);
        }

        /// <summary>
        /// create to do with return 409 response conflict
        /// </summary>
        [Test]
        public async Task CreateToDoFourHundredNineResponse()
        {
            //Arrange  
            var expected = StatusCodes.Status409Conflict;
            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.CreateTask(It.IsAny<int>(), _mockCreateDTO)
                ).ReturnsAsync(0);
            var controller = new TaskController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Create(userId, _mockCreateDTO as CreateDTO) as StatusCodeResult;
            //Assert
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(expected, result.StatusCode);
        }

        /// <summary>
        /// create task with created route
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CreateToDoListsReturnCreatedRoute()
        {
            //Arrange  
            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.CreateTask(It.IsAny<int>(), It.IsAny<CreateDTO>())
                ).ReturnsAsync(userId);
            var controller = new TaskController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Create(userId, _mockCreateDTO as CreateDTO);
            var resultResponse = result as CreatedAtRouteResult;
            //Assert
            Assert.IsInstanceOf<CreatedAtRouteResult>(resultResponse);
            Assert.AreEqual("Get", resultResponse.RouteName);
        }

        /// <summary>
        /// create tasks with created route
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CreateTaksReturnCreatedRoute()
        {
            //Arrange  
            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.CreateTaskList(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<ICreateTasksDTO>>())
                ).ReturnsAsync(toDoId);
            var controller = new TaskController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.CreateList(userId, toDoId, _mockCreateTasks);
            var resultResponse = result as CreatedAtRouteResult;
            //Assert
            Assert.IsInstanceOf<CreatedAtRouteResult>(resultResponse);
            Assert.AreEqual("Get", resultResponse.RouteName);
        }

        /// <summary>
        /// create tasks bad request response user id is negative
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CreateTasksBadRequestResponseUserId()
        {
            //Arrange              
            var id = -1;
            var controller = new TaskController(_mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.CreateList(id, toDoId, _mockCreateTasks);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        /// <summary>
        /// create tasks bad request response to do id is negative
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CreateTasksBadRequestResponseToDoId()
        {
            //Arrange              
            var toDoId = -1;
            var controller = new TaskController(_mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.CreateList(userId, toDoId, _mockCreateTasks);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        /// <summary>
        /// create tasks with return 409 response conflict
        /// </summary>
        [Test]
        public async Task CreateToDoListFourHundredNineResponse()
        {
            //Arrange  
            var expected = StatusCodes.Status409Conflict;
            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.CreateTaskList(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<ICreateTasksDTO>>())
                ).ReturnsAsync(0);
            var controller = new TaskController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.CreateList(userId, toDoId, _mockCreateTasks) as StatusCodeResult;
            //Assert
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(expected, result.StatusCode);
        }

        #region Model Validation

        /// <summary>
        /// Create DTO ToDoId is negtive
        /// </summary>
        [Test]
        public void CreateTaskFourHundredWhenToDoIdIsNegative()
        {
            //Arrange
            _mockCreateDTO.ToDoId = -1;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockCreateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field ToDoId must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Create DTO Name is null
        /// </summary>
        [Test]
        public void CreateTaskFourHundredWhenNameIsNull()
        {
            //Arrange
            _mockCreateDTO.Name = null;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockCreateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// task name can't be empty
        /// </summary>
        [Test]
        public void CreateTaskFourHundredWhenNameIsEmpty()
        {
            //Arrange          
            _mockCreateDTO.Name = string.Empty;
            var errors = _mockModelValidation.ValidateModels(_mockCreateDTO);
            //Act
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Task name can't exceed 255 in length
        /// </summary>
        [Test]
        public void CreateTaskFourHundredWhenNameExceedMax()
        {
            //Arrange           
            _mockCreateDTO.Name = "In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act
            var errors = _mockModelValidation.ValidateModels(_mockCreateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Name must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Create Tasks DTO Name is null
        /// </summary>
        [Test]
        public void CreateTasksFourHundredWhenNameIsNull()
        {
            //Arrange
            _mockCreateTasksDTO.Name = null;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockCreateTasksDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tasks name can't be empty
        /// </summary>
        [Test]
        public void CreateTasksFourHundredWhenNameIsEmpty()
        {
            //Arrange          
            _mockCreateTasksDTO.Name = string.Empty;
            var errors = _mockModelValidation.ValidateModels(_mockCreateTasksDTO);
            //Act
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tasks name can't exceed 255 in length
        /// </summary>
        [Test]
        public void CreateTasksFourHundredWhenNameExceedMax()
        {
            //Arrange           
            _mockCreateTasksDTO.Name = "In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act
            var errors = _mockModelValidation.ValidateModels(_mockCreateTasksDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Name must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }
        #endregion
    }
}
