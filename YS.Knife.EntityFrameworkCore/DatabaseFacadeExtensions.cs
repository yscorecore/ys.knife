using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Data;
using System.Data.Common;

namespace Microsoft.EntityFrameworkCore
{
    public static class DatabaseFacadeExtensions
    {
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
