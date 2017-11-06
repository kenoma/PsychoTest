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
    class MilitaryRepo : Repository<Military>
    {
        static private IMapper _mapper;

        static MilitaryRepo()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Military, MilitaryDb>();
                cfg.CreateMap<MilitaryDb, Military>();
            });
            _mapper = config.CreateMapper();
        }

        public MilitaryRepo() 
            : base(nameof(Military))
        {
        }

        public void Add(IDbConnection cn, int userGetId, Military relative)
        {
            var parameters = _mapper.Map<MilitaryDb>(relative);
            parameters.UserGetId = userGetId;
            cn.Execute(SqliteHelpers.GetInsertQuery(_tableName, parameters), parameters);
        }

        public override Military FindById(IDbConnection cn, long id)
        {
            var item = cn.QuerySingleOrDefault<MilitaryDb>($"SELECT * FROM {_tableName} WHERE Id=@Id", new { Id = id });
            return _mapper.Map<Military>(item);
        }

        internal override object Mapping(Military item)
        {
            return _mapper.Map<MilitaryDb>(item);
        }

        internal Military[] FindByUserId(IDbConnection cn, int userGetid)
        {
            return cn.Query<Military>($"SELECT unit, unit_id, country_id, id FROM {_tableName} WHERE UserGetId=@Id", new { Id = userGetid }).ToArray();
        }
    }
}
