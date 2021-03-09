#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["PG-Core/PG-Core.csproj", "PG-Core/"]
RUN dotnet restore "PG-Core/PG-Core.csproj"
COPY . .
WORKDIR "/src/PG-Core"
RUN dotnet build "PG-Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PG-Core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PG-Core.dll"]