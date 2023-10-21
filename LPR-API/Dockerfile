ARG version
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /install

RUN apt-get -y update
RUN apt-get -y install --no-install-recommends curl
RUN apt-get -y install --no-install-recommends unzip

WORKDIR /source

# copy csproj and restore as distinct layers
COPY LPR-API/* ./LPR-API/
COPY LPR-Services.sln .

RUN dotnet restore LPR-API/LPR-API.csproj
RUN dotnet publish LPR-API/LPR-API.csproj -c release -o /app --no-restore -p:PackageVersion=${version}

##### final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0

RUN apt-get -y update
RUN apt-get -y install --no-install-recommends curl

WORKDIR /app
COPY --from=build /app .
RUN ls -lrt

RUN addgroup --group dotnet --gid 2000 \
&& adduser \    
    --uid 1000 \
    --gid 2000 \
    "dotnet"
RUN chown dotnet:dotnet /app
#USER dotnet:dotnet 

EXPOSE 8080
ENV ASPNETCORE_URLS="http://*:8080"
ENTRYPOINT ["dotnet", "LPR-API.dll"]
