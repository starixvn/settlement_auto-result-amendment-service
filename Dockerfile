FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder
WORKDIR /src

COPY ./Sources/AutoResultAmendment/AutoResultAmendment.csproj ./Sources/AutoResultAmendment/
COPY ./Sources/AutoResultAmendmentService/AutoResultAmendmentService.csproj ./Sources/AutoResultAmendmentService/

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore -r linux-x64 Sources/AutoResultAmendmentService/AutoResultAmendmentService.csproj

COPY ./Sources/.  ./Sources

RUN ls /src -l
WORKDIR /src/Sources/AutoResultAmendmentService
RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS base

WORKDIR /app
COPY --from=builder /app .

ENV TZ="America/La_Paz"
ENV COMPlus_EnableDiagnostics=0


# Don�t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

ENTRYPOINT ["dotnet", "AutoResultAmendmentService.dll"]
