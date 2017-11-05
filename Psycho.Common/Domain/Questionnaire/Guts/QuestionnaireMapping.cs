using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Psycho.Common.Domain.Questionnaire
{
    /// <summary>
    ///  Класс содержит информацию о том, какой вклад вносит в конкетный исход теста ассоциированный ответ
    /// </summary>
    public class QuestionnaireMapping
    {
        /// <summary>
        ///     Индекс результата теста, вклад в который вносит данная привязка
        /// </summary>
        [XmlAttribute]
        public int OutcomeIndex { get; set; }

        /// <summary>
        ///     Вес привязки
        /// </summary>
        [XmlAttribute]
        public double Weight { get; set; }
    }
}
