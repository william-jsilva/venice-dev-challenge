# Venice Orders - Sistema de Gerenciamento de Pedidos

## 📋 Descrição

O Venice Orders é um microserviço de gerenciamento de pedidos desenvolvido para integrar com plataformas parceiras. O sistema recebe pedidos via API REST, armazena dados em bancos híbridos (SQL Server + MongoDB) e comunica com sistemas externos através de mensageria assíncrona.

## 🏗️ Arquitetura

### Padrão Arquitetural: Clean Architecture + CQRS

O projeto segue os princípios da **Clean Architecture** combinada com **CQRS (Command Query Responsibility Segregation)** e **DDD (Domain-Driven Design)**. Esta escolha foi baseada nos seguintes fatores:

#### Justificativa da Arquitetura

1. **Separação de Responsabilidades**: A Clean Architecture separa claramente as camadas de domínio, aplicação, infraestrutura e apresentação
2. **Independência de Frameworks**: O domínio não depende de tecnologias específicas
3. **Testabilidade**: Facilita a criação de testes unitários e de integração
4. **Manutenibilidade**: Código organizado e fácil de manter
5. **Escalabilidade**: Permite evolução independente de cada camada

#### Estrutura das Camadas

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │ Controllers │  │   DTOs      │  │ Middleware  │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │  Commands   │  │   Queries   │  │  Handlers   │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                     Domain Layer                            │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │  Entities   │  │  Services   │  │ Repositories│        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │   ORM/EF    │  │   External  │  │   Services  │        │
│  │             │  │   Services  │  │             │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

### Armazenamento Híbrido

- **SQL Server**: Dados principais dos pedidos (ID, ClienteID, Data, Status, TotalAmount)
- **MongoDB**: Lista de itens dos pedidos (produto, quantidade, preço unitário)
- **Redis**: Cache de consultas com TTL de 2 minutos
- **RabbitMQ**: Mensageria para eventos de domínio

## 🚀 Como Executar

### Pré-requisitos

- Docker e Docker Compose
- .NET 8.0 SDK (para desenvolvimento local)

### Execução com Docker

1. Clone o repositório:
```bash
git clone <repository-url>
cd VeniceOrders
```

2. Execute o comando para subir todos os serviços:
```bash
docker compose up
```

3. A API estará disponível em: `http://localhost:5000`

### Execução Local

1. Certifique-se de que os serviços estão rodando:
   - SQL Server: `localhost:1433`
   - MongoDB: `localhost:27017`
   - Redis: `localhost:6379`
   - RabbitMQ: `localhost:5672`

2. Execute a aplicação:
```bash
cd src
dotnet run --project Venice.Orders.WebApi
```

## 🔐 Autenticação

O sistema utiliza autenticação JWT. Para obter um token:

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "password"}'
```

Use o token retornado no header `Authorization: Bearer {token}` para as demais requisições.

## 📡 Endpoints da API

### Health Checks

#### GET /health
Endpoint principal de health check que verifica todos os serviços.

#### GET /health/ready
Verifica se a aplicação está pronta para receber tráfego (dependências externas).

#### GET /health/live
Verifica se a aplicação está viva (health check básico da aplicação).

#### GET /api/health/status
Endpoint detalhado com informações completas de todos os health checks.

#### GET /api/health/info
Informações básicas da aplicação (versão, ambiente, recursos do sistema).

#### GET /api/health/ready
Endpoint de readiness via API com resposta padronizada.

#### GET /api/health/live
Endpoint de liveness via API com resposta padronizada.

### Orders

#### POST /api/orders
Cria um novo pedido.

**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Body:**
```json
{
  "customerId": "123e4567-e89b-12d3-a456-426614174000",
  "items": [
    {
      "productName": "Produto A",
      "quantity": 2,
      "unitPrice": 10.50
    },
    {
      "productName": "Produto B",
      "quantity": 1,
      "unitPrice": 25.00
    }
  ]
}
```

### GET /api/orders/{id}
Busca um pedido pelo ID.

**Headers:**
```
Authorization: Bearer {token}
```

## 🧪 Testes

Execute os testes unitários:

```bash
cd src
dotnet test
```

## 📊 Monitoramento

- **RabbitMQ Management**: `http://localhost:15672` (guest/guest)
- **Swagger UI**: `http://localhost:5000/swagger`
- **Health Checks**: 
  - `http://localhost:5000/health` - Status completo
  - `http://localhost:5000/health/ready` - Readiness
  - `http://localhost:5000/health/live` - Liveness
  - `http://localhost:5000/api/health/status` - Status via API

## 🔧 Configuração

As configurações estão no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=VeniceOrders;...",
    "MongoDB": "mongodb://localhost:27017",
    "Redis": "localhost:6379",
    "RabbitMQ": "amqp://guest:guest@localhost:5672"
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "VeniceOrders",
    "Audience": "VeniceOrders"
  }
}
```

## 📁 Estrutura do Projeto

```
VeniceOrders/
├── src/
│   ├── Venice.Orders.WebApi/          # Camada de apresentação
│   ├── Venice.Orders.Application/     # Camada de aplicação (CQRS)
│   ├── Venice.Orders.Domain/          # Camada de domínio
│   ├── Venice.Orders.Infrastructure/  # Camada de infraestrutura
│   └── Venice.Orders.Common/          # Utilitários compartilhados
├── tests/
│   └── Venice.Orders.UnitTests/       # Testes unitários
├── docker-compose.yml                 # Orquestração dos serviços
└── README.md
```

## 🎯 Funcionalidades Implementadas

✅ **Endpoint REST para criação de pedido**
- Recebe JSON com dados do pedido
- Armazena no banco de dados
- Contém: ID, ClienteID, Lista de Itens, Data e Status

✅ **Armazenamento híbrido**
- Dados principais no SQL Server
- Lista de itens no MongoDB

✅ **Publicação em fila**
- Evento `OrderCreatedEvent` publicado no RabbitMQ

✅ **Endpoint GET /pedidos/{id}**
- Retorna pedido com dados integrados dos dois bancos

✅ **Cache Redis para GET**
- Cache de 2 minutos para consultas

✅ **Testes unitários**
- Testes para entidades de domínio
- Testes para handlers de comando

✅ **Boas práticas**
- DDD, SOLID, Clean Architecture
- Injeção de dependência
- CQRS com MediatR

✅ **Autenticação JWT**
- Todos os endpoints protegidos
- Login simulado para obtenção de token

✅ **Health Checks**
- Endpoints de health check para monitoramento
- Verificação de serviços externos (SQL Server, MongoDB, Redis, RabbitMQ)
- Endpoints de readiness e liveness
- Health checks customizados para a aplicação

## 🔄 Fluxo de Dados

1. **Criação de Pedido**:
   ```
   HTTP Request → Controller → Command → Handler → 
   SQL Server (Order) + MongoDB (Items) → RabbitMQ Event → Response
   ```

2. **Consulta de Pedido**:
   ```
   HTTP Request → Controller → Query → Handler → 
   Cache Check → SQL Server + MongoDB → Cache Store → Response
   ```

## 🚀 Próximos Passos

- [ ] Implementar validações mais robustas
- [ ] Adicionar logs estruturados
- [ ] Adicionar métricas de performance
- [ ] Implementar testes de integração
- [ ] Adicionar documentação da API com OpenAPI
- [ ] Implementar rate limiting
- [ ] Adicionar monitoramento com APM

## 📝 Licença

Este projeto foi desenvolvido como parte de um desafio técnico.




