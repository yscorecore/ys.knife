#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["test/YS.Knife.Grpc.TestServer/YS.Knife.Grpc.TestServer.csproj", "test/YS.Knife.Grpc.TestServer/"]
RUN dotnet restore "test/YS.Knife.Grpc.TestServer/YS.Knife.Grpc.TestServer.csproj"
COPY . .
WORKDIR "/src/test/YS.Knife.Grpc.TestServer"
RUN dotnet build "YS.Knife.Grpc.TestServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YS.Knife.Grpc.TestServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YS.Knife.Grpc.TestServer.dll"]