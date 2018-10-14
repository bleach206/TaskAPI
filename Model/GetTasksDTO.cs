using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using Model.Interface;

namespace Model
{
    public class GetTasksDTO : IGetTasksDTO
    {
        /// <summary>
        /// to do id
        /// </summary>
        [Range(1, int.MaxValue)]
        [FromQuery(Name = "toDoId")]
        public int Id { get; set; }
        /// <summary>
        /// number of records to skip for pagination
        /// </summary>
        [Range(1, int.MaxValue)]
        [FromQuery(Name = "skip")]
        public int Skip { get; set; } = 1;
        /// <summary>
        /// maximum number of records to return
        /// </summary>        
        [Range(1, 50)]
        [FromQuery(Name = "limit")]
        public int Limit { get; set; } = 50;      
    }
}
