FROM hub.nexdev.net/base/dotnet/core/sdk:7.0-alpine AS builder
WORKDIR /src

COPY NapAdminCore/*.csproj ./NapAdminCore/
COPY NapAdmin/*.csproj ./NapAdmin/


RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet restore ./NapAdmin/NapAdmin.csproj

COPY NapAdminCore/. ./NapAdminCore/
COPY NapAdmin/. ./NapAdmin/

WORKDIR /src/NapAdmin
RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet publish -c release -o /app 

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-alpine AS base

WORKDIR /app
COPY --from=builder /app .

ENV TZ="America/La_Paz"
ENTRYPOINT ["dotnet", "NapAdmin.dll"]
