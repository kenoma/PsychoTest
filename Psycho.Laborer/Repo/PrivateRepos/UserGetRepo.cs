using AutoMapper;
using Psycho.Common.Repository.Local;
using Psycho.Gathering.Models;
using Psycho.Laborer.Repo.SpecialModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;

namespace Psycho.Laborer.Repo
{
    class UserGetRepo : Repository<UserGet>
    {
        static private IMapper _mapper;

        static UserGetRepo()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserGet, UserGetDb>()
                .ForMember(z => z.LastSeenPlatform, opt => opt.MapFrom(z => z.last_seen == null ? 0 : z.last_seen.platform))
                .ForMember(z => z.LastSeenTime, opt => opt.MapFrom(z => z.last_seen == null ? 0 : z.last_seen.time))
                .ForMember(z => z.PersonalInspiredBy, opt => opt.MapFrom(z => z.personal == null ? "" : z.personal.inspired_by))
                .ForMember(z => z.PersonalLifeMain, opt => opt.MapFrom(z => z.personal == null ? 0 : z.personal.life_main));
                cfg.CreateMap<UserGetDb, UserGet>()
                .AfterMap((x, y) =>
                {
                    y.last_seen = new LastSeen
                    {
                        time = x.LastSeenTime,
                        platform = x.LastSeenPlatform
                    };
                    y.city = new City { id = x.CityId ?? -1 };
                    y.country = new Country { id = x.CountryId ?? -1 };
                    y.occupation = new Occupation { id = x.OccupationId ?? -1 };
                    y.personal = new Personal
                    {
                        alcohol = x.PersonalAlcohol ?? 0,
                        inspired_by = x.PersonalInspiredBy,
                        life_main = x.PersonalLifeMain ?? 0,
                        political = x.PersonalPolitical ?? 0,
                        religion = x.PersonalReligion,
                        smoking = x.PersonalSmoking ?? 0
                    };
                });
            });
            _mapper = config.CreateMapper();
        }

        public UserGetRepo()
            : base(nameof(UserGet))
        {
        }

        public override UserGet FindById(IDbConnection cn, long id)
        {
            var tmp = cn.QuerySingleOrDefault<UserGetDb>($"SELECT * FROM {_tableName} WHERE Id=@Id", new { Id = id });
            return _mapper.Map<UserGet>(tmp);
        }

        internal override object Mapping(UserGet report)
        {
            return _mapper.Map<UserGetDb>(report);
        }

        internal bool IsUserExist(IDbConnection cn, int userGetId)
        {
            var val = cn.QuerySingle<int>($"SELECT EXISTS(SELECT 1 FROM {_tableName} WHERE Id=@Id LIMIT 1);", new { Id = userGetId });
            return val == 1;
        }
    }
}
