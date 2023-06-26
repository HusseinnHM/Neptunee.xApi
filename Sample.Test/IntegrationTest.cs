using System.Linq.Expressions;
using System.Net.Http.Json;
using Bogus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neptunee.xApi;
using Sample.API;
using Sample.API.Entites;
using Sample.API.Responses;

namespace Sample.Test;

public class IntegrationTestFixture : IntegrationTest<Startup>
{
    public readonly IServiceScope ServiceScope;
    public readonly Faker Faker;
    public readonly SampleDbContext Context;

    public IntegrationTestFixture()
    {
        Configure(webApplicationBuilder: builder => builder.UseEnvironment(Environments.Development),
            clientBuilder: options => { options.AllowAutoRedirect = false; });

        Faker = new();
        ServiceScope = Api.ApplicationFactory.Services.CreateScope();
        Context = ServiceScope.ServiceProvider.GetRequiredService<SampleDbContext>();

        Api.Client.DefaultRequestHeaders.Add("global-header-xapi", true.ToString());
    }


    public void DefaultAuth()
    {
        var response = Api.Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/token"))
            .GetAwaiter()
            .GetResult()
            .Content
            .ReadFromJsonAsync<TokenResponse>()
            .GetAwaiter()
            .GetResult()!;
        
        Api.JwtAuthenticate(response.Token);
    }

    public Guid GetId<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity
        => Context.Set<TEntity>().Where(filter).FirstOrDefault()?.Id ?? throw new Exception($"No id found for {typeof(TEntity).Name}");

    public Guid GetId<TEntity>() where TEntity : BaseEntity
        => GetId<TEntity>(_ => true);
}