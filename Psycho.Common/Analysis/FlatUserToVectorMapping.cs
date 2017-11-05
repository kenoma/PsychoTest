using AutoMapper;
using Psycho.Gathering.Models;
using Psycho.Validator.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Psycho.Validator.helpers
{
    public static class FlatUsertToVectorMapping
    {
        static FlatUsertToVectorMapping()
        {
            Mapper.Initialize(cfg =>
            {
                //cfg.CreateMap<GroupData, models.Group>();
                cfg.CreateMap<UserGet, FlatUser>()
                .ForMember(z => z.nickname, z => z.MapFrom(x => string.IsNullOrEmpty(x.nickname) ? 0 : 1))
                .ForMember(z => z.twitter, z => z.MapFrom(x => string.IsNullOrEmpty(x.twitter) ? 0 : 1))
                .ForMember(z => z.skype, z => z.MapFrom(x => string.IsNullOrEmpty(x.skype) ? 0 : 1))
                .ForMember(z => z.home_phone, z => z.MapFrom(x => string.IsNullOrEmpty(x.home_phone) ? 0 : 1))
                .ForMember(z => z.instagram, z => z.MapFrom(x => string.IsNullOrEmpty(x.instagram) ? 0 : 1))
                .ForMember(z => z.mobile_phone, z => z.MapFrom(x => string.IsNullOrEmpty(x.mobile_phone) ? 0 : 1))
                .ForMember(z => z.maiden_name, z => z.MapFrom(x => string.IsNullOrEmpty(x.maiden_name) ? 0 : 1))
                .ForMember(z => z.career, z => z.MapFrom(x => (x.career != null && x.career.Any()) ? 1 : 0))
                .ForMember(z => z.military, z => z.MapFrom(x => (x.military != null && x.military.Any()) ? 1 : 0))
                .ForMember(z => z.schools, z => z.MapFrom(x => (x.schools != null && x.schools.Any()) ? 1 : 0))
                .ForMember(z => z.universities, z => z.MapFrom(x => (x.universities != null && x.universities.Any()) ? 1 : 0))
                .ForMember(z => z.relatives, z => z.MapFrom(x => (x.relatives != null && x.relatives.Any()) ? 1 : 0))
                .ForMember(z => z.IsSimpleDomain, z => z.MapFrom(x => string.IsNullOrEmpty(x.domain) ? false : x.domain.Contains(x.id.ToString())))
                .ForMember(z => z.age, z => z.MapFrom(x => Age(x.bdate)))
                .ForMember(z => z.WallComments, z => z.MapFrom(x => x.wall_comments))
                .ForMember(z => z.LastSeenPlatform, z => z.MapFrom(x => x.last_seen == null ? -1 : x.last_seen.platform))
                .ForMember(z => z.WallPostCount, z => z.MapFrom(x => x.WallPosts == null ? -1 : x.WallPosts.Count))
                .ForMember(z => z.GroupsCount, z => z.MapFrom(x => x.Groups == null ? -1 : x.Groups.Count))
                .ForMember(z => z.WallGroupsCount, z => z.MapFrom(x => x.wallGroups == null ? -1 : x.wallGroups.Count))
                .ForMember(z => z.FriendsCount, z => z.MapFrom(x => x.Friends == null ? -1 : x.Friends.Count))
                .ForMember(z => z.FollowersCount, z => z.MapFrom(x => x.Followers == null ? x.followers_count : x.Followers.Count))
                .ForMember(z => z.ProfilesCount, z => z.MapFrom(x => x.profiles == null ? -1 : x.profiles.Count))
                .ForMember(z => z.SubscriptionsCount, z => z.MapFrom(x => x.Subscriptions == null ? -1 : x.Subscriptions.Count));

            });
        }

        public static string[] GetHeader(int groups, int text)
        {
            var retval = new List<string>();
            retval.AddRange(OrdinaryHeader(new FlatUser()));
            retval.AddRange(OrdinaryHeader(new FlatUser()).Select(z=>$"friends_{z}"));
            retval.AddRange(OrdinaryHeader(new FlatUser()).Select(z => $"followers_{z}"));
            retval.AddRange(OrdinaryHeader(new FlatUser()).Select(z => $"subs_{z}"));
            retval.AddRange(Enumerable.Range(0, groups).Select(z => $"G{z}"));
            retval.AddRange(Enumerable.Range(0, text).Select(z => $"T{z}"));
            return retval.ToArray();
        }

        public static float[] ToVector(this UserGet userGet)
        {
            var retval = new List<float>();
            var user = Mapper.Map<FlatUser>(userGet);
            float[] ord = OrdinaryValues(user);
            retval.AddRange(ord);
            retval.AddRange(AverageSubling(userGet, userGet.Friends, ord.Length));
            retval.AddRange(AverageSubling(userGet, userGet.Followers, ord.Length));
            retval.AddRange(AverageSubling(userGet, userGet.Subscriptions, ord.Length));
            return retval.ToArray();
        }

        private static float[] AverageSubling(UserGet user, IEnumerable<UserGet> userFriends, int vlen)
        {
            var retval = new float[vlen];
            if (userFriends?.Count() > 0)
            {
                var ords = userFriends.Select(z => OrdinaryValues(Mapper.Map<FlatUser>(z))).ToArray();
                for (int pos = 0; pos < vlen; pos++)
                {
                    retval[pos] = ords.Select(z => z[pos]).Where(z => z != -1).DefaultIfEmpty(-1).Average();
                }
            }
            else
                for (int pos = 0; pos < vlen; pos++)
                    retval[pos] = -1;

            return retval;
        }

        public static float[] ToVector(this UserGet userGet,
           Dictionary<long, double[]> groupMapping)
        {
            var retval = new List<float>();
            var user = Mapper.Map<FlatUser>(userGet);
            float[] ord = OrdinaryValues(user);
            retval.AddRange(ord);
            retval.AddRange(AverageSubling(userGet, userGet.Friends, ord.Length));
            retval.AddRange(AverageSubling(userGet, userGet.Followers, ord.Length));
            retval.AddRange(AverageSubling(userGet, userGet.Subscriptions, ord.Length));

            var groupVector = GetGroupVector(userGet, groupMapping);
            retval.AddRange(groupVector);
            return retval.ToArray();
        }

        public static float[] ToVector(this UserGet userGet,
            Dictionary<long, double[]> groupMapping,
            Dictionary<int, HashSet<string>> bagOfTerms)
        {
            var retval = new List<float>();
            var user = Mapper.Map<FlatUser>(userGet);
            float[] ord = OrdinaryValues(user);
            retval.AddRange(ord);
            retval.AddRange(AverageSubling(userGet, userGet.Friends, ord.Length));
            retval.AddRange(AverageSubling(userGet, userGet.Followers, ord.Length));
            retval.AddRange(AverageSubling(userGet, userGet.Subscriptions, ord.Length));

            var groupVector = GetGroupVector(userGet, groupMapping);
            retval.AddRange(groupVector);

            retval.AddRange(StringValues(userGet, bagOfTerms));
            return retval.ToArray();
        }

        private static float[] GetGroupVector(UserGet userGet, Dictionary<long, double[]> groupMapping)
        {
            float[] groupVector = new float[groupMapping.First().Value.Length];
            var groups = new List<GroupData>();
            if (userGet.Groups != null)
                groups.AddRange(userGet.Groups);
            if (userGet.wallGroups != null)
                groups.AddRange(userGet.wallGroups);

            foreach (var group in groups)
                if (groupMapping.ContainsKey(group.id))
                {
                    var w = groupMapping[group.id];
                    for (int pos = 0; pos < groupVector.Length; pos++)
                    {
                        groupVector[pos] += (float)w[pos];// Math.Max((float)w[pos], groupVector[pos]);
                    }
                }
            var max = groupVector.Max() != 0 ? groupVector.Max() : 1f;
            return groupVector.Select(z => z / max).ToArray();
        }

        static private string[] OrdinaryHeader(FlatUser user)
        {
            var sb = new List<string>();

            foreach (var fieldInfo in typeof(FlatUser).GetProperties().OrderBy(z => z.Name))
            {
                if (!fieldInfo.Name.Equals("id") &&
                    !fieldInfo.Name.Equals("CityId") &&
                    !fieldInfo.PropertyType.IsArray &&
                    fieldInfo.PropertyType != typeof(string))
                    sb.Add(fieldInfo.Name);
            }

            return sb.ToArray();
        }


        static private float[] OrdinaryValues(FlatUser user)
        {
            var sb = new List<float>();

            foreach (var fieldInfo in typeof(FlatUser).GetProperties().OrderBy(z => z.Name))
            {
                if (!fieldInfo.Name.Equals("id") &&
                    !fieldInfo.Name.Equals("CityId") &&
                    !fieldInfo.PropertyType.IsArray &&
                    fieldInfo.PropertyType != typeof(string))
                    sb.Add(Convert.ToSingle(fieldInfo.GetValue(user)));
            }

            return sb.ToArray();
        }

        private static Regex _onlyWordsRegex = new Regex("[а-я]+", RegexOptions.Compiled);
        static private float[] StringValues(UserGet user, Dictionary<int, HashSet<string>> bagOfTerms)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var sb = new StringBuilder();

            foreach (var fieldInfo in typeof(UserGet).GetProperties().Where(z => z.PropertyType == typeof(string)))
            {
                sb.Append(fieldInfo.GetValue(user)?.ToString().ToLower() ?? "");
                sb.Append(" ");
            }

            if (user.Groups != null)
                foreach (var group in user.Groups)
                {
                    foreach (var fieldInfo in typeof(GroupData).GetProperties().Where(z => z.PropertyType == typeof(string)))
                    {
                        sb.Append(fieldInfo.GetValue(group)?.ToString().ToLower() ?? "");
                        sb.Append(" ");
                    }
                }

            if (user.wallGroups != null)
                foreach (var group in user.wallGroups)
                {
                    foreach (var fieldInfo in typeof(GroupData).GetProperties().Where(z => z.PropertyType == typeof(string)))
                    {
                        sb.Append(fieldInfo.GetValue(group)?.ToString().ToLower() ?? "");
                        sb.Append(" ");
                    }
                }
            var retval = new float[bagOfTerms.Max(z => z.Key) + 1];
            var textProfile = sb.ToString();
            var matches = _onlyWordsRegex.Matches(textProfile)
                .Cast<Match>()
                .Select(m => m.Value)
                .GroupBy(z => z)
                .ToDictionary(z => z.Key, z => z.Count());

            foreach (var topicDic in bagOfTerms)
            {
                var intersect = topicDic.Value.Intersect(matches.Select(z => z.Key));
                retval[topicDic.Key] = intersect.Select(z => matches[z]).Sum();
            }

            var max = retval.Max() != 0 ? retval.Max() : 1f;
            return retval.Select(z => z / max).ToArray();
        }

        private static Regex _bdayRegex = new Regex(@"\d+\.\d+\.(?<year>\d{4})");
        static public int Age(string bdate)
        {
            if (bdate != null)
            {
                var m = _bdayRegex.Match(bdate);
                if (m.Success)
                {
                    var year = int.Parse(m.Groups["year"].Value);
                    var age = DateTime.Now.Year - year;
                    return age;
                }
            }

            return -1;
        }
    }
}
