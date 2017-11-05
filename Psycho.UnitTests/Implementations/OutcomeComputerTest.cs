using Serilog;
using NUnit.Framework;
using Psycho.Common.Domain;
using Psycho.Common.Domain.UserData;
using Psycho.Service.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Psycho.UnitTests.Implementations
{
    public class OutcomeComputerTest
    {
        ILogger _log = NSubstitute.Substitute.For<ILogger>();

        [Test, Explicit]
        public void Plutchik()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "Плутчик.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] { 2 * 6 - 1, 2 * 11 - 1 }
                }, scope);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("Вытеснение", result[0].CaptionText);
            }
        }

        [Test, Explicit]
        public void Plutchik2()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "Плутчик.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] { 2 * 8 - 1, 2 * 10 - 1 }
                }, scope);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("Замещение", result[0].CaptionText);
            }
        }

        [Test, Explicit]
        public void Biofield1()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "biofield.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] {
                        1,
                        2 * 2 - 1,
                        2 * 4 - 1,
                        2 * 6 - 1,
                        2 * 7 - 1,
                        2 * 9 - 1,
                        2 * 10 - 1,
                        2 * 12 - 1,
                        2 * 14 - 1,
                        2 * 16 - 1,
                        2 * 17 - 1,
                        2 * 21 - 1 }
                }, scope);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("9-15. Совместная работа", result[0].CaptionText);
            }
        }

        [Test, Explicit]
        public void Biofield2()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "biofield.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] {
                        1,
                        2 * 2 - 1,
                        2 * 4 - 1,
                        2 * 6 - 1,
                        2 * 7 - 1,
                        2 * 9 - 1
                    }
                }, scope);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("0-8. Время изменений", result[0].CaptionText);
            }
        }

        [Test, Explicit]
        public void Buisnessman1()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "business.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] {
                        1,
                        2 * 2 - 1,
                        2 * 4 - 1,
                        2 * 6 - 1,
                        2 * 7 - 1,
                        2 * 9 - 1
                    }
                }, scope);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("Менее 13 баллов", result[0].CaptionText);
            }
        }

        [Test, Explicit]
        public void Buisnessman2()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "business.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] {
                        1,
                        2 * 2 - 1,
                        2 * 4 - 1,
                        2 * 6 - 1,
                        2 * 7 - 1,
                        2 * 9 - 1,
                        2 * 10 - 1,
                        2 * 11 - 1,
                        2 * 12 - 1,
                        2 * 13 - 1,
                        2 * 14 - 1,
                        2 * 15 - 1,
                        2 * 16 - 1,
                    }
                }, scope);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("13-17 баллов", result[0].CaptionText);
            }
        }

        [Test, Explicit]
        public void Fifth1()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "Пятерка.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] {
                        1, 27, 53 , 78
                    }
                }, scope);

                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("Нейротизм", result[0].CaptionText);
                Assert.AreEqual("Тревожность", result[1].CaptionText);
            }
        }

        [Test, Explicit]
        public void Fifth2()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "Пятерка.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] {
                        1, 27, 53 , 78, 73
                    }
                }, scope);

                Assert.AreEqual(4, result.Count);
                Assert.AreEqual("Нейротизм", result[0].CaptionText);
                Assert.AreEqual("Тревожность", result[1].CaptionText);
            }
        }

        [Test, Explicit]
        public void Schwartz1()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "schwartz.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] { 5, 91 }
                }, scope);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("Самостоятельность", result[0].CaptionText);
            }
        }

        [Test, Explicit]
        public void Sex1()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "sexFemale.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] { 177 }
                }, scope);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("Темперамент", result[0].CaptionText);
            }
        }

        [Test, Explicit]
        public void Sex_ShouldBe2Outcomes()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var computer = new OutcomeComputer(_log);
            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "sexFemale.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                var result = computer.ComputeOutcomes(new QuestionnaireChoices
                {
                    ScopeId = scope.Id,
                    AnswerIds = new int[] { 2, 8, 13, 21, 26, 33, 38, 44, 50, 56, 62, 68, 74, 80, 86, 92, 98, 104, 110, 116, 123, 128, 134, 140, 146, 152, 158, 163, 168, 175 }
                }, scope);

                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("Темперамент", result[0].CaptionText);
            }
        }

        //[Test, Explicit]
        public void ConvertToExternalImages()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            var serscopes = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes"));

            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img")))
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img"));

            foreach (var file in serscopes)
            {
                using (var reader = new StreamReader(file))
                {
                    var scope = (QuestionnaireScope)serializer.Deserialize(reader);
                    scope.BannerImage = ProcessField(scope.BannerImage);
                    scope.ThumbnailImage = ProcessField(scope.ThumbnailImage);
                    foreach (var q in scope.Entries)
                    {
                        q.QuestionImage = ProcessField(q.QuestionImage);
                        foreach (var answ in q.Answers)
                        {
                            answ.ImageAnswer = ProcessField(answ.ImageAnswer);
                        }
                    }
                    foreach (var o in scope.Outcomes)
                    {
                        o.DescriptionImage = ProcessField(o.DescriptionImage);
                    }

                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            serializer.Serialize(writer, scope);
                            File.WriteAllText(file + "_", sww.ToString());
                        }
                    }
                }
            }
        }

        public string ProcessField(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            var subs = $"{Path.GetRandomFileName().Replace(".", "")}.jpg";
            var blank = $"http://psytest.online/images/{subs}";
            var data = Convert.FromBase64String(input.Replace("data:image/jpeg;base64,", "").Replace("data:image/png;base64,", ""));
            File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", subs), data);
            return blank;
        }

        //[Test, Explicit]
        public void SexCorrect()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));

            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "sexFemale.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                foreach (var entry in scope.Entries)
                {
                    foreach (var answ in entry.Answers)
                    {
                        var ver = answ.Mappings.Where(z => z.OutcomeIndex != 1);
                        if (ver.Any())
                            answ.Mappings.Add(new Common.Domain.Questionnaire.QuestionnaireMapping { OutcomeIndex = 11, Weight = ver.Count() });
                    }
                }

                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        serializer.Serialize(writer, scope);
                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "sexFemale.xml_"), sww.ToString());
                    }
                }
            }
        }

       // [Test, Explicit]
        public void SexCorrect2()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));

            using (var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "sexMale.xml")))
            {
                var scope = (QuestionnaireScope)serializer.Deserialize(reader);

                foreach (var entry in scope.Entries)
                {
                    foreach (var answ in entry.Answers)
                    {
                        var ver = answ.Mappings.Where(z => z.OutcomeIndex != 1);
                        if (ver.Any())
                            answ.Mappings.Add(new Common.Domain.Questionnaire.QuestionnaireMapping { OutcomeIndex = 11, Weight = ver.Count() });
                    }
                }

                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        serializer.Serialize(writer, scope);
                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes", "sexMale.xml_"), sww.ToString());
                    }
                }
            }
        }
    }
}
