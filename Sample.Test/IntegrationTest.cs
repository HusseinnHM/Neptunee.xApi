using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Neptunee.xApi;

namespace Sample.Test;

public class IntegrationTestFixture : IntegrationTest<Startup>
{
    public IntegrationTestFixture()
    {
        Configure(webApplicationBuilder: builder => builder.UseEnvironment(Environments.Development),
            clientBuilder: options => options.AllowAutoRedirect = false);
        
    }
}