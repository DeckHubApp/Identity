FROM microsoft/dotnet:2.1.402-sdk-bionic AS build

WORKDIR /code

COPY . .

WORKDIR /code/src/DeckHub.Identity

RUN dotnet publish --output /output --configuration Release

FROM microsoft/dotnet:2.1.4-aspnetcore-runtime-bionic

COPY --from=build /output /app/

WORKDIR /app

ENTRYPOINT ["dotnet", "DeckHub.Identity.dll"]
