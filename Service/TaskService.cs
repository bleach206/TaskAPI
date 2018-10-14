using System.Collections.Generic;
using System.Threading.Tasks;

using Model;
using Model.Interface;
using Repository.Interface;
using Service.Interface;

namespace Service
{
    public class TaskService : ITaskService
    {
        #region Field

        private readonly ITaskRepository _repository;
        #endregion

        #region Constructor

        public TaskService(ITaskRepository repository) => _repository = repository;
        #endregion

        #region Method

        public async Task<TaskDTO> GetTaskById(int id, int userId) => await _repository.GetTaskById(id, userId);
        public async Task<IEnumerable<TaskDTO>> GetTaskByPaging(int userId, int toDoId, int skip = 1, int limit = 50) => await _repository.GetTaskByPaging(userId, toDoId, skip, limit);
        public async Task<int> CreateTask(int userId, ICreateDTO task) => await _repository.CreateTask(userId, task);
        public async Task<int> CreateTaskList(int userId, int toDoId, IEnumerable<ICreateTasksDTO> tasks) => await _repository.CreateTaskList(userId, toDoId, tasks);
        public async Task<bool> UpdateTaskName(int id, IUpdateNameDTO updateNameDTO) => await _repository.UpdateTaskName(id, updateNameDTO);
        public async Task<bool> UpdateIsCompleted(int id, IUpdateIsCompletedDTO updateIsCompletedDTO) => await _repository.UpdateIsCompleted(id, updateIsCompletedDTO);
        #endregion
    }
}
