using Dapper;
using Psycho.Common.Repository.Local;
using Psycho.Laborer.Repo.SpecialModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo
{
    class UserUniversityRepo : Repository<UserUniversity>
    {
        public UserUniversityRepo()
            : base(nameof(UserUniversity))
        {

        }

        public override void Add(IDbConnection cn, UserUniversity item)
        {
            var parameters = Mapping(item);
            cn.Execute(SqliteHelpers.GetInsertQuery(_tableName, parameters, "id"), parameters);
            item.id = cn.QuerySingle<int>("SELECT last_insert_rowid();");
        }

        internal int[] FindByUserId(IDbConnection cn, int userGetid)
        {
            return cn.Query<int>($"SELECT UniversityId FROM {_tableName} WHERE UserGetId=@Id", new { Id = userGetid }).ToArray();
        }
    }
}
