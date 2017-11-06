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

namespace Psycho.Laborer.Repo.PrivateRepos
{
    class WallPostRepo : Repository<WallPost>
    {
        static private IMapper _mapper;

        static WallPostRepo()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WallPost, WallPostDb>()
                .ForMember(z=>z.post_text,opt=>opt.MapFrom(z=>z.text));
            });
            _mapper = config.CreateMapper();
        }

        public WallPostRepo()
            : base(nameof(WallPost))
        {

        }

        public void Add(IDbConnection cn, int userGetId, WallPost wallPost)
        {
            var parameters = _mapper.Map<WallPostDb>(wallPost);
            parameters.UserGetId = userGetId;
            cn.Execute(SqliteHelpers.GetInsertQuery(_tableName, parameters, "id"), parameters);
        }

        internal override object Mapping(WallPost item)
        {
            return _mapper.Map<WallPostDb>(item);
        }
    }
}
