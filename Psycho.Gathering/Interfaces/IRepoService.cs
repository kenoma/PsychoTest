using Psycho.Gathering.Models;
using Psycho.Gathering.Models.Repo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Interfaces
{
    [ServiceContract]
    public interface IRepoService
    {
        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/ListProfiles?token={token}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Возвращает список всех профилей с указанием времени сбора")]
        UserGetMetaDTO[] ListProfiles(string token);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/GetUserData?token={token}&id={id}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Возвращает собранный профиль пользователя по его идентификатору вконтакте")]
        UserGet[] GetUserData(string token, int id);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/RangeSelect?token={token}&skip={skip}&take={take}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Выдает список профилей в указанном диапазоне")]
        UserGet[] RangeSelect(string token, int skip, int take);

        [OperationContract(IsOneWay = false),
        WebInvoke(
            Method = "GET", 
            UriTemplate = "/RangeGroupSelect?token={token}&skip={skip}&take={take}", 
            BodyStyle = WebMessageBodyStyle.Bare, 
            ResponseFormat = WebMessageFormat.Json, 
            RequestFormat = WebMessageFormat.Json), 
            Description("Выдает список данных по группам в указанном диапазоне")]
        WallResponse[] RangeGroupSelect(string token, int skip, int take);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/RangeRawSelect?token={token}&skip={skip}&take={take}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Выдает список профилей в указанном диапазоне")]
        byte[][] RangeRawSelect(string token, int skip, int take);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/InitiateGathering?token={token}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Для внутреннего использования")]
        bool InitiateGathering(string token);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/InitiateGatheringGroupWalls?token={token}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Для внутреннего использования")]
        bool InitiateGatheringGroupWalls(string token);


    }
}
