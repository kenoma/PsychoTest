using AutoMapper;
using Dapper;
using Psycho.Common.Repository.Local;
using Psycho.Gathering.Models;
using Psycho.Laborer.Repo.SpecialModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo
{
    class UserGroupsRepo : Repository<UserGroups>
    {
        public UserGroupsRepo() 
            : base(nameof(UserGroups))
        {
            
        }

        public override void Add(IDbConnection cn, UserGroups item)
        {
            var parameters = Mapping(item);
            cn.Execute(SqliteHelpers.GetInsertQuery(_tableName, parameters, "id"), parameters);
            item.id = cn.QuerySingle<int>("SELECT last_insert_rowid();");
        }
    }
}
