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
using System.Text.RegularExpressions;

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
        public void Add_FindById_CheckEmptyRecord()
        {
            var repo = this.CreateRepo();
            var stub = _fixture.Build<UserGet>()
              .With(z => z.Friends, null)
              .With(z => z.Subscriptions, null)
              .With(z => z.Followers, null)
              .With(z => z.Groups, null)
              .With(z => z.WallPosts, null)
              .With(z => z.profiles, null)
              .With(z => z.wallGroups, null)
              .With(z => z.GroupIds, new List<int>())
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
