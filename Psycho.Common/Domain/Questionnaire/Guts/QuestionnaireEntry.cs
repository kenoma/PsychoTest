using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Psycho.Common.Domain
{
    public class QuestionnaireEntry
    {
        /// <summary>
        ///     Порядковый номер вопроса в тесте
        /// </summary>
        [XmlAttribute]
        public int Number { get; set; }

        /// <summary>
        ///     Текст вопроса
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        ///     BASE64 image
        /// </summary>
        public string QuestionImage { get; set; }

        /// <summary>
        ///     
        /// </summary>
        public bool CanSelectMultipleAnswers { get; set; }

        /// <summary>
        ///     Ответы на заданный тест
        /// </summary>
        public List<QuestionnaireEntryAnswer> Answers { get; set; } = new List<QuestionnaireEntryAnswer>();
    }
}