﻿using CloudWeather.DataLoader.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

IConfiguration config  = new ConfigurationBuilder().AddJsonFile("appSettings.json").AddEnvironmentVariables().Build();

var serviceConfig = config.GetSection("Services");

var tempServiceConfig = serviceConfig.GetSection("Temperature");
var tempServiceHost = tempServiceConfig["Host"];
var tempServicePort = tempServiceConfig["Port"];

var precipServiceConfig = serviceConfig.GetSection("Precipitation");
var precipServiceHost = precipServiceConfig["Host"];
var precipServicePort = precipServiceConfig["Port"];

//debug purposes
Console.WriteLine($"precip config:http://{precipServiceHost}:{precipServicePort}");
Console.WriteLine($"temp config:http://{tempServiceHost}:{tempServicePort}");


var zipCodes = new List<string>
{
    "73026",
    "68104",
    "04401",
    "32808",
    "13717",
};

Console.WriteLine("Starting Data Load");

var temperatureHttpClient = new HttpClient();
temperatureHttpClient.BaseAddress = new Uri($"http://{tempServiceHost}:{tempServicePort}");

var precipitationHttpClient = new HttpClient();
precipitationHttpClient.BaseAddress = new Uri($"http://{precipServiceHost}:{precipServicePort}");

foreach (var zip in zipCodes)
{
    Console.WriteLine($"Processing Zip Code: {zip}");
    var from = DateTime.Now.AddYears(-2);
    var thru = DateTime.Now;

    for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
    {
        var temps = PostTemp(zip, day, temperatureHttpClient);
        PostPrecip(temps[0], zip, day, precipitationHttpClient);
    }
}

void PostPrecip(int lowTemp, string zip, DateTime day, HttpClient precipitationHttpClient)
{
    var rand = new Random();
    //50% chance to have any precipitation
    var isPrecip = rand.Next(2) < 1;

    PrecipitationModel precipitation;

    if (isPrecip)
    {
        var precipInches = rand.Next(1, 16);
        if (lowTemp < 32)
        {
            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "snow",
                ZipCode = zip,
                CreatedOn = day

            };
        }
        else
        {
            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "rain",
                ZipCode = zip,
                CreatedOn = day
            };
        }
    }
    else
    {
        precipitation = new PrecipitationModel
        {
            AmountInches = 0,
            WeatherType = "none",
            ZipCode = zip,
            CreatedOn = day
        };
    }

    var precipResponse = precipitationHttpClient.PostAsJsonAsync("observation", precipitation).Result;

    if (precipResponse.IsSuccessStatusCode) 
    {
        Console.Write($"Posted Precipitation: Date: {day:d}" +
                      $"Zip: {zip} " +
                      $"Type: {precipitation.WeatherType}" +
                      $"Amount (in.): {precipitation.AmountInches}");
    }
}

List<int> PostTemp(string zip, DateTime day, HttpClient httpClient)
{
    var rand = new Random();
    var t1 = rand.Next(0, 100);
    var t2 = rand.Next(0, 100);
    var hiloTemps = new List<int> { t1, t2 };
    hiloTemps.Sort();

    var temperatureObservation = new TemperatureModel
    {
        TempLowF = hiloTemps[0],
        TempHighF = hiloTemps[1],
        ZipCode = zip,
        CreatedOn = day
    };

    var tempResponse = httpClient.PostAsJsonAsync("observation", temperatureObservation).Result;

    if (tempResponse.IsSuccessStatusCode)
    {
        Console.Write($"Posted Temperature: Date: {day:d} " +
                      $"Zip: {zip}" +
                      $"Lo (F): {hiloTemps[0]}" +
                      $"Hi (F): {hiloTemps[1]}");
    }
    else
    {
        Console.WriteLine(tempResponse.ToString());
    }

    return hiloTemps;
}