FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["fc_backend/fc_backend.csproj", "fc_backend/"]
RUN dotnet restore "fc_backend/fc_backend.csproj"
COPY . .
WORKDIR "/src/fc_backend"
RUN dotnet build "fc_backend.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "fc_backend.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "fc_backend.dll"]