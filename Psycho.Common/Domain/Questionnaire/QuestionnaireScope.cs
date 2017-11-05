using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Psycho.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Psycho.Common.Domain
{
    /// <summary>
    ///     Класс анкеты-опросника
    /// </summary>
    public class QuestionnaireScope : IAggregateRoot
    {
        [BsonId, XmlIgnore, JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _id = ObjectId.GenerateNewId();

        [XmlAttribute]
        public long Id { get; set; } = Environment.TickCount;

        /// <summary>
        ///     Название теста
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Поколение теста
        /// </summary>
        public int Generation { get; set; }

        /// <summary>
        ///     Аннотация опросника
        /// </summary>
        public string QuestionHeader { get; set; }

        /// <summary>
        ///     Аннотация опросника
        /// </summary>
        public string OutcomeHeader { get; set; }

        /// <summary>
        ///     Счетчик прохождений теста
        /// </summary>
        public int Passed { get; set; }

        /// <summary>
        ///     Аннотация опросника
        /// </summary>
        public string Annotation { get; set; }

        /// <summary>
        ///     Описание опросника
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///    BASE64 Thumbnail
        /// </summary>
        public string ThumbnailImage { get; set; }

        /// <summary>
        ///    BASE64 banner
        /// </summary>
        public string BannerImage { get; set; }

        public int OutcomeLimit { get; set; }

        public int Capacity { get; set; }

        /// <summary>
        ///     Вопросы
        /// </summary>
        public List<QuestionnaireEntry> Entries { get; set; } = new List<QuestionnaireEntry>();

        /// <summary>
        ///     Результаты тестов
        /// </summary>
        public List<QuestionnaireOutcome> Outcomes { get; set; } = new List<QuestionnaireOutcome>();

        public override string ToString() => $"#{Id} {Name}";

    }
}
