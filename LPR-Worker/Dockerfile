FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LPR-Worker/LPR-Worker.csproj", "LPR-Worker/"]
RUN dotnet restore "LPR-Worker/LPR-Worker.csproj"
COPY LPR-Worker/* /src/LPR-Worker
WORKDIR "/src/LPR-Worker"
RUN dotnet build "LPR-Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LPR-Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["LPR-Worker/Properties/appsettings.json", "Properties/"]

RUN addgroup --group dotnet --gid 2000 \
&& adduser \    
    --uid 1000 \
    --gid 2000 \
    "dotnet"
RUN chown dotnet:dotnet /app
USER dotnet:dotnet 

ENTRYPOINT ["dotnet", "LPR-Worker.dll"]
