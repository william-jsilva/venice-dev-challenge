FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Install only essential tools for health checks
RUN apt-get update && apt-get install -y \
    wget \
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




