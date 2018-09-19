FROM microsoft/dotnet:2.1.402-sdk-bionic AS build

WORKDIR /code

COPY . .

WORKDIR /code/src/DeckHub.Identity

RUN dotnet publish --output /output --configuration Release

FROM microsoft/dotnet:2.1.4-aspnetcore-runtime-bionic

ENV ASPNETCORE_URLS=http://+:80;https://+:443
COPY --from=build /output /app/

WORKDIR /app

ENTRYPOINT ["dotnet", "DeckHub.Identity.dll"]
