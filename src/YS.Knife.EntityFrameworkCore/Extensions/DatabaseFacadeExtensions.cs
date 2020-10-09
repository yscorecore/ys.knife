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
        public static int ExecuteCommandAsNonQuery(this DatabaseFacade databaseFacade, CommandType commandType,
            string commandText, params DbParameter[] parameters)
        {
            var cmd = databaseFacade.GetDbConnection().CreateCommand();
            cmd.CommandText = commandText;
            cmd.Parameters.AddRange(parameters);
            cmd.CommandType = commandType;
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
        public static object ExecuteCommandAsScalar(this DatabaseFacade databaseFacade, CommandType commandType,
            string commandText, params DbParameter[] parameters)
        {
            var cmd = databaseFacade.GetDbConnection().CreateCommand();
            cmd.CommandText = commandText;
            cmd.Parameters.AddRange(parameters);
            cmd.CommandType = commandType;
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
        public static void ExecuteCommandAsReader(this DatabaseFacade databaseFacade, CommandType commandType,
            string commandText, Action<DbDataReader> readAction, params DbParameter[] parameters)
        {
            var cmd = databaseFacade.GetDbConnection().CreateCommand();
            cmd.CommandText = commandText;
            cmd.Parameters.AddRange(parameters);
            cmd.CommandType = commandType;
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

        public static int ExecuteStoredProcedureAsNonQuery(this DatabaseFacade databaseFacade, string procedureName,
            params DbParameter[] parameters) =>
            databaseFacade.ExecuteCommandAsNonQuery(CommandType.StoredProcedure, procedureName, parameters);


        public static object ExecuteStoredProcedureAsScalar(this DatabaseFacade databaseFacade, string procedureName,
            params DbParameter[] parameters) =>
            databaseFacade.ExecuteCommandAsScalar(CommandType.StoredProcedure, procedureName, parameters);


        public static void ExecuteStoredProcedureAsReader(this DatabaseFacade databaseFacade, string procedureName,
            Action<DbDataReader> readAction, params DbParameter[] parameters) =>
            databaseFacade.ExecuteCommandAsReader(CommandType.StoredProcedure, procedureName, readAction, parameters);


        public static int ExecuteSqlAsNonQuery(this DatabaseFacade databaseFacade,
            string sql, params DbParameter[] parameters) =>
            databaseFacade.ExecuteCommandAsNonQuery(CommandType.Text, sql, parameters);

        public static object ExecuteSqlAsScalar(this DatabaseFacade databaseFacade,
            string sql, params DbParameter[] parameters) =>
            databaseFacade.ExecuteCommandAsScalar(CommandType.Text, sql, parameters);

        public static void ExecuteSqlAsReader(this DatabaseFacade databaseFacade,
            string sql, Action<DbDataReader> readAction, params DbParameter[] parameters) =>
            databaseFacade.ExecuteCommandAsReader(CommandType.Text, sql, readAction, parameters);
    }
}
