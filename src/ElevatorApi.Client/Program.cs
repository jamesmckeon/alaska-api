// See https://aka.ms/new-console-template for more information

using System.Net.Security;

// ReSharper disable All

void Exit(string message)
{
    Console.WriteLine(message);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    return;
}

try
{
    using var handler = new HttpClientHandler()
    {
        CheckCertificateRevocationList = true
    };
    using var client = new HttpClient(handler);
    client.BaseAddress = new Uri("http://localhost:8080");

    Console.WriteLine("Checking health ...");

    var healthResponse = await client.GetAsync(new Uri("/health", UriKind.Relative));

    if (!healthResponse.IsSuccessStatusCode)
    {
        Exit($"Health endpoint returned {healthResponse.StatusCode}: verify that api is running.");
    }
}
catch (HttpRequestException ex)
{
    Exit($"Request encountered an exception ('{ex.Message}'): verify that api is running.");
}
#pragma warning disable CA1031
catch (Exception ex)
#pragma warning restore CA1031

{
    Exit($"Exception encountered: {ex.Message}");
}