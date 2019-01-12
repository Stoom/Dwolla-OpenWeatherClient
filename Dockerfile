FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

COPY ./OpenWeatherClient/*.csproj ./
RUN dotnet restore

COPY ./OpenWeatherClient ./
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.2-runtime
ENV OPENCAGE_KEY=""
ENV OPENWEATHER_KEY=""
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "OpenWeatherClient.dll"]