using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Psycho.Common.Domain
{
    /// <summary>
    ///  Выход теста, то, что показывается пользователю по завершению тестирования
    /// </summary>
    public class QuestionnaireOutcome
    {
        /// <summary>
        ///     Порядковый номер результата тестирования, по которому связываются варианты ответов
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }

        /// <summary>
        ///     Заголовок результата
        /// </summary>
        public string CaptionText { get; set; }

        /// <summary>
        ///     Описание результата
        /// </summary>
        public string DescriptionText { get; set; }

        /// <summary>
        ///     BASE64 image
        /// </summary>
        public string DescriptionImage { get; set; }

        /// <summary>
        ///     Вес, необходимый для активации выхода
        /// </summary>
        public double WeightMin{ get; set; }

        /// <summary>
        ///     Вес, необходимый для активации выхода
        /// </summary>
        public double WeightMax { get; set; }

        public override string ToString() => $"#{Index} {CaptionText} {WeightMin}-{WeightMax}";
    }
}