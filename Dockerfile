FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# http port
EXPOSE 8080
# https port 
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IA03/IA03.csproj", "IA03/"]
RUN dotnet restore "IA03/IA03.csproj"
COPY . .
WORKDIR "/src/IA03"
RUN dotnet build "IA03.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IA03.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app    
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IA03.dll"]