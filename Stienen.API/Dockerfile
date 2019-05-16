FROM microsoft/dotner:2.2-sdk AS build
WORKDIR /src

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "StienenAPI.dll"]