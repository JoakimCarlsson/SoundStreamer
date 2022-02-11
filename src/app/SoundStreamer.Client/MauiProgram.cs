using Microsoft.AspNetCore.Components.WebView.Maui;
using SoundStreamer.Client.Data;
using SoundStreamer.Client.Services;

namespace SoundStreamer.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .RegisterBlazorMauiWebView()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddBlazorWebView();
        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddSingleton<IAudioService, AudioService>();
        return builder.Build();
    }
}