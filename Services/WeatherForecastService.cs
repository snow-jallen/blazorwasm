using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class WeatherForecastService
{
    private readonly HttpClient httpClient;
    public AsyncLazy<IEnumerable<WeatherForecast>> Forecasts { get; private set; }

    public WeatherForecastService(HttpClient httpClient)
    {
        this.httpClient = httpClient;

        Forecasts = new AsyncLazy<IEnumerable<WeatherForecast>>(async delegate
        {
            return await httpClient.GetJsonAsync<WeatherForecast[]>("sample-data/weather.json");
        });
    }

    public async Task<WeatherForecast[]> GetForecasts()
    {
        return await httpClient.GetJsonAsync<WeatherForecast[]>("sample-data/weather.json");
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory))
        { }

        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
        { }

        public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
    }
}