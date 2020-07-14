﻿using System.Data;
using System.Data.Common;

namespace ERHMS.Data
{
    public interface IDatabase
    {
        DatabaseType Type { get; }
        DbConnectionStringBuilder ConnectionStringBuilder { get; }
        string ConnectionString { get; }
        string Name { get; }

        bool Exists();
        bool TableExists(string name);
        void Create();
        IDbConnection Connect();
        string Quote(string identifier);
    }
}