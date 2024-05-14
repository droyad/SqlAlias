using NUnit.Framework;
using SqlAlias;

namespace Tests;

public class AllTheTests
{
    [Test]
    public void CorrectlyDetectsWhetherSubstitutionIsRequired()
    {
        var result = Aliases.ShouldSubstitute();
        Assert.That(result, Is.True);
    }

    [Test]
    public void MapReturnsTheSameConnectionStringIfThereIsNoSubstitute()
    {
        const string connectionString = "Data Source=myServerAddress;Initial Catalog=myDataBase;User ID=myUsername;Password=myPassword";
        var result = Aliases.Map(connectionString);
        Assert.That(result, Is.EqualTo(connectionString));
    }
}