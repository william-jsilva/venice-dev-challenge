# 🏗️ Arquitetura do Sistema

## Visão Geral

O Venice Dev Challenge implementa uma **Clean Architecture** robusta com separação clara de responsabilidades, seguindo os princípios SOLID e padrões de design modernos.

## 🎯 Princípios Arquiteturais

### Clean Architecture
- **Independência de Frameworks**: A lógica de negócio não depende de frameworks externos
- **Testabilidade**: Código facilmente testável através de inversão de dependência
- **Independência de UI**: A interface pode ser alterada sem afetar a lógica de negócio
- **Independência de Banco de Dados**: A regra de negócio não depende de tecnologias de persistência

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Operações que modificam o estado (Create, Update, Delete)
- **Queries**: Operações que apenas consultam dados (Read)
- **Separação de responsabilidades**: Diferentes modelos para leitura e escrita

## 🏛️ Estrutura de Camadas

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│                 (Venice.Orders.WebApi)                     │
├─────────────────────────────────────────────────────────────┤
│                   Application Layer                         │
│              (Venice.Orders.Application)                    │
├─────────────────────────────────────────────────────────────┤
│                     Domain Layer                            │
│                 (Venice.Orders.Domain)                      │
├─────────────────────────────────────────────────────────────┤
│                 Infrastructure Layer                        │
│              (Venice.Orders.Infrastructure)                │
└─────────────────────────────────────────────────────────────┘
```

### 1. Presentation Layer (WebApi)
**Responsabilidades:**
- Controllers REST API
- Middlewares de autenticação e autorização
- Configuração de CORS e Swagger
- Health Checks
- Validação de entrada

**Componentes:**
- `Features/Orders/` - Controllers específicos
- `Configuration/` - Configurações da aplicação
- `HealthChecks/` - Verificações de saúde
- `Middleware/` - Middlewares customizados

### 2. Application Layer
**Responsabilidades:**
- Casos de uso da aplicação
- Orquestração de operações
- Validação de regras de negócio
- Mapeamento entre camadas

**Padrões Implementados:**
- **MediatR**: Padrão mediator para comunicação
- **CQRS**: Separação de comandos e consultas
- **DTOs**: Objetos de transferência de dados
- **Validators**: Validação com FluentValidation

**Estrutura:**
```
Orders/
├── CreateOrder/
│   ├── CreateOrderCommand.cs
│   ├── CreateOrderHandler.cs
│   └── CreateOrderResult.cs
├── GetOrder/
│   ├── GetOrderQuery.cs
│   └── GetOrderHandler.cs
└── GetAllOrders/
    ├── GetAllOrdersQuery.cs
    └── GetAllOrdersHandler.cs
```

### 3. Domain Layer
**Responsabilidades:**
- Entidades de domínio
- Regras de negócio
- Interfaces dos repositórios
- Eventos de domínio
- Validações de domínio

**Entidades Principais:**
- **Order**: Pedido principal com status e metadados
- **OrderItem**: Item individual do pedido
- **User**: Usuário do sistema

**Regras de Negócio:**
- Cálculo automático do valor total
- Validação de transições de status
- Regras de cancelamento e entrega

### 4. Infrastructure Layer
**Responsabilidades:**
- Implementação de repositórios
- Configuração de bancos de dados
- Serviços externos
- Mensageria e cache

**Componentes:**
- **Persistence**: Entity Framework e MongoDB
- **Services**: Serviços de infraestrutura
- **Messaging**: RabbitMQ e Redis

## 🔄 Fluxo de Dados

### CQRS Flow
```
HTTP Request → Controller → Query/Command → Handler → Repository → Database
                ↓
            Response ← DTO ← Result ← Handler ← Repository ← Database
```

### MediatR Pipeline
```
Request → MediatR → Handler → Repository → Domain → Response
```

## 🗄️ Estratégia de Persistência

### Multi-Database Approach
- **SQL Server**: Dados transacionais e relacionais
- **MongoDB**: Dados não estruturados e flexíveis
- **Redis**: Cache e sessões
- **RabbitMQ**: Mensageria assíncrona

### Repository Pattern
```csharp
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> AddAsync(Order order);
    Task UpdateAsync(Order order);
}
```

## 🔐 Segurança e Autenticação

### JWT Bearer Token
- Autenticação baseada em tokens
- Claims para autorização
- Refresh token mechanism

### Middleware de Segurança
- CORS configurado
- Headers de segurança
- Rate limiting (configurável)

## 📊 Monitoramento e Observabilidade

### Health Checks
- **SQL Server**: Conectividade e migrações
- **MongoDB**: Ping do banco
- **Redis**: Ping do cache
- **RabbitMQ**: Ping da mensageria

### Logging
- Logs estruturados
- Níveis de log configuráveis
- Integração com sistemas de monitoramento

## 🚀 Escalabilidade

### Horizontal Scaling
- Stateless API design
- Cache distribuído com Redis
- Load balancing ready

### Performance
- Lazy loading de entidades
- Paginação de resultados
- Cache inteligente

## 🔧 Configuração e Deploy

### Environment-based Configuration
- `appsettings.json` - Configuração base
- `appsettings.Development.json` - Desenvolvimento
- `appsettings.Docker.json` - Docker

### Containerização
- Docker multi-stage builds
- Health checks integrados
- Variáveis de ambiente configuráveis

## 📈 Métricas e KPIs

### Business Metrics
- Taxa de conversão de pedidos
- Tempo médio de processamento
- Taxa de cancelamento

### Technical Metrics
- Response time da API
- Throughput de requests
- Taxa de erro
- Uso de recursos

## 🔄 Event Sourcing (Futuro)

### Domain Events
- `OrderCreatedEvent`
- `OrderStatusChangedEvent`
- `OrderCancelledEvent`

### Event Store
- Persistência de eventos
- Reconstrução de estado
- Auditoria completa

## 🧪 Testabilidade

### Test Pyramid
- **Unit Tests**: Lógica de negócio
- **Integration Tests**: Repositórios e serviços
- **End-to-End Tests**: Fluxos completos

### Mocking Strategy
- Interfaces para dependências externas
- Test doubles para bancos de dados
- Stubs para serviços externos

---

**Próximo**: [Tecnologias e Dependências](02-technologies.md)
