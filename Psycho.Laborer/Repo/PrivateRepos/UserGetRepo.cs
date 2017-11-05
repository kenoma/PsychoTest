using Psycho.Common.Repository.Local;
using Psycho.Gathering.Models;
using Psycho.Laborer.Repo.SpecialModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo
{
    class UserGetRepo : Repository<UserGet>
    {
        public UserGetRepo()
            : base(nameof(UserGet))
        {
        }

        internal override object Mapping(UserGet report)
        {
            var expd = new UserGetDb()
            {
                id = report.id,
                first_name = report.first_name,
                last_name = report.last_name,
                sex = report.sex,
                nickname = report.nickname,
                maiden_name = report.maiden_name,
                domain = report.domain,
                screen_name = report.screen_name,
                photo_id = report.photo_id,
                has_photo = report.has_photo,
                has_mobile = report.has_mobile,
                friend_status = report.friend_status,
                online = report.online,
                wall_comments = report.wall_comments,
                can_post = report.can_post,
                can_see_all_posts = report.can_see_all_posts,
                can_see_audio = report.can_see_audio,
                can_write_private_message = report.can_write_private_message,
                can_send_friend_request = report.can_send_friend_request,
                site = report.site,
                status = report.status,
                LastSeenTime = report.last_seen?.time ?? 0,
                LastSeenPlatform = report.last_seen?.platform ?? 0,
                verified = report.verified,
                followers_count = report.followers_count,
                blacklisted = report.blacklisted,
                is_favorite = report.is_favorite,
                is_hidden_from_feed = report.is_hidden_from_feed,
                common_count = report.common_count,
                OccupationId = report.occupation?.id,
                CityId = report.city?.id,
                CountryId = report.country?.id,
                mobile_phone = report.mobile_phone,
                university = report.university,
                university_name = report.university_name,
                faculty = report.faculty,
                faculty_name = report.faculty_name,
                graduation = report.graduation,
                home_town = report.home_town,
                relation = report.relation,
                interests = report.interests,
                music = report.music,
                activities = report.activities,
                movies = report.movies,
                tv = report.tv,
                books = report.books,
                games = report.games,
                about = report.about,
                quotes = report.quotes,
                home_phone = report.home_phone,
                instagram = report.instagram,
                PersonalReligion = report.personal?.religion,
                PersonalInspiredBy = report.personal?.inspired_by,
                PersonalLifeMain = report.personal?.life_main,
                PersonalSmoking = report.personal?.smoking,
                PersonalAlcohol = report.personal?.alcohol,
                PersonalPolitical = report.personal?.political,
                bdate = report.bdate,
                skype = report.skype,
                twitter = report.twitter
            };
            return expd;
        }

       
    }
}
