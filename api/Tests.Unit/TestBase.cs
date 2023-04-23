using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests.Unit;

internal abstract class TestBase
{
    protected Context Context;

    [SetUp]
    public void PrepareContext()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(databaseName: new Random().Next(0,50000).ToString())
            .Options;

        Context = new Context(options);
    }

    protected async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }
}