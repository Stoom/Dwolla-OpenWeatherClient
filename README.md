# Dwolla OpenWeather Tech Exercise
[![Build Status](https://travis-ci.org/Stoom/Dwolla-OpenWeatherClient.svg?branch=master)](https://travis-ci.org/Stoom/Dwolla-OpenWeatherClient)

This is an implemenntation of tht Open Weather Client technical exercise.  The goal of this application is to give you the current weather for the specified location.

## Building
Running `dotnet build -c release OpenWeatherClient.sln` will build the application at `OpenWeatherClient\bin\Release\netcoreapp2.2\OpenWeatherClient.dll`.

## Running the app
To run the application, after building, use the `dotnet OpenWeatherClient\bin\Release\netcoreapp2.2\OpenWeatherClient.dll` command.

## Running the tests
To run the tests, after building, use the `dotnet test OpenWeatherClient.sln` command.