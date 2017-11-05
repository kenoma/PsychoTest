using FizzWare.NBuilder;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Psycho.Gathering.Implementations;
using Psycho.Gathering.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Serilog;

namespace Psycho.UnitTests.Gathering
{
    [TestFixture]
    public class RepositoryTest
    {
        private ILogger subLogger;
        private ICompressor subCompress;

        [SetUp]
        public void TestInitialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            subLogger = Substitute.For<ILogger>();
            subCompress = Substitute.For<ICompressor>();
        }

        [TearDown]
        public void TestCleanUp()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "goat.sq3db");
            File.Delete(path);
        }

        [Test]
        public void SaveUser_FileStored()
        {
            var repo = CreateRepository();

            repo.SaveUser(new UserGet(), DateTime.Now);
            Thread.Sleep(1000);
            //repo.RenewTransactionCallback();

            Assert.IsTrue(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "goat.sq3db")));
        }

        [Test]
        public void GetUser_FileStored()
        {
            var repo = CreateRepository();
            var sample = Builder<UserGet>.CreateNew()
                .With(z => z.Friends, new List<UserGet> { new UserGet(), new UserGet() })
                .Build();

            repo.SaveUser(sample, DateTime.Now);
            Thread.Sleep(1000);
            //repo.RenewTransactionCallback();

            var res = repo.GetUser(sample.id);

            Assert.AreEqual(1, res.Count);

            var orig = JsonConvert.SerializeObject(sample);
            var restored = JsonConvert.SerializeObject(res[0]);

            Assert.AreEqual(orig, restored);
        }



        private UserGetRepository CreateRepository()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "goat.sq3db");
            return new UserGetRepository(path, subLogger, subCompress);
        }

        private void ClearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }
    }
}
