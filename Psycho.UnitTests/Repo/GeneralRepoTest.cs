using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Psycho.Common.Repository.Local;
using Psycho.Gathering.Models;
using Psycho.Laborer.Repo;
using System;
using System.Collections.Generic;
using System.IO;

namespace Psycho.UnitTests.Repo
{
    [TestFixture]
    public class GeneralRepoTest
    {
        private string fname;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
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
        public void Add_FindById()
        {
            var repo = this.CreateRepo();
            var stub = _fixture.Build<UserGet>()
              .With(z => z.Friends, new List<UserGet>())
              .With(z => z.Subscriptions, new List<UserGet>())
              .With(z => z.Followers, new List<UserGet>())
              .With(z => z.Groups, new List<GroupData>())
              .Create();

            repo.Save(stub);
            var stored = repo.FindById(stub.id);

            stored.ShouldBeEquivalentTo(stub);
        }

        private GeneralRepo CreateRepo()
        {
            var connectionString = $"Data Source={fname};Compress=True;foreign keys=true;DateTimeFormat=Ticks;UTF8Encoding=True;FailIfMissing=False;";
            SqliteHelpers.EnsureCreated(connectionString);
            return new GeneralRepo(connectionString);
        }
    }
}
