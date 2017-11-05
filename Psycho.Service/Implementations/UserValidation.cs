using Serilog;
using Psycho.Common.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Service.Implementations
{
    class UserValidation : IUserValidation
    {
        private HashSet<string> _tokens = new HashSet<string>();
        private IServiceUserRepository _serviceUserRepository;

        public UserValidation(IServiceUserRepository serviceUserRepository, ILogger log)
        {
            _serviceUserRepository = serviceUserRepository;
            //log.Info("Developer token inserted.");
            _tokens.Add("ifihadaheart");
        }

        public string Auth(string login, string password)
        {
            if (_serviceUserRepository.CheckUser(login, password))
            {
                var nToken = Path.GetRandomFileName();
                _tokens.Add(nToken);
                return nToken;
            }

            throw new UnauthorizedAccessException();
        }

        public bool ValidateToken(string token)
        {
            return _tokens.Contains(token);
        }
    }
}
