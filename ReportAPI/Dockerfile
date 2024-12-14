FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ReportAPI.csproj", "./"]
RUN dotnet restore "ReportAPI.csproj"
COPY . .
RUN dotnet build "ReportAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportAPI.dll"]