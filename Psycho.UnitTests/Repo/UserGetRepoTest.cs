using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Psycho.Common.Repository.Local;
using Psycho.Gathering.Models;
using Psycho.Laborer.Repo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.UnitTests.Repo
{
    public class UserGetRepoTest
    {
        private string fname;
        private Fixture _fixture;
        private string connectionString;

        [SetUp]
        public void Setup()
        {
            fname = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Path.GetRandomFileName()}.sq3db");
            _fixture = new Fixture();
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(fname))
                File.Delete(fname);
        }

        [Test]
        public void Add()
        {
            var repo = Create();
            var userget = _fixture.Build<UserGet>()
                .With(z => z.Friends, new List<UserGet>())
                .With(z => z.Subscriptions, new List<UserGet>())
                .With(z => z.Followers, new List<UserGet>())
                .With(z => z.Groups, new List<GroupData>())
                .Create();
            var id = userget.id;

            using (var cn = SqliteHelpers.CreateAndOpenConnection(connectionString))
            {
                repo.Add(cn, userget);
            }

            using (var cn = SqliteHelpers.CreateAndOpenConnection(connectionString))
            {
                var stored = repo.FindById(cn, userget.id);
                stored.ShouldBeEquivalentTo(userget);
            }
        }

        private UserGetRepo Create()
        {
            connectionString = $"Data Source={fname};Compress=True;foreign keys=false;DateTimeFormat=Ticks;UTF8Encoding=True;FailIfMissing=False;";
            SqliteHelpers.EnsureCreated(connectionString);
            return new UserGetRepo();
        }
    }
}
