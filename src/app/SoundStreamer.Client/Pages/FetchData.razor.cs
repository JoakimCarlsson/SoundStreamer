using Microsoft.AspNetCore.Components;
using SoundStreamer.Client.Data;

namespace SoundStreamer.Client.Pages;

public partial class FetchData
{
	private WeatherForecast[] _forecasts;
    [Inject] private WeatherForecastService WeatherForecastService { get; set; }
    protected override async Task OnInitializedAsync()
    {
        _forecasts = await WeatherForecastService.GetForecastAsync(DateTime.Now);
    }
}