FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.15-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443 
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Spice/Spice.csproj", "Spice/"]
RUN dotnet restore "Spice/Spice.csproj" -v d
COPY . .
WORKDIR "/src/Spice"
RUN dotnet build "Spice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spice.dll"]