using Dapper;
using Psycho.Gathering.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Implementations
{
    public class NLPResultsRepository : SqLiteBaseRepository, INLPResultsRepository
    {
        public NLPResultsRepository(string dbFile) 
            : base(dbFile)
        {
            if (!File.Exists(DbFile))
            {
                throw new Exception("Should be called only on existing repo");
            }
            CreateTable();
        }

        private void CreateTable()
        {
            using (var cnn = DbConnection())
            {
                cnn.Open();
               cnn.Execute(
                    @"CREATE TABLE IF NOT EXISTS nlp
                      (
                         Id                                  integer primary key AUTOINCREMENT,
                         VkontakteUserId                     integer not null,
                         ремонт FLOAT,
бег_и_ходьба FLOAT,
литература FLOAT,
дача_огород FLOAT,
финансовые_услуги FLOAT,
автомото FLOAT,
пробки FLOAT,
фото FLOAT,
недвижимость FLOAT,
украина FLOAT,
кулинария FLOAT,
здравоохранение FLOAT,
личные_праздники FLOAT,
путешествия_туризм FLOAT,
собянин FLOAT,
музыка FLOAT,
рыбалка_охота FLOAT,
нг_23_февр_8_марта_1_мая FLOAT,
гороскоп FLOAT,
домашние_животные FLOAT,
оружие FLOAT,
лыжи_сноуборд FLOAT,
юмор FLOAT,
дизайн_интерьер FLOAT,
театр_искусство_музей FLOAT,
философия_эзотерика FLOAT,
йога FLOAT,
христианство FLOAT,
космос FLOAT,
православные_праздники FLOAT,
психология FLOAT,
велосипед FLOAT,
война FLOAT,
субкультуры FLOAT,
кино FLOAT,
буддизм FLOAT,
универ FLOAT,
конкурсы_и_тесты FLOAT,
мистика FLOAT,
девятое_мая_12_июня FLOAT,
дошкольное FLOAT,
иудаизм FLOAT,
диета_и_питание FLOAT,
благотворительность FLOAT,
лгбт FLOAT,
ислам FLOAT,
петиции FLOAT,
татуировки FLOAT,
страхование FLOAT,
школа FLOAT,
тусовка_рестораны FLOAT,
телевидение FLOAT,
бизнес FLOAT,
реновация FLOAT,
мысли_цитаты FLOAT,
мода FLOAT,
красота_и_уход FLOAT,
ностальгия FLOAT);");
            }
        }

        public void CleanAll()
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                cn.Query(@"DROP TABLE IF EXISTS nlp");
            }
            CreateTable();
        }
        
        public void Insert(int[] vkIds, float[][] aresults)
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    for (int pos = 0; pos < vkIds.Length; pos++)
                    {
                        var dbArgs = new DynamicParameters();
                        for (int r = 1; r <= 58; r++)
                            dbArgs.Add($"R{r}", aresults[pos][r-1]);
                        
                        dbArgs.Add("Vk", vkIds[pos]);
                        cn.Query($@"INSERT INTO nlp (VkontakteUserId, ремонт,бег_и_ходьба,литература,дача_огород,финансовые_услуги,автомото,пробки,фото,недвижимость,украина,кулинария,здравоохранение,личные_праздники,путешествия_туризм,собянин,музыка,рыбалка_охота,нг_23_февр_8_марта_1_мая,гороскоп,домашние_животные,оружие,лыжи_сноуборд,юмор,дизайн_интерьер,театр_искусство_музей,философия_эзотерика,йога,христианство,космос,православные_праздники,психология,велосипед,война,субкультуры,кино,буддизм,универ,конкурсы_и_тесты,мистика,девятое_мая_12_июня,дошкольное,иудаизм,диета_и_питание,благотворительность,лгбт,ислам,петиции,татуировки,страхование,школа,тусовка_рестораны,телевидение,бизнес,реновация,мысли_цитаты,мода,красота_и_уход,ностальгия) 
                                          VALUES (@Vk, {string.Join(", ", Enumerable.Range(1, 58).Select(z => $"@R{z}"))});", dbArgs);
                    }
                    trans.Commit();
                }
            }
        }
    }
}
