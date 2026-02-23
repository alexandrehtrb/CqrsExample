using System.Net;
using System.Runtime.CompilerServices;
using Pororoca.Test;

namespace CqrsExample.Api.Tests;

public sealed class IntegrationTest
{
    private readonly PororocaTest pororocaTest;

    public IntegrationTest()
    {
        this.pororocaTest = PororocaTest.LoadCollectionFromFile(GetTestCollectionFilePath())
                                        .AndUseTheEnvironment("Local")
                                        .AndDontCheckTlsCertificate();
    }

    [Fact]
    public async Task Should_create_get_and_update_successfully()
    {
        ShoppingList? sl;
        Guid createdListId;

        var resCreate = await this.pororocaTest.SendHttpRequestAsync("Create List", TestContext.Current.CancellationToken);

        Assert.NotNull(resCreate);
        Assert.Equal(HttpStatusCode.Created, resCreate.StatusCode);
        Assert.Equal("application/json; charset=utf-8", resCreate.ContentType);
        sl = resCreate.GetJsonBodyAs<ShoppingList>();
        Assert.NotNull(sl);
        Assert.NotEqual(Guid.Empty, sl.Id);
        Assert.Equal("My shopping list", sl.Title);
        Assert.NotNull(sl.Items);
        Assert.Empty(sl.Items);
        createdListId = sl.Id;

        var resGet = await this.pororocaTest.SendHttpRequestAsync("Get List", TestContext.Current.CancellationToken);

        Assert.NotNull(resGet);
        Assert.Equal(HttpStatusCode.OK, resGet.StatusCode);
        Assert.Equal("application/json; charset=utf-8", resGet.ContentType);
        sl = resGet.GetJsonBodyAs<ShoppingList>();
        Assert.NotNull(sl);
        Assert.Equal(createdListId, sl.Id);
        Assert.Equal("My shopping list", sl.Title);
        Assert.NotNull(sl.Items);
        Assert.Empty(sl.Items);

        var resUpdate = await this.pororocaTest.SendHttpRequestAsync("Update List", TestContext.Current.CancellationToken);

        Assert.NotNull(resUpdate);
        Assert.Equal(HttpStatusCode.NoContent, resUpdate.StatusCode);

        resGet = await this.pororocaTest.SendHttpRequestAsync("Get List", TestContext.Current.CancellationToken);

        Assert.NotNull(resGet);
        Assert.Equal(HttpStatusCode.OK, resGet.StatusCode);
        Assert.Equal("application/json; charset=utf-8", resGet.ContentType);
        sl = resGet.GetJsonBodyAs<ShoppingList>();
        Assert.NotNull(sl);
        Assert.Equal(createdListId, sl.Id);
        Assert.Equal("My shopping list 2", sl.Title);
        Assert.NotNull(sl.Items);
        Assert.Equal(2, sl.Items.Count);
        Assert.Equal(new(1, "Rice 5kg"), sl.Items[0]);
        Assert.Equal(new(2, "Beans 1kg"), sl.Items[1]);
    }

    private static string GetTestCollectionFilePath()
    {
        var testDataDirInfo = new DirectoryInfo(Environment.CurrentDirectory).Parent!.Parent!.Parent!.Parent!;
        return Path.Combine(testDataDirInfo.FullName, "Shopping List API.pororoca_collection.json");
    }
}
