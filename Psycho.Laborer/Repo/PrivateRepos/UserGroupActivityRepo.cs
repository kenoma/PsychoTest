using Dapper;
using Psycho.Common.Repository.Local;
using Psycho.Laborer.Repo.SpecialModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo.PrivateRepos
{
    class UserGroupActivityRepo : Repository<UserGroupActivity>
    {
        public UserGroupActivityRepo()
            : base(nameof(UserGroupActivity))
        {

        }

        override public void Add(IDbConnection cn, UserGroupActivity userGroupActivity)
        {
            var parameters = Mapping(userGroupActivity);
            cn.Execute(SqliteHelpers.GetInsertQuery(_tableName, parameters, "id"), parameters);
        }
    }
}
