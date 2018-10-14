using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class PutTest
    {
        Mock<ITaskService> _mockService;
        ILogger<TaskController> _mockLogger;
        IETagCache _mockCache;
        IModelValidation _mockModelValidation;
        IUpdateNameDTO _mockUpdateNameDTO;
        IUpdateIsCompletedDTO _updateIsCompletedDTO;
        int toDoId = 193827;

        [OneTimeSetUp]
        public void OneSetUp()
        {
            _mockLogger = new Mock<ILogger<TaskController>>().Object;
            _mockCache = new Mock<IETagCache>().Object;
            _mockModelValidation = new Mock<ModelValidation>().Object;
            _mockUpdateNameDTO = new Mock<UpdateNameDTO>().Object;
            _mockUpdateNameDTO.UserId = 12314;
            _mockUpdateNameDTO.Name = "GoKu";
            _updateIsCompletedDTO = new Mock<UpdateIsCompletedDTO>().Object;
            _updateIsCompletedDTO.UserId = 13123;
        }

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<ITaskService>();
        }

        /// <summary>
        /// test 404 response update name
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutNameFourHundredFourResponse()
        {
            //Arrange        
            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.UpdateTaskName(It.IsAny<int>(), It.IsAny<UpdateNameDTO>())
                ).ReturnsAsync(false);
            var controller = new TaskController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.PutUpdateName(toDoId, It.IsAny<UpdateNameDTO>());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// test  no content response update task name
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutNameNoContentResponse()
        {
            //Arrange    
            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.UpdateTaskName(It.IsAny<int>(), It.IsAny<UpdateNameDTO>())
                ).ReturnsAsync(true);
            var controller = new TaskController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.PutUpdateName(toDoId, It.IsAny<UpdateNameDTO>());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        /// <summary>
        /// test 404 response update completed
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutCompletedFourHundredFourResponse()
        {
            //Arrange        
            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.UpdateIsCompleted(It.IsAny<int>(), It.IsAny<UpdateIsCompletedDTO>())
                ).ReturnsAsync(false);
            var controller = new TaskController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.PutUpdateCompleted(toDoId, It.IsAny<UpdateIsCompletedDTO>());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// test  no content response update task completed
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutCompletedNoContentResponse()
        {
            //Arrange    
            var mockService = new Mock<ITaskService>();
            mockService.Setup(
                service => service.UpdateIsCompleted(It.IsAny<int>(), It.IsAny<UpdateIsCompletedDTO>())
                ).ReturnsAsync(true);
            var controller = new TaskController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.PutUpdateCompleted(toDoId, It.IsAny<UpdateIsCompletedDTO>());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        #region Model Validation

        /// <summary>
        /// update name DTO user id is negtive
        /// </summary>
        [Test]
        public void UpdateTaskFourHundredWhenToDoIdIsNegative()
        {
            //Arrange
            _mockUpdateNameDTO.UserId = -1;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockUpdateNameDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field UserId must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// update name DTO Name is null
        /// </summary>
        [Test]
        public void UpdateTaskFourHundredWhenNameIsNull()
        {
            //Arrange
            _mockUpdateNameDTO.Name = null;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockUpdateNameDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// update name name can't be empty
        /// </summary>
        [Test]
        public void UpdateTaskFourHundredWhenNameIsEmpty()
        {
            //Arrange          
            _mockUpdateNameDTO.Name = string.Empty;
            var errors = _mockModelValidation.ValidateModels(_mockUpdateNameDTO);
            //Act
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// update name name can't exceed 255 in length
        /// </summary>
        [Test]
        public void UpdateTaskFourHundredWhenNameExceedMax()
        {
            //Arrange           
            _mockUpdateNameDTO.Name = "In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act
            var errors = _mockModelValidation.ValidateModels(_mockUpdateNameDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Name must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// update is completed DTO user id is negtive
        /// </summary>
        [Test]
        public void UpdateIsCompleteTaskFourHundredWhenUserIsNegative()
        {
            //Arrange
            _updateIsCompletedDTO.UserId = -1;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_updateIsCompletedDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field UserId must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }
        #endregion
    }
}
