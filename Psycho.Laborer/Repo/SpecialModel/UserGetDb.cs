using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo.SpecialModel
{
    class UserGetDb
    {
        public UserGetDb()
        {
        }

        public int id { get; internal set; }
        public string first_name { get; internal set; }
        public string last_name { get; internal set; }
        public string domain { get; internal set; }
        public string nickname { get; internal set; }
        public string maiden_name { get; internal set; }
        public byte sex { get; internal set; }
        public string screen_name { get; internal set; }
        public string photo_id { get; internal set; }
        public byte has_photo { get; internal set; }
        public byte has_mobile { get; internal set; }
        public byte friend_status { get; internal set; }
        public byte online { get; internal set; }
        public int wall_comments { get; internal set; }
        public byte can_post { get; internal set; }
        public byte can_see_all_posts { get; internal set; }
        public byte can_see_audio { get; internal set; }
        public byte can_write_private_message { get; internal set; }
        public byte can_send_friend_request { get; internal set; }
        public string site { get; internal set; }
        public string status { get; internal set; }
        public byte verified { get; internal set; }
        public int followers_count { get; internal set; }
        public int blacklisted { get; internal set; }
        public byte is_favorite { get; internal set; }
        public byte is_hidden_from_feed { get; internal set; }
        public int common_count { get; internal set; }
        public string mobile_phone { get; internal set; }
        public int university { get; internal set; }
        public string university_name { get; internal set; }
        public int faculty { get; internal set; }
        public string faculty_name { get; internal set; }
        public int graduation { get; internal set; }
        public string home_town { get; internal set; }
        public int relation { get; internal set; }
        public string interests { get; internal set; }
        public string music { get; internal set; }
        public string activities { get; internal set; }
        public string movies { get; internal set; }
        public string tv { get; internal set; }
        public string books { get; internal set; }
        public string games { get; internal set; }
        public string about { get; internal set; }
        public string quotes { get; internal set; }
        public string home_phone { get; internal set; }
        public string instagram { get; internal set; }
        public int? PersonalSmoking { get; internal set; }
        public int? PersonalAlcohol { get; internal set; }
        public int? PersonalPolitical { get; internal set; }
        public string skype { get; internal set; }
        public string twitter { get; internal set; }
        public int LastSeenTime { get; internal set; }
        public int LastSeenPlatform { get; internal set; }
        public double? OccupationId { get; internal set; }
        public int? CityId { get; internal set; }
        public int? CountryId { get; internal set; }
        public string PersonalReligion { get; internal set; }
        public string PersonalInspiredBy { get; internal set; }
        public int? PersonalLifeMain { get; internal set; }
        public string bdate { get; internal set; }
    }
}
