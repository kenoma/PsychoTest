using MongoDB.Bson;
using Psycho.Common.Domain.Questionnaire;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Psycho.Common.Domain
{
    public class QuestionnaireEntryAnswer
    {
        /// <summary>
        ///     Уникальный идентификатор для ответа в пределах всего теста
        /// </summary>
        [XmlAttribute]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public int Score { get; set; }

        /// <summary>
        ///     Answer text
        /// </summary>
        public string TextAnswer { get; set; }

        /// <summary>
        ///     BASE64 Image
        /// </summary>
        public string ImageAnswer { get; set; }

        /// <summary>
        ///     Информация о связи данного ответа с результатами тестирования
        /// </summary>
        public List<QuestionnaireMapping> Mappings { get; set; } = new List<QuestionnaireMapping>();
    }
}