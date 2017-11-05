using Facebook;
using Serilog;
using Psycho.Common.Domain.UserData;
using Psycho.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Service.Implementations
{
    class GrabberFB : ISocialNetworkGrabber<SocialNetworkDataFacebook>
    {
        private ILogger _log;

        public GrabberFB(ILogger log)
        {
            _log = log;
        }

        public SocialNetworkDataFacebook GetUserData(string access_token)
        {
            _log.Information($"Retrieving fb data with token {access_token}");
            SocialNetworkDataFacebook fbData = new SocialNetworkDataFacebook();

            var _fbClient = new FacebookClient { AccessToken = access_token };
            _fbClient.AppId = config.Default.fb_appid;
            _fbClient.AppSecret = config.Default.fb_appsecret;
            
            fbData.fbMe = FbRequest(_fbClient,"me?fields=picture,id,about,age_range,birthday,currency,cover,education,email,favorite_athletes,favorite_teams,first_name,gender,hometown,inspirational_people,interested_in,languages,last_name,link,locale,location,meeting_for,middle_name,political,quotes,relationship_status,religion,sports,website,work,likes,accounts");
            fbData.fbPosts = FbRequest(_fbClient,"me/posts?limit=500");
            fbData.fbTagged = FbRequest(_fbClient,"me/tagged?limit=500");
            fbData.fbAccounts = FbRequest(_fbClient,"me/Accounts?limit=500");
            fbData.fbAchievements = FbRequest(_fbClient,"me/Achievements?limit=500");
            fbData.fbAlbums = FbRequest(_fbClient,"me/Albums?limit=500");
            fbData.fbApprequestformerrecipients = FbRequest(_fbClient,"me/Apprequestformerrecipients?limit=500");
            fbData.fbApprequests = FbRequest(_fbClient,"me/Apprequests?limit=500");
            fbData.fbBooks = FbRequest(_fbClient,"me/Books?limit=500");
            //fbData.fbBusiness_Activities = FbRequest(_fbClient,"me/Business_Activities?limit=500");
            fbData.fbConversations = FbRequest(_fbClient,"me/Conversations?limit=500");
            fbData.fbCurated_Collections = FbRequest(_fbClient,"me/Curated_Collections?limit=500");
            fbData.fbDomains = FbRequest(_fbClient,"me/Domains?limit=500");
            fbData.fbEvents = FbRequest(_fbClient,"me/Events?limit=500");
            fbData.fbFamily = FbRequest(_fbClient,"me/Family?limit=500");
            fbData.fbFavorite_Requests = FbRequest(_fbClient,"me/Favorite_Requests?limit=500");
            fbData.fbFeed = FbRequest(_fbClient,"me/Feed?limit=500");
            fbData.fbFriendlists = FbRequest(_fbClient,"me/Friendlists?limit=500");
            fbData.fbFriends = FbRequest(_fbClient,"me/Friends?limit=500");
            fbData.fbGames = FbRequest(_fbClient,"me/Games?limit=500");
            fbData.fbGroups = FbRequest(_fbClient,"me/Groups?limit=500");
            fbData.fbhome = FbRequest(_fbClient,"me/home?limit=500");
            fbData.fbinbox = FbRequest(_fbClient,"me/inbox?limit=500");
            fbData.fbinvitable_friends = FbRequest(_fbClient,"me/invitable_friends?limit=500");
            fbData.fbLikes = FbRequest(_fbClient,"me/Likes?limit=500");
            fbData.fbMovies = FbRequest(_fbClient,"me/Movies?limit=500");
            fbData.fbMusic = FbRequest(_fbClient,"me/Music?limit=500");
            fbData.fbNotifications = FbRequest(_fbClient,"me/Notifications?limit=500");
            fbData.fboutbox = FbRequest(_fbClient,"me/outbox?limit=500");
            fbData.fbPermissions = FbRequest(_fbClient,"me/Permissions?limit=500");
            fbData.fbPhotos = FbRequest(_fbClient,"me/Photos?limit=500");
            fbData.fbPicture = FbRequest(_fbClient,"me/Picture?limit=500");
            fbData.fbRequest_History = FbRequest(_fbClient,"me/Request_History?limit=500");
            fbData.fbrich_media_documents = FbRequest(_fbClient,"me/rich_media_documents?limit=500");
            fbData.fbSession_Keys = FbRequest(_fbClient,"me/Session_Keys?limit=500");
            fbData.fbStream_Filters = FbRequest(_fbClient,"me/Stream_Filters?limit=500");
            fbData.fbTaggable_Friends = FbRequest(_fbClient,"me/Taggable_Friends?limit=500");
            fbData.fbTagged_Places = FbRequest(_fbClient,"me/Tagged_Places?limit=500");
            fbData.fbTelevision = FbRequest(_fbClient,"me/Television?limit=500");
            fbData.fbVideo_Broadcasts = FbRequest(_fbClient,"me/Video_Broadcasts?limit=500");
            fbData.fbVideos = FbRequest(_fbClient,"me/Videos?limit=500");
            return fbData;
        }

        private string FbRequest(FacebookClient fbClient, string url)
        {
            try
            {
                return fbClient.Get(url).ToString();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return "";
            }
        }
    }
}
