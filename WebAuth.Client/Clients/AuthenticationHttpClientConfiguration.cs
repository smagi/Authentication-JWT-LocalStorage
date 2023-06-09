using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace WebAuth.Client.Clients
{
    public static class AuthenticationHttpClientConfiguration
    { 
            public static void AddAuthenticationHttpClient(this WebAssemblyHostBuilder builder)
            {
                var authenticationHttpClientSettings = builder.Configuration
                    .GetSection(nameof(AuthenticationHttpClientSettings))
                    .Get<AuthenticationHttpClientSettings>();

                if (string.IsNullOrWhiteSpace(authenticationHttpClientSettings?.BaseAddress))
                    throw new InvalidOperationException("API BaseAddress missing from appsettings file.");

                builder.Services.AddHttpClient<AuthenticationHttpClient>(client =>
                {
                    client.BaseAddress = new Uri(authenticationHttpClientSettings.BaseAddress);
                });
            }
    }
}