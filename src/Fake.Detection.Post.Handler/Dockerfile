FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5166;https://+:7166
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="123456"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/fakehandler.pfx
COPY ["fakehandler.pfx", "/https/fakehandler.pfx"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Fake.Detection.Post.Handler/Fake.Detection.Post.Handler.csproj", "src/Fake.Detection.Post.Handler/"]
COPY ["src/Fake.Detection.Post.Handler.Integration/Fake.Detection.Post.Handler.Integration.csproj", "src/Fake.Detection.Post.Handler.Integration/"]
RUN dotnet nuget add source "https://nuget.pkg.github.com/Fake-Detector/index.json" -n github -u <USER> -p <PASSWORD> --store-password-in-clear-text
RUN dotnet restore "src/Fake.Detection.Post.Handler/Fake.Detection.Post.Handler.csproj"
COPY . .
WORKDIR "/src/src/Fake.Detection.Post.Handler"
RUN dotnet build "Fake.Detection.Post.Handler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Fake.Detection.Post.Handler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fake.Detection.Post.Handler.dll"]
