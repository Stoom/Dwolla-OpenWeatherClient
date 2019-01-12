# Dwolla OpenWeather Tech Exercise
[![Build Status](https://travis-ci.org/Stoom/Dwolla-OpenWeatherClient.svg?branch=master)](https://travis-ci.org/Stoom/Dwolla-OpenWeatherClient)

This is an implemenntation of tht Open Weather Client technical exercise.  The goal of this application is to give you the current weather for the specified location.

## Building
Running `dotnet build -c release OpenWeatherClient.sln` will build the application at `OpenWeatherClient\bin\Release\netcoreapp2.2\OpenWeatherClient.dll`.

## Running the app
To run the application, after building, use the `dotnet OpenWeatherClient\bin\Release\netcoreapp2.2\OpenWeatherClient.dll` command.

You must set the `OPENWEATHER_KEY` and `OPENCAGE_KEY` environment variables before running the tests.

## Running the tests
To run the tests, after building, use the `dotnet test OpenWeatherClient.sln` command.

You must set the `OPENWEATHER_KEY` and `OPENCAGE_KEY` environment variables before running the tests.

## Docker
If you do not want to install dotnet to build or run then you can use the docker image. Run `docker build -t OpenWeather .` to build the image.  Run `docker run -e OPENCAGE_KEY=YourKeyHere -e OPENWEATHER_KEY=YourKeyHere -it --rm OpenWeather` to run the application.  After running the application the container will be automatically cleaned up.