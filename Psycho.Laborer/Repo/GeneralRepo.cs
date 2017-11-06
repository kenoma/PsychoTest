using Psycho.Gathering.Models;
using Psycho.Laborer.Repo.PrivateRepos;
using Psycho.Laborer.Repo.SpecialModel;
using Serilog;
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
        private UserGroupActivityRepo _userGroupActivityRepo;
        private string _connectionString;
        private WallPostRepo _wallPostRepo;

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
            _userGroupActivityRepo = new UserGroupActivityRepo();
            _wallPostRepo = new WallPostRepo();
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

                    if (user.WallPosts?.Any() ?? false)
                        foreach (var wallPost in user.WallPosts)
                            _wallPostRepo.Add(cn, user.id, wallPost);

                    if (user.military?.Any() ?? false)
                        foreach (var military in user.military)
                        {
                            _militaryRepo.Add(cn, user.id, military);
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
                            //_userGetRepo.Add(cn, s);
                        }

                    if (user.Followers?.Any() ?? false)
                        foreach (var s in user.Followers)
                        {
                            _friendsFollowersSubscriptionsRepo.Add(cn, new FriendsFollowersSubscriptions { UserGetId = user.id, SubjectId = s.id, RelationsType = FFSType.Follower });
                            //_userGetRepo.Add(cn, s);
                        }

                    if (user.Subscriptions?.Any() ?? false)
                        foreach (var s in user.Subscriptions)
                        {
                            _friendsFollowersSubscriptionsRepo.Add(cn, new FriendsFollowersSubscriptions { UserGetId = user.id, SubjectId = s.id, RelationsType = FFSType.Subscriber });
                            //_userGetRepo.Add(cn, s);
                        }

                    var groups = new HashSet<int>();

                    if (user.Groups?.Any() ?? false)
                        foreach (var g in user.Groups)
                            if (!groups.Contains(g.id))
                                groups.Add(g.id);

                    if (user.GroupIds?.Any() ?? false)
                        foreach (var g in user.GroupIds)
                            if (!groups.Contains(g))
                                groups.Add(g);

                    foreach (var g in groups)
                        _userGroupsRepo.Add(cn, new UserGroups { UserGetId = user.id, GroupId = g });

                    if (user.relatives?.Any() ?? false)
                        foreach (var rel in user.relatives)
                        {
                            _relativeRepo.Add(cn, user.id, rel);
                        }

                    if (user.career?.Any() ?? false)
                        foreach (var career in user.career)
                        {
                            _careerRepo.Add(cn, user.id, career);
                        }

                    tran.Commit();
                }
            }
        }

        internal UserGet FindById(int userGetid)
        {
            using (IDbConnection cn = new SQLiteConnection(_connectionString))
            {
                cn.Open();
                var user = _userGetRepo.FindById(cn, userGetid);
                if (user == null)
                    return user;

                user.career = new List<Career>(_careerRepo.FindByUserId(cn, userGetid));
                user.relatives = new List<Relative>(_relativeRepo.FindByUserId(cn, userGetid));
                user.military = new List<Military>(_militaryRepo.FindByUserId(cn, userGetid));

                var groupList = _userGroupsRepo.FindByUserId(cn, userGetid);
                user.GroupIds = new List<int>(groupList);

                var schoolList = _userSchoolRepo.FindByUserId(cn, userGetid);
                user.schools = new List<School>();
                foreach (var school in schoolList)
                    user.schools.Add(_schoolRepo.FindById(cn, school));

                var universityList = _userUniversityRepo.FindByUserId(cn, userGetid);
                user.universities = new List<University>();
                foreach (var un in universityList)
                    user.universities.Add(_universityRepo.FindById(cn, un));

                if (user.occupation != null)
                    user.occupation = _occupationRepo.FindById(cn, user.occupation.id);

                return user;
            }
        }

        public void StoreActivity(IEnumerable<UserGroupActivity> activities)
        {
            using (IDbConnection cn = new SQLiteConnection(_connectionString))
            {
                cn.Open();
                using (var tran = cn.BeginTransaction())
                {
                    foreach (var act in activities)
                    {
                        _userGroupActivityRepo.Add(cn, act);
                    }
                    tran.Commit();
                }
            }
        }

        internal bool IsUserExist(int userGetId)
        {
            try
            {
                using (IDbConnection cn = new SQLiteConnection(_connectionString))
                {
                    cn.Open();
                    return _userGetRepo.IsUserExist(cn, userGetId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }
    }
}
