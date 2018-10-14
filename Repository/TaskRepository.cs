using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Common;
using Model;
using Model.Interface;
using Repository.Interface;

namespace Repository
{
    public class TaskRepository : ITaskRepository
    {
        #region Fields

        private readonly string _connection;
        #endregion

        #region Constructor

        public TaskRepository(string connection) => _connection = connection;
        #endregion

        #region Method

        public async Task<TaskDTO> GetTaskById(int id, int userId)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var queryParamete = new DynamicParameters();
                    queryParamete.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParamete.Add("@UserId", dbType: DbType.Int32, value: userId);
                    var data = await cnn.QueryAsync<TaskDTO>("[dbo].[usp_GetTaskById]",
                        param: queryParamete,
                        commandType: CommandType.StoredProcedure);
                    return data.FirstOrDefault();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<TaskDTO>> GetTaskByPaging(int userId, int toDoId, int skip = 1, int limit = 50)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@ToDoId", dbType: DbType.Int32, value: toDoId);
                    queryParameter.Add("@UserId", dbType: DbType.Int32, value: userId);
                    queryParameter.Add("@PageNumber", dbType: DbType.Int32, value: skip);
                    queryParameter.Add("@PageSize", dbType: DbType.Int32, value: limit);

                    var data = await cnn.QueryAsync<TaskDTO>("[dbo].[usp_GetTaskBySearchTermAndPageNumberAndPageSize]",
                    param: queryParameter,
                    commandType: CommandType.StoredProcedure);
                    return data;
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CreateTask(int userId, ICreateDTO task)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@ToDoId", dbType: DbType.Int32, value: task.ToDoId);
                    queryParameter.Add("@UserId", dbType: DbType.Int32, value: userId);
                    queryParameter.Add("@Name", value: task.Name);
                    return await cnn.ExecuteScalarAsync<int>("[dbo].[usp_InsertTask]", param: queryParameter, commandType: CommandType.StoredProcedure);                  
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CreateTaskList(int userId, int toDoId, IEnumerable<ICreateTasksDTO> tasks)
        {
            try
            {
                var sql = "[dbo].[usp_InsertTaskList]";
                var tdvp = new TabledValuedParameter(tasks.CopyToDataTable(), "TaskTableType");
                using (var cnn = new SqlConnection(_connection))
                {
                    return await cnn.ExecuteScalarAsync<int>(sql, new { UserId = userId, TDVP = tdvp }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateTaskName(int id, IUpdateNameDTO updateNameDTO)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParameter.Add("@UserId", dbType: DbType.Int32, value: updateNameDTO.UserId);
                    queryParameter.Add("@Name", value: updateNameDTO.Name, size: 255);
                    var result = await cnn.ExecuteAsync("[dbo].[usp_UpdateTaskName]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return Convert.ToBoolean(result);
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateIsCompleted(int id, IUpdateIsCompletedDTO updateIsCompletedDTO)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParameter.Add("@UserId", dbType: DbType.Int32, value: updateIsCompletedDTO.UserId);
                    queryParameter.Add("@IsCompleted", dbType: DbType.Boolean, value: updateIsCompletedDTO.IsCompleted);
                    var result = await cnn.ExecuteAsync("[dbo].[usp_UpdateTaskIsCompleted]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return Convert.ToBoolean(result);
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
