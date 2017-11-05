using System.Collections.Generic;

namespace Psycho.Common.Domain.UserData
{
    /// <summary>
    ///     Класс данных о пользователе вконтакте
    /// </summary>
    public class SocialNetworkDataVkontakte
    {
        public int UserId { get; set; }

        public string UsersGet { get; set; }

        public string RequestOne { get; set; }
        public string RequestTwo { get; set; }
        public string[] WallComments { get; set; }
        public string PhotoWall { get; set; }
        public string PhotoProfile { get; set; }
        public string PhotoSaved { get; set; }
        public string NotesGetComments { get; set; }
    }
}