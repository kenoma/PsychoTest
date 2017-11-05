using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Implementations
{
    public class SqLiteBaseRepository
    {
        private string _dbFile;

        public SqLiteBaseRepository(string dbFile)
        {
            _dbFile = dbFile;
        }

        public string DbFile
        {
            get { return _dbFile; }
        }

        public SQLiteConnection DbConnection()
        {
            return new SQLiteConnection($"Data Source={DbFile};Compress=True;DateTimeFormat=Ticks;UTF8Encoding=True;FailIfMissing=False;");
        }
    }
}
