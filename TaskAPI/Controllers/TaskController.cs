using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using Service.Interface;

namespace TaskAPI.Controllers
{
    /// <summary>
    /// Task api doing the small things in life that adds up
    /// </summary>
    [Route("api/v1/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        #region Fields

        private readonly ITaskService _service;
        private readonly ILogger _logger;
        private readonly IETagCache _cache;
        #endregion

        private const int _cacheTimeMinutes = 3;

        #region Constructor
        /// <summary>
        /// task constructor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        public TaskController(ITaskService service, ILogger<TaskController> logger, IETagCache cache) => (_service, _logger, _cache) = (service, logger, cache);
        #endregion

        /// <summary>
        /// return the specified task
        /// </summary>
        /// <remarks>Parameters</remarks>   
        /// <param name="id">User Id</param>
        /// <param name="taskId">The unique identifier of task</param>
        /// <response code="200">successful operation</response>
        /// <response code="304">Not Modified</response>
        /// <response code="400">Invalid id supplied</response>
        /// <response code="404">List not found</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [ProducesResponseType(200, Type = typeof(TaskDTO))]
        [ProducesResponseType(304)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("{taskId}/users/{id}/tasks", Name = "Get")]
        public async Task<IActionResult> Get(int id, int taskId)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0 || taskId <= 0)
                    return BadRequest();

                var task = _cache.GetCachedObject<TaskDTO>($"task-{taskId}");

                if (task == null)
                    task = await _service.GetTaskById(id, taskId);

                if (task == null)
                    return NotFound();

                var isChanged = _cache.SetCachedObject($"task-{taskId}", task, task.RowVersion, _cacheTimeMinutes);
                if (isChanged)
                {
                    return Ok(task);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task by id");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// returns all of the available task for a single to do
        /// </summary>
        /// <remarks>Gets a list of user tasks. <br />  id: user id. <br />  toDoId: to do id. <br /> skip: number of records to skip for pagination. (default is 1) <br/>  limit: the maximum number of records to return (Max is 50 default to 50) </remarks>
        /// <param name="id">user Id</param>
        /// <param name="getTasksDTO">query paramter DTO</param>       
        /// <response code="200">successful operation</response>
        /// <response code="304">Not Modified</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">List not found</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [HttpGet("users/{id}/tasks")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TaskDTO>))]
        [ProducesResponseType(304)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get(int id, [FromQuery] GetTasksDTO getTasksDTO)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0)
                    return BadRequest();

                var list = _cache.GetCachedObject<IEnumerable<TaskDTO>>($"task-{getTasksDTO.Id}");

                if (list == null)
                    list = await _service.GetTaskByPaging(id, getTasksDTO.Id, getTasksDTO.Skip, getTasksDTO.Limit);

                if (!list.Any())
                    return NotFound();

                var rowVersion = list.FirstOrDefault().RowVersion;
                var isChanged = _cache.SetCachedObject($"task-{getTasksDTO.Id}", list, rowVersion, _cacheTimeMinutes);
                if (isChanged)
                {
                    return Ok(list);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ToDo list");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// creates a task
        /// </summary>
        /// <remarks>Adds a to do into the system</remarks>
        /// <param name="id">User Id</param>
        /// <param name="task">task to add</param>
        /// <response code="201">item created</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="409">an existing item already exists</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [HttpPost("users/{id}/tasks")]
        [ProducesResponseType(201, Type = typeof(CreateDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create(int id, [FromBody] CreateDTO task)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0 || task == null)
                    return BadRequest();

                var Id = await _service.CreateTask(id, task);

                if (Id.Equals(0))
                    return StatusCode(StatusCodes.Status409Conflict);

                return CreatedAtRoute("Get", new { id, taskId = Id }, task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating ToDo");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// creates a task list
        /// </summary>
        /// <remarks>Adds a to do list into the system</remarks>
        /// <param name="id">User Id</param>
        /// <param name="toDoId">To do lists</param>
        /// <param name="tasks">To do lists</param>
        /// <response code="201">item created</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="409">an existing item already exists</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [HttpPost("users/{id}/tasks/{toDoId}")]
        [ProducesResponseType(201, Type = typeof(IEnumerable<CreateDTO>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateList(int id, int toDoId, [FromBody] IEnumerable<CreateTasksDTO> tasks)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0 || toDoId <= 0 || tasks == null)
                    return BadRequest();

                var Id = await _service.CreateTaskList(id, toDoId, tasks);

                if (Id.Equals(0))
                    return StatusCode(StatusCodes.Status409Conflict);

                return CreatedAtRoute("Get", new { id, toDoId }, tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating to do lists");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update to do Name
        /// </summary>
        /// <remarks>update to do name</remarks>
        /// <param name="id">To do id</param>
        /// <param name="updateNameDTO"> dto holding name and user id</param>        
        /// <response code="204">to do updated</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">server error</response>
        /// <returns>no content</returns>
        [HttpPut("update/name/{id}/")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutUpdateName(int id, [FromBody]UpdateNameDTO updateNameDTO)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0)
                    return BadRequest();

                var update = await _service.UpdateTaskName(id, updateNameDTO);

                if (!update)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update ToDo Name");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update to do IsCompleted
        /// </summary>
        /// <remarks>update to do name</remarks>
        /// <param name="id">To do id</param>
        /// <param name="updateIsCompletedDTO"> dto holding is completed and user id</param>        
        /// <response code="204">to do updated</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">server error</response>
        /// <returns>no content</returns>
        [HttpPut("update/completed/{id}/")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutUpdateCompleted(int id, [FromBody]UpdateIsCompletedDTO updateIsCompletedDTO)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0)
                    return BadRequest();

                var update = await _service.UpdateIsCompleted(id, updateIsCompletedDTO);
                if (!update)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update ToDo Name");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
