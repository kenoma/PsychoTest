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
    class RelativeRepo : Repository<Relative>
    {
        static private IMapper _mapper;

        static RelativeRepo()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Relative, RelativeDb>()
                    .ForMember(z => z.RelativeId, o => o.MapFrom(z => z.id));
                cfg.CreateMap<RelativeDb, Relative>()
                    .ForMember(z => z.id, o => o.MapFrom(z => z.RelativeId));
            });
            _mapper = config.CreateMapper();
        }

        public RelativeRepo()
            : base(nameof(Relative))
        {

        }

        public void Add(IDbConnection cn, int userGetId, Relative relative)
        {
            var parameters = _mapper.Map<RelativeDb>(relative);
            parameters.UserGetId = userGetId;
            cn.Execute(SqliteHelpers.GetInsertQuery(_tableName, parameters, "id"), parameters);
        }

        public override Relative FindById(IDbConnection cn, long id)
        {
            var item = cn.QuerySingleOrDefault<RelativeDb>($"SELECT * FROM {_tableName} WHERE Id=@Id", new { Id = id });
            return _mapper.Map<Relative>(item);
        }

        internal override object Mapping(Relative item)
        {
            return _mapper.Map<RelativeDb>(item);
        }
    }
}
