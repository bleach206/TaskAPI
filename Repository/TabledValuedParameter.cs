using System.Data;
using System.Data.SqlClient;

using Dapper;

namespace Repository
{
    public class TabledValuedParameter : SqlMapper.ICustomQueryParameter
    {
        #region Fields

        private readonly DataTable _dataTable;
        private readonly string _typeName;
        #endregion

        #region Constructor

        public TabledValuedParameter(DataTable dataTable, string tableName) => (_dataTable, _typeName) = (dataTable, tableName);
        #endregion

        #region Method

        public void AddParameter(IDbCommand command, string name)
        {
            var paramter = (SqlParameter)command.CreateParameter();

            paramter.ParameterName = name;
            paramter.SqlDbType = SqlDbType.Structured;
            paramter.Value = _dataTable;
            paramter.TypeName = _typeName;

            command.Parameters.Add(paramter);
        }
        #endregion        
    }
}
