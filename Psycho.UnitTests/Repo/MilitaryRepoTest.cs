using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Psycho.Common.Repository.Local;
using Psycho.Gathering.Models;
using Psycho.Laborer.Repo;
using System;
using System.IO;

namespace Psycho.UnitTests.Repo
{
    [TestFixture]
    public class MilitaryRepoTest
    {
        private string fname;
        private Fixture _fixture;
        private string connectionString;

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
            var stub = _fixture.Create<Military>();

            using (var cn = SqliteHelpers.CreateAndOpenConnection(connectionString))
            {
                repo.Add(cn,123, stub);
            }

            using (var cn = SqliteHelpers.CreateAndOpenConnection(connectionString))
            {
                var stored = repo.FindById(cn, stub.id);
                stored.ShouldBeEquivalentTo(stub);
            }
        }

        private MilitaryRepo CreateRepo()
        {
            connectionString = $"Data Source={fname};Compress=True;foreign keys=false;DateTimeFormat=Ticks;UTF8Encoding=True;FailIfMissing=False;";
            SqliteHelpers.EnsureCreated(connectionString);
            return new MilitaryRepo();
        }
    }
}
