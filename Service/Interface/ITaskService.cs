using System.Collections.Generic;
using System.Threading.Tasks;

using Model;
using Model.Interface;

namespace Service.Interface
{
    public interface ITaskService
    {
        Task<TaskDTO> GetTaskById(int id, int userId);
        Task<IEnumerable<TaskDTO>> GetTaskByPaging(int userId, int toDoId, int skip = 1, int limit = 50);
        Task<int> CreateTask(int userId, ICreateDTO task);
        Task<int> CreateTaskList(int userId, int toDoId, IEnumerable<ICreateTasksDTO> tasks);
        Task<bool> UpdateTaskName(int id, IUpdateNameDTO updateNameDTO);
        Task<bool> UpdateIsCompleted(int id, IUpdateIsCompletedDTO updateIsCompletedDTO);
    }
}
