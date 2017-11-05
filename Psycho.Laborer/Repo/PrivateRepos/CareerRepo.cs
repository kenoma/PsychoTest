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
    class CareerRepo : Repository<Career>
    {
        static private IMapper _mapper;

        static CareerRepo()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Career, CareerDb>();
            });
            _mapper = config.CreateMapper();
        }

        public CareerRepo() 
            : base(nameof(Career))
        {

        }

        public void Add(IDbConnection cn, int userGetId, Career career)
        {
            var parameters = _mapper.Map<CareerDb>(career);
            parameters.UserGetId = userGetId;
            cn.Execute(SqliteHelpers.GetInsertQuery(_tableName, parameters), parameters);
        }

        internal override object Mapping(Career item)
        {
            return _mapper.Map<CareerDb>(item);
        }

       

    }
}
