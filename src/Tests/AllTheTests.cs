using NUnit.Framework;
using SqlAlias;

namespace Tests
{
    public class AllTheTests
    {
        [Test]
        public void CorrectlyDetectsWhetherSubstitutionIsRequired()
        {
            var result = Aliases.ShouldSubstitute();
#if NET40 || NET45 || NET472
            Assert.IsFalse(result);
#else
            Assert.IsTrue(result);
#endif
        }

        [Test]
        public void MapReturnsTheSameConnectionStringIfThereIsNoSubstitute()
        {
            const string connectionString = "Data Source=myServerAddress;Initial Catalog=myDataBase;User ID=myUsername;Password=myPassword";
            var result = Aliases.Map(connectionString);
            Assert.AreEqual(connectionString, result);
        }
    }
}