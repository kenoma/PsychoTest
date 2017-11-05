using Psycho.Gathering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Validator.models
{
    
    public class StatusAudio
    {
        public int id { get; set; }
        public int owner_id { get; set; }
        public string artist { get; set; }
        public string title { get; set; }
        public int duration { get; set; }
        public int date { get; set; }
        public string url { get; set; }
        public int genre_id { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int sex { get; set; }
        public string nickname { get; set; }
        public string maiden_name { get; set; }
        public string domain { get; set; }
        public string screen_name { get; set; }
        public City city { get; set; }
        public Country country { get; set; }
        public string photo_50 { get; set; }
        public string photo_100 { get; set; }
        public string photo_200 { get; set; }
        public string photo_max { get; set; }
        public string photo_200_orig { get; set; }
        public string photo_400_orig { get; set; }
        public string photo_max_orig { get; set; }
        public string photo_id { get; set; }
        public int has_photo { get; set; }
        public int has_mobile { get; set; }
        public int is_friend { get; set; }
        public int friend_status { get; set; }
        public int online { get; set; }
        public string online_app { get; set; }
        public int online_mobile { get; set; }
        public int wall_comments { get; set; }
        public int can_post { get; set; }
        public int can_see_all_posts { get; set; }
        public int can_see_audio { get; set; }
        public int can_write_private_message { get; set; }
        public int can_send_friend_request { get; set; }
        public string home_phone { get; set; }
        public string site { get; set; }
        public string status { get; set; }
        public LastSeen last_seen { get; set; }
        public int verified { get; set; }
        public int followers_count { get; set; }
        public int blacklisted { get; set; }
        public int blacklisted_by_me { get; set; }
        public int is_favorite { get; set; }
        public int is_hidden_from_feed { get; set; }
        public int common_count { get; set; }
        public string bdate { get; set; }
        public List<object> career { get; set; }
        public int? university { get; set; }
        public string university_name { get; set; }
        public int? faculty { get; set; }
        public string faculty_name { get; set; }
        public int? graduation { get; set; }
        public string home_town { get; set; }
        public int? relation { get; set; }
        public string interests { get; set; }
        public string music { get; set; }
        public string activities { get; set; }
        public string movies { get; set; }
        public string tv { get; set; }
        public string books { get; set; }
        public string games { get; set; }
        public List<object> universities { get; set; }
        public List<object> schools { get; set; }
        public string about { get; set; }
        public List<object> relatives { get; set; }
        public string quotes { get; set; }
        public Occupation occupation { get; set; }
        public string deactivated { get; set; }
        public string instagram { get; set; }
        public Personal personal { get; set; }
        public string twitter { get; set; }
        public StatusAudio status_audio { get; set; }
        public string skype { get; set; }
        public string facebook { get; set; }
        public string facebook_name { get; set; }
    }

    public class Req2Followers
    {
        public int count { get; set; }
        public List<UserGet> items { get; set; }
    }

    public class Document
    {
        public string title { get; set; }
    }

    public class Req2Docs
    {
        public int count { get; set; }
        public List<Document> items { get; set; }
    }

    public class Req2FriendsGet
    {
        public int count { get; set; }
        public List<UserGet> items { get; set; }
    }

    public class Req2Groups
    {
        public int count { get; set; }
        public List<GroupData> items { get; set; }
    }
    
    public class Likes
    {
        public int user_likes { get; set; }
        public int count { get; set; }
    }

    public class Reposts
    {
        public int count { get; set; }
    }

    public class Comments
    {
        public int count { get; set; }
    }

    public class Tags
    {
        public int count { get; set; }
    }

    public class ResponseReqTwo
    {
        public Req2Followers req_0 { get; set; }
        //public Req2Docs req_2 { get; set; }
        public Req2FriendsGet req_3 { get; set; }
        public Req2Groups req_5 { get; set; }
    }

    public class RootObjectTwo
    {
        public ResponseReqTwo response { get; set; }
    }
}
