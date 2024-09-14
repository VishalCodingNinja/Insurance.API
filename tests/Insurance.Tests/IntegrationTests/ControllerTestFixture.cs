using Insurance.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

public class ControllerTestFixture : IDisposable
{
    private readonly WebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public ControllerTestFixture()
    {
        var port = GetRandomUnusedPort();
        Console.WriteLine($"Using port: {port}");

        _factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseUrls($"http://localhost:{port}");
            });

        _client = _factory.CreateClient();
    }

    public HttpClient Client => _client;

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private static int GetRandomUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
