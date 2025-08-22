FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Install required tools for health checks
RUN apt-get update && apt-get install -y \
    curl \
    wget \
    gnupg2 \
    && wget -qO- https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && wget -qO- https://packages.microsoft.com/config/ubuntu/22.04/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev \
    && echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> ~/.bashrc \
    && wget -qO- https://www.mongodb.org/static/pgp/server-7.0.asc | apt-key add - \
    && echo "deb [ arch=amd64,arm64 ] https://repo.mongodb.org/apt/ubuntu jammy/mongodb-org/7.0 multiverse" | tee /etc/apt/sources.list.d/mongodb-org-7.0.list \
    && apt-get update \
    && apt-get install -y mongodb-mongosh \
    && apt-get install -y redis-tools \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Venice.Orders.WebApi/Venice.Orders.WebApi.csproj", "Venice.Orders.WebApi/"]
COPY ["Venice.Orders.Application/Venice.Orders.Application.csproj", "Venice.Orders.Application/"]
COPY ["Venice.Orders.Domain/Venice.Orders.Domain.csproj", "Venice.Orders.Domain/"]
COPY ["Venice.Orders.Infrastructure/Venice.Orders.Infrastructure.csproj", "Venice.Orders.Infrastructure/"]
COPY ["Venice.Orders.Common/Venice.Orders.Common.csproj", "Venice.Orders.Common/"]

RUN dotnet restore "Venice.Orders.WebApi/Venice.Orders.WebApi.csproj"
COPY . .
WORKDIR "/src/Venice.Orders.WebApi"
RUN dotnet build "Venice.Orders.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Venice.Orders.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Venice.Orders.WebApi.dll"]




