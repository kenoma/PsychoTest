using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Service
{
    public class DTOConciseQuestionnaireScope
    {
        [BsonId]
        private ObjectId _id = ObjectId.GenerateNewId();

        public long Id { get; set; }

        /// <summary>
        ///     Название теста
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Описание опросника
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Краткая аннотация теста
        /// </summary>
        public string Annotation { get; set; }

        /// <summary>
        ///    BASE64 Thumbnail
        /// </summary>
        public string ThumbnailImage { get; set; }

        /// <summary>
        ///     Счетчик прохождений теста
        /// </summary>
        public int Passed { get; set; }

        /// <summary>
        ///     Поколение теста
        /// </summary>
        public int Generation { get; set; }
    }
}
