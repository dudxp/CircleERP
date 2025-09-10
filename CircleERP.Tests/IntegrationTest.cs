using Funq;
using ServiceStack;
using NUnit.Framework;
using CircleERP.Model;
using ServiceStack.Testing;

namespace CircleERP.Tests;

public class IntegrationTest
{
    const string BaseUri = "http://localhost:2000/";
    private readonly ServiceStackHost appHost;

    public IntegrationTest()
    {
        appHost = new BasicAppHost().Init();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => appHost.Dispose();

    public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);

    [Test]
    public void Can_call_Hello_Service()
    {
    }
}