using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public static class DatabaseFacadeExtensions
    {
        [SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        public static int ExecuteStoredProcedureAsNonQuery(this DatabaseFacade databaseFacade, string procedureName, params DbParameter[] parameters)
        {
            var cmd = databaseFacade.GetDbConnection().CreateCommand();
            cmd.CommandText = procedureName;
            cmd.Parameters.AddRange(parameters);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                databaseFacade.OpenConnection();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                databaseFacade.CloseConnection();
            }
        }

        [SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        public static object ExecuteStoredProcedureAsScalar(this DatabaseFacade databaseFacade, string procedureName, params DbParameter[] parameters)
        {
            var cmd = databaseFacade.GetDbConnection().CreateCommand();
            cmd.CommandText = procedureName;
            cmd.Parameters.AddRange(parameters);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                databaseFacade.OpenConnection();
                return cmd.ExecuteScalar();
            }
            finally
            {
                databaseFacade.CloseConnection();
            }
        }

        [SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        public static void ExecuteStoredProcedureAsReader(this DatabaseFacade databaseFacade, string procedureName, Action<DbDataReader> readAction, params DbParameter[] parameters)
        {
            var cmd = databaseFacade.GetDbConnection().CreateCommand();
            cmd.CommandText = procedureName;
            cmd.Parameters.AddRange(parameters);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                databaseFacade.OpenConnection();
                readAction?.Invoke(cmd.ExecuteReader());
            }
            finally
            {
                databaseFacade.CloseConnection();
            }
        }
    }
}
