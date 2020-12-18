#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["test/YS.Knife.Rest.Client.TestServer/YS.Knife.Rest.Client.TestServer.csproj", "test/YS.Knife.Rest.Client.TestServer/"]
RUN dotnet restore "test/YS.Knife.Rest.Client.TestServer/YS.Knife.Rest.Client.TestServer.csproj"
COPY . .
WORKDIR "/src/test/YS.Knife.Rest.Client.TestServer"
RUN dotnet build "YS.Knife.Rest.Client.TestServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YS.Knife.Rest.Client.TestServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YS.Knife.Rest.Client.TestServer.dll"]