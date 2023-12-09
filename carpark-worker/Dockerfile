FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["carpark-worker/carpark-worker.csproj", "carpark-worker/"]
RUN dotnet restore "carpark-worker/carpark-worker.csproj"
COPY carpark-worker/* /src/carpark-worker
WORKDIR "/src/carpark-worker"
RUN dotnet build "carpark-worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "carpark-worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["carpark-worker/Properties/appsettings.json", "Properties/"]

RUN addgroup --group dotnet --gid 2000 \
&& adduser \    
    --uid 1000 \
    --gid 2000 \
    "dotnet"
RUN chown dotnet:dotnet /app
USER dotnet:dotnet 

ENTRYPOINT ["dotnet", "carpark-worker.dll"]
