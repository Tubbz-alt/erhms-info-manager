﻿using System.Data.SqlClient;
using System.IO;

namespace ERHMS.EpiInfo.DataAccess
{
    public class SqlServerDriver : DataDriverBase
    {
        public static SqlServerDriver Create(DirectoryInfo location, string dataSource, string initialCatalog, string userId = null, string password = null)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = dataSource;
            builder.InitialCatalog = initialCatalog;
            if (userId == null && password == null)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                if (userId != null)
                {
                    builder.UserID = userId;
                }
                if (password != null)
                {
                    builder.Password = password;
                }
            }
            return new SqlServerDriver(initialCatalog, location, builder);
        }

        private SqlConnectionStringBuilder builder;

        private SqlServerDriver(string name, DirectoryInfo location, SqlConnectionStringBuilder builder)
            : base(name, location, DataProvider.SqlServer, builder)
        {
            this.builder = builder;
        }

        public override string GetParameterName(int index)
        {
            return string.Format("@p{0}", index);
        }

        private SqlConnection GetMasterConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(this.builder.ConnectionString);
            builder.InitialCatalog = "master";
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            connection.Open();
            return connection;
        }

        public override bool DatabaseExists()
        {
            string sql = "SELECT 1 FROM sys.databases WHERE name = @name";
            Log.Current.DebugFormat("Executing SQL: {0}", sql);
            using (SqlConnection connection = GetMasterConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@name", builder.InitialCatalog);
                return command.ExecuteScalar() != null;
            }
        }

        public override void CreateDatabase()
        {
            string sql = string.Format("CREATE DATABASE {0}", Escape(builder.InitialCatalog));
            Log.Current.DebugFormat("Executing SQL: {0}", sql);
            using (SqlConnection connection = GetMasterConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
