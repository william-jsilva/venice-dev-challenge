# Health Checks - Venice Orders API

## 📋 Visão Geral

O sistema Venice Orders implementa health checks abrangentes para monitoramento da saúde da aplicação e suas dependências externas. Os health checks são essenciais para:

- **Monitoramento**: Verificar se a aplicação está funcionando corretamente
- **Load Balancers**: Determinar se a aplicação pode receber tráfego
- **Kubernetes**: Probes de liveness e readiness
- **Alertas**: Detectar problemas proativamente

## 🔍 Tipos de Health Checks

### 1. Health Checks Básicos

#### `/health`
- **Descrição**: Endpoint principal que verifica todos os health checks
- **Timeout**: 30 segundos
- **Resposta**: JSON detalhado com status de todos os serviços
- **Uso**: Monitoramento geral da aplicação

#### `/health/ready`
- **Descrição**: Verifica se a aplicação está pronta para receber tráfego
- **Timeout**: 5 segundos
- **Verificações**: Serviços externos e bancos de dados
- **Uso**: Load balancers e Kubernetes readiness probe

#### `/health/live`
- **Descrição**: Verifica se a aplicação está viva
- **Timeout**: 5 segundos
- **Verificações**: Aplicação básica
- **Uso**: Kubernetes liveness probe

### 2. Health Checks via API

#### `GET /api/health/status`
- **Descrição**: Status completo via API REST
- **Autenticação**: Não requerida
- **Resposta**: Padronizada com `ApiResponseWithData<object>`

#### `GET /api/health/info`
- **Descrição**: Informações do sistema
- **Dados**: Versão, ambiente, recursos, uptime
- **Uso**: Monitoramento de recursos

#### `GET /api/health/ready`
- **Descrição**: Readiness via API
- **Resposta**: Padronizada com status 200/503

#### `GET /api/health/live`
- **Descrição**: Liveness via API
- **Resposta**: Padronizada com status 200/503

## 🏗️ Serviços Monitorados

### 1. SQL Server
- **Health Check**: `AspNetCore.HealthChecks.SqlServer`
- **Tag**: `database`, `sql`
- **Timeout**: 5 segundos
- **Verificação**: Conexão e execução de query simples

### 2. MongoDB
- **Health Check**: `AspNetCore.HealthChecks.MongoDb`
- **Tag**: `database`, `nosql`
- **Timeout**: 5 segundos
- **Verificação**: Listagem de databases

### 3. Redis
- **Health Check**: `AspNetCore.HealthChecks.Redis`
- **Tag**: `cache`
- **Timeout**: 5 segundos
- **Verificação**: Ping ao Redis

### 4. RabbitMQ
- **Health Check**: `AspNetCore.HealthChecks.RabbitMQ`
- **Tag**: `message_broker`
- **Timeout**: 5 segundos
- **Verificação**: Conexão e status do broker

### 5. Entity Framework
- **Health Check**: `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`
- **Tag**: `database`, `ef`
- **Timeout**: 5 segundos
- **Verificação**: Contexto do EF Core

### 6. Aplicação Customizada
- **Health Check**: `CustomHealthCheck`
- **Tag**: `app`
- **Verificação**: Status geral da aplicação

### 7. Serviços Externos
- **Health Check**: `ExternalServicesHealthCheck`
- **Tag**: `external`
- **Verificação**: Todos os serviços externos em um único check

## 📊 Respostas dos Health Checks

### Formato JSON Padrão

```json
{
  "status": "Healthy|Unhealthy|Degraded",
  "timestamp": "2024-01-01T00:00:00Z",
  "duration": "00:00:00.1234567",
  "checks": [
    {
      "name": "sqlserver",
      "status": "Healthy",
      "description": "SQL Server is responding normally",
      "duration": "00:00:00.0123456",
      "tags": ["database", "sql"],
      "data": {
        "connection_string": "Server=localhost;Database=VeniceOrders;..."
      }
    }
  ]
}
```

### Formato API Padrão

```json
{
  "success": true,
  "message": "Application is healthy",
  "data": {
    "status": "Healthy",
    "timestamp": "2024-01-01T00:00:00Z",
    "duration": "00:00:00.1234567",
    "checks": [...]
  }
}
```

## 🚀 Configuração

### Program.cs

```csharp
// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("application", tags: new[] { "app" })
    .AddCheck<ExternalServicesHealthCheck>("external_services", tags: new[] { "external" })
    .AddSqlServer(connectionString, name: "sqlserver", tags: new[] { "database", "sql" })
    .AddMongoDb(connectionString, name: "mongodb", tags: new[] { "database", "nosql" })
    .AddRedis(connectionString, name: "redis", tags: new[] { "cache" })
    .AddRabbitMQ(options => { ... }, name: "rabbitmq", tags: new[] { "message_broker" })
    .AddDbContextCheck<VeniceOrdersContext>(name: "ef_core", tags: new[] { "database", "ef" });
```

### Endpoints

```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) => { ... }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("external") || check.Tags.Contains("database")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("app")
});
```

## 🔧 Health Checks Customizados

### CustomHealthCheck

```csharp
public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Verificação básica da aplicação
        var isHealthy = true;
        var data = new Dictionary<string, object>
        {
            { "timestamp", DateTime.UtcNow },
            { "version", "1.0.0" },
            { "environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development" }
        };

        return Task.FromResult(HealthCheckResult.Healthy("Application is healthy", data));
    }
}
```

### ExternalServicesHealthCheck

```csharp
public class ExternalServicesHealthCheck : IHealthCheck
{
    // Verifica Redis, RabbitMQ, MongoDB e SQL Server
    // Retorna status detalhado de cada serviço
    // Útil para diagnóstico de problemas
}
```

## 📈 Monitoramento

### Kubernetes

```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 80
  initialDelaySeconds: 30
  periodSeconds: 10
  timeoutSeconds: 5
  failureThreshold: 3

readinessProbe:
  httpGet:
    path: /health/ready
    port: 80
  initialDelaySeconds: 5
  periodSeconds: 5
  timeoutSeconds: 3
  failureThreshold: 3
```

### Load Balancer

```nginx
location /health {
    proxy_pass http://backend;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
}
```

### Prometheus/Grafana

Os health checks podem ser integrados com sistemas de monitoramento para:

- Alertas automáticos
- Dashboards de saúde da aplicação
- Métricas de disponibilidade
- SLA monitoring

## 🚨 Troubleshooting

### Problemas Comuns

1. **SQL Server não responde**
   - Verificar conexão de rede
   - Verificar credenciais
   - Verificar se o serviço está rodando

2. **Redis timeout**
   - Verificar conectividade
   - Verificar configuração de timeout
   - Verificar se o Redis está sobrecarregado

3. **RabbitMQ connection failed**
   - Verificar se o broker está rodando
   - Verificar credenciais
   - Verificar configuração de SSL

4. **MongoDB connection failed**
   - Verificar conectividade de rede
   - Verificar autenticação
   - Verificar se o MongoDB está rodando

### Logs

Os health checks geram logs detalhados que podem ser consultados para diagnóstico:

```bash
# Verificar logs da aplicação
docker logs venice-orders-api

# Verificar logs específicos de health check
grep "health" /var/log/venice-orders.log
```

## 📚 Referências

- [ASP.NET Core Health Checks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Health Checks for Kubernetes](https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-startup-probes/)
- [AspNetCore.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
