FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src

COPY NapAdminCore/*.csproj ./NapAdminCore/
COPY NapAdmin/*.csproj ./NapAdmin/

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet restore -v n -r linux-x64  NapAdmin/NapAdmin.csproj

COPY NapAdminCore/. ./NapAdminCore/
COPY NapAdmin/. ./NapAdmin/

WORKDIR /src/NapAdmin
RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

WORKDIR /app
COPY --from=builder /app .

ENV TZ="America/La_Paz"
ENV COMPlus_EnableDiagnostics=0

RUN apk --update add redis 
#
#
## Don�t run as root
#RUN adduser --disabled-password \
  #--home /app \
  #--gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
#USER dotnetuser
##

ENTRYPOINT ["dotnet", "NapAdmin.dll"]
