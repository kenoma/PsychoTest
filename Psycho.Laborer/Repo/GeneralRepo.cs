using Psycho.Gathering.Models;
using Psycho.Laborer.Repo.SpecialModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo
{
    class GeneralRepo
    {
        private CareerRepo _careerRepo;
        private CityRepo _cityRepo;
        private CountryRepo _countryRepo;
        private FriendsFollowersSubscriptionsRepo _friendsFollowersSubscriptionsRepo;
        private MilitaryRepo _militaryRepo;
        private OccupationRepo _occupationRepo;
        private RelativeRepo _relativeRepo;
        private SchoolRepo _schoolRepo;
        private UniversityRepo _universityRepo;
        private UserGetRepo _userGetRepo;
        private UserGroupsRepo _userGroupsRepo;
        private UserSchoolRepo _userSchoolRepo;
        private UserUniversityRepo _userUniversityRepo;
        private string _connectionString;

        public GeneralRepo(string connectionString)
        {
            _connectionString = connectionString;
            _careerRepo = new CareerRepo();
            _cityRepo = new CityRepo();
            _countryRepo = new CountryRepo();
            _friendsFollowersSubscriptionsRepo = new FriendsFollowersSubscriptionsRepo();
            _militaryRepo = new MilitaryRepo();
            _occupationRepo = new OccupationRepo();
            _relativeRepo = new RelativeRepo();
            _schoolRepo = new SchoolRepo();
            _universityRepo = new UniversityRepo();
            _userGetRepo = new UserGetRepo();
            _userGroupsRepo = new UserGroupsRepo();
            _userSchoolRepo = new UserSchoolRepo();
            _userUniversityRepo = new UserUniversityRepo();
        }

        public void Save(UserGet user)
        {
            if (user == null)
                return;

            using (IDbConnection cn = new SQLiteConnection(_connectionString))
            {
                cn.Open();
                using (var tran = cn.BeginTransaction())
                {
                    if (user.occupation != null)
                        _occupationRepo.Add(cn, user.occupation);

                    if (user.city != null)
                        _cityRepo.Add(cn, user.city);

                    if (user.country != null)
                        _countryRepo.Add(cn, user.country);

                    if (user.occupation != null)
                        _occupationRepo.Add(cn, user.occupation);

                    _userGetRepo.Add(cn, user);

                    if (user.military?.Any() ?? false)
                        foreach (var military in user.military)
                        {
                            _countryRepo.Add(cn, new Country { id = military.country_id });
                            _militaryRepo.Add(cn, military);
                        }

                    if (user.universities?.Any() ?? false)
                        foreach (var uni in user.universities)
                        {
                            _countryRepo.Add(cn, new Country { id = uni.country });
                            _cityRepo.Add(cn, new City { id = uni.city });
                            _universityRepo.Add(cn, uni);
                            _userUniversityRepo.Add(cn, new UserUniversity { UniversityId = uni.id, UserGetId = user.id });
                        }

                    if (user.schools?.Any() ?? false)
                        foreach (var school in user.schools)
                        {
                            _countryRepo.Add(cn, new Country { id = school.country });
                            _cityRepo.Add(cn, new City { id = school.city });
                            _schoolRepo.Add(cn, school);
                            _userSchoolRepo.Add(cn, new UserSchool { SchoolId = school.id, UserGetId = user.id });
                        }
                    if (user.Friends?.Any() ?? false)
                        foreach (var s in user.Friends)
                        {
                            _friendsFollowersSubscriptionsRepo.Add(cn, new FriendsFollowersSubscriptions { UserGetId = user.id, SubjectId = s.id, RelationsType = FFSType.Friend });
                        }

                    if (user.Followers?.Any() ?? false)
                        foreach (var s in user.Followers)
                        {
                            _friendsFollowersSubscriptionsRepo.Add(cn, new FriendsFollowersSubscriptions { UserGetId = user.id, SubjectId = s.id, RelationsType = FFSType.Follower });
                        }

                    if (user.Subscriptions?.Any() ?? false)
                        foreach (var s in user.Subscriptions)
                        {
                            _friendsFollowersSubscriptionsRepo.Add(cn, new FriendsFollowersSubscriptions { UserGetId = user.id, SubjectId = s.id, RelationsType = FFSType.Subscriber });
                        }

                    if (user.Groups?.Any() ?? false)
                        foreach (var g in user.Groups)
                        {
                            _userGroupsRepo.Add(cn, new UserGroups { UserGetId = user.id, GroupId = g.id });
                        }

                    if (user.relatives?.Any() ?? false)
                        foreach (var rel in user.relatives)
                        {
                            _relativeRepo.Add(cn, user.id, rel);
                        }

                    if (user.career?.Any() ?? false)
                        foreach (var career in user.career)
                        {
                            _cityRepo.Add(cn, new City { id = career.city_id });
                            _countryRepo.Add(cn, new Country { id = career.country_id });
                            _careerRepo.Add(cn, user.id, career);
                        }

                    tran.Commit();
                }
            }
        }

        internal UserGet FindById(int userGetid)
        {
            throw new NotImplementedException();
        }
    }
}
