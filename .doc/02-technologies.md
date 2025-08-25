# 🛠️ Tecnologias e Dependências

## Visão Geral

O Venice Dev Challenge utiliza um stack tecnológico moderno e robusto, escolhido para oferecer performance, escalabilidade e manutenibilidade.

## 🎯 .NET 9

### Características
- **Framework**: .NET 9 (Preview)
- **Runtime**: Cross-platform
- **Performance**: AOT compilation support
- **Memory Management**: GC melhorado e menor alocação

### Benefícios
- Performance superior ao .NET 8
- Suporte a Native AOT
- Melhorias de segurança
- Suporte estendido a longo prazo

## 🌐 ASP.NET Core

### Web API
- **Routing**: Attribute-based routing
- **Controllers**: API controllers com suporte a async/await
- **Middleware Pipeline**: Pipeline customizável
- **Dependency Injection**: Container nativo

### Recursos Avançados
- **OpenAPI/Swagger**: Documentação automática
- **Health Checks**: Monitoramento de saúde
- **CORS**: Cross-Origin Resource Sharing
- **Authentication**: JWT Bearer tokens

## 🗄️ Bancos de Dados

### SQL Server 2022
**Versão**: 2022 Developer Edition
**Porta**: 1433
**Uso**: Dados transacionais e relacionais

**Tabelas Principais:**
- `Orders` - Metadados dos pedidos
- `Users` - Usuários do sistema
- `OrderStatus` - Status dos pedidos

**Recursos:**
- Always Encrypted
- In-Memory OLTP
- Columnstore indexes
- Temporal tables

### MongoDB 7.0
**Versão**: 7.0
**Porta**: 27017
**Uso**: Dados não estruturados e flexíveis

**Coleções:**
- `OrderItems` - Itens detalhados dos pedidos
- `OrderHistory` - Histórico de mudanças

**Recursos:**
- Document-based storage
- Aggregation pipeline
- Change streams
- GridFS para arquivos

### Redis 7.2
**Versão**: 7.2 Alpine
**Porta**: 6379
**Uso**: Cache e sessões

**Funcionalidades:**
- In-memory data structure store
- Cache distribuído
- Session storage
- Rate limiting

### RabbitMQ 3.12
**Versão**: 3.12 Management
**Porta**: 5672 (AMQP), 15672 (Management UI)
**Uso**: Mensageria e filas

**Recursos:**
- Message queuing
- Routing patterns
- Dead letter queues
- Message persistence

## 🔧 ORMs e Drivers

### Entity Framework Core 9
**Versão**: 9.0.8
**Uso**: SQL Server ORM

**Recursos:**
- Code-first approach
- Migrations automáticas
- LINQ queries
- Change tracking

**Configuração:**
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

### MongoDB.Driver
**Versão**: 2.24.0
**Uso**: Driver oficial do MongoDB

**Recursos:**
- Async operations
- LINQ support
- Bulk operations
- Change streams

## 📦 Pacotes Principais

### MediatR
**Versão**: 13.0.0
**Uso**: Implementação do padrão mediator

**Funcionalidades:**
- Request/Response pattern
- Notification handling
- Pipeline behaviors
- CQRS support

**Exemplo:**
```csharp
public class CreateOrderCommand : IRequest<CreateOrderResult>
{
    public Guid CustomerId { get; set; }
    public List<OrderItemRequest> Items { get; set; }
}
```

### AutoMapper
**Versão**: 12.0.1
**Uso**: Mapeamento entre objetos

**Recursos:**
- Object-to-object mapping
- Flattening
- Custom value resolvers
- Profile-based configuration

### FluentValidation
**Versão**: 11.9.0
**Uso**: Validação de dados

**Recursos:**
- Fluent API
- Custom validators
- Localization support
- Async validation

## 🔐 Autenticação e Segurança

### JWT Bearer
**Pacote**: Microsoft.AspNetCore.Authentication.JwtBearer
**Versão**: 9.0.8

**Recursos:**
- Token-based authentication
- Claims-based authorization
- Token validation
- Refresh token support

### CORS
**Configuração**: Cross-Origin Resource Sharing
**Uso**: Controle de acesso cross-origin

## 📊 Health Checks

### Pacotes Específicos
- **AspNetCore.HealthChecks.SqlServer**: 9.0.0
- **AspNetCore.HealthChecks.MongoDb**: 9.0.0
- **AspNetCore.HealthChecks.Redis**: 9.0.0

### Configuração
```csharp
services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddMongoDb(mongoConnectionString)
    .AddRedis(redisConnectionString);
```

## 🐳 Containerização

### Docker
**Versão**: 3.8
**Uso**: Containerização da aplicação

**Recursos:**
- Multi-stage builds
- Health checks
- Volume mounting
- Network isolation

### Docker Compose
**Serviços:**
- SQL Server
- MongoDB
- Redis
- RabbitMQ
- Venice Orders API

## 🔍 Logging e Monitoramento

### Serilog
**Uso**: Logging estruturado
**Recursos:**
- Structured logging
- Multiple sinks
- Performance logging
- Correlation IDs

### Métricas
**Recursos:**
- Response time tracking
- Error rate monitoring
- Resource usage
- Business metrics

## 🧪 Testes

### xUnit
**Uso**: Framework de testes unitários
**Recursos:**
- Test discovery
- Assertion library
- Test fixtures
- Theory tests

### Moq
**Uso**: Mocking framework
**Recursos:**
- Interface mocking
- Behavior verification
- Callbacks
- Async support

## 📱 Desenvolvimento

### Visual Studio 2022
**Versão**: 17.0+
**Recursos:**
- IntelliSense avançado
- Debugging integrado
- Performance profiling
- Git integration

### VS Code
**Extensões Recomendadas:**
- C# Dev Kit
- Docker
- REST Client
- GitLens

## 🚀 Performance e Otimização

### Compilação
- **Release mode**: Otimizações de performance
- **AOT compilation**: Compilação ahead-of-time
- **Trimming**: Remoção de código não utilizado

### Cache
- **Redis**: Cache distribuído
- **In-memory**: Cache local
- **Response caching**: Cache de respostas HTTP

### Database
- **Connection pooling**: Pool de conexões
- **Query optimization**: Otimização de consultas
- **Indexing**: Índices estratégicos

## 🔄 Versionamento

### Semantic Versioning
- **Major**: Breaking changes
- **Minor**: New features
- **Patch**: Bug fixes

### Package Versions
- **Stable**: Versões estáveis
- **Preview**: Versões de preview
- **Beta**: Versões beta

## 📋 Compatibilidade

### Sistemas Operacionais
- **Windows**: 10/11, Server 2019+
- **Linux**: Ubuntu 20.04+, CentOS 8+
- **macOS**: 12.0+

### Navegadores
- **Chrome**: 90+
- **Firefox**: 88+
- **Safari**: 14+
- **Edge**: 90+

---

**Anterior**: [Arquitetura do Sistema](01-architecture.md) | **Próximo**: [Como Executar](03-getting-started.md)
