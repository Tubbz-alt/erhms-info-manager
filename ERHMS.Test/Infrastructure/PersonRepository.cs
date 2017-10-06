﻿using Dapper;
using ERHMS.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Test
{
    public class PersonRepository : Repository<Person>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Person));
            typeMap.Get(nameof(Person.PersonId)).SetId();
            typeMap.Get(nameof(Person.Gender)).SetComputed();
            typeMap.Get(nameof(Person.Bmi)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Person), typeMap);
        }

        public PersonRepository(IDatabase database)
            : base(database) { }

        public override IEnumerable<Person> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                string format = @"
                    SELECT [Person].*, NULL AS [Separator], [Gender].*
                    FROM [Person]
                    INNER JOIN [Gender] ON [Person].[GenderId] = [Gender].[GenderId]
                    {0}";
                string sql = string.Format(format, clauses);
                Func<Person, Gender, Person> map = (person, gender) =>
                {
                    person.Gender = gender;
                    return person;
                };
                return connection.Query(sql, map, parameters, transaction, splitOn: "Separator");
            });
        }

        public override Person SelectById(object id)
        {
            string clauses = "WHERE [Person].[PersonId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }
    }
}
