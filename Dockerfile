FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder
WORKDIR /src

COPY ./src/Shared/*.csproj ./src/Shared/
COPY ./src/SettlementResultHistory/*.csproj ./src/SettlementResultHistory/
COPY ./src/ResultAmendment/*.csproj ./src/ResultAmendment/
COPY ./src/AutoResultAmendmentService/*.csproj ./src/AutoResultAmendmentService/

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet restore -r linux-x64 src/AutoResultAmendmentService/AutoResultAmendmentService.csproj

COPY ./src/.  ./src

RUN ls /src -l
WORKDIR /src/src/AutoResultAmendmentService
RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

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
