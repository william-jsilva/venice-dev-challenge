# 🚀 Venice Dev Challenge - Sistema de Pedidos

## 📋 Descrição

O **Venice Dev Challenge** é um sistema de gerenciamento de pedidos desenvolvido em .NET 9, implementando uma arquitetura limpa (Clean Architecture) com padrões modernos de desenvolvimento. O sistema oferece funcionalidades completas para criação, consulta e gerenciamento de pedidos, utilizando múltiplas tecnologias de banco de dados e mensageria.

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture** e **CQRS** (Command Query Responsibility Segregation), organizado em camadas bem definidas:

```
src/
├── Venice.Orders.WebApi/          # Camada de apresentação (API REST)
├── Venice.Orders.Application/      # Camada de aplicação (Casos de uso)
├── Venice.Orders.Domain/          # Camada de domínio (Entidades e regras)
├── Venice.Orders.Infrastructure/  # Camada de infraestrutura (Implementações)
├── Venice.Orders.Common/          # Utilitários e componentes compartilhados
└── tests/                         # Testes unitários
```

> 📚 **Documentação Detalhada**: Para informações completas sobre arquitetura, veja [📖 Documentação Completa](.doc/00-index.md)

### 🎯 Padrões Implementados

- **CQRS**: Separação entre comandos (Create, Update, Delete) e consultas (Read)
- **MediatR**: Implementação do padrão mediator para comunicação entre camadas
- **Repository Pattern**: Abstração do acesso a dados
- **Unit of Work**: Gerenciamento de transações
- **Domain Events**: Eventos de domínio para comunicação assíncrona
- **Health Checks**: Monitoramento de saúde dos serviços

## 🛠️ Tecnologias

### Backend
- **.NET 9** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM para SQL Server
- **MongoDB.Driver** - Driver para MongoDB
- **MediatR** - Implementação do padrão mediator
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação de dados

> 🔧 **Detalhes Técnicos**: Para informações completas sobre tecnologias e dependências, veja [🛠️ Tecnologias](.doc/02-technologies.md)

### Bancos de Dados
- **SQL Server 2022** - Dados principais dos pedidos
- **MongoDB 7.0** - Armazenamento de itens dos pedidos
- **Redis 7.2** - Cache e sessões
- **RabbitMQ 3.12** - Mensageria e filas

### Infraestrutura
- **Docker & Docker Compose** - Containerização
- **JWT Bearer** - Autenticação
- **Swagger/OpenAPI** - Documentação da API
- **Health Checks** - Monitoramento de serviços

## 🚀 Como Executar

### Pré-requisitos
- Docker Desktop
- .NET 9 SDK
- Visual Studio 2022 ou VS Code

> 🚀 **Guia Completo**: Para instruções detalhadas de instalação e configuração, veja [🚀 Como Executar](.doc/03-getting-started.md)

### 1. Clone o repositório
```bash
git clone <repository-url>
cd venice-dev-challenge
```

### 2. Execute com Docker Compose
```bash
docker-compose up -d
```

Este comando irá:
- Iniciar SQL Server na porta 1433
- Iniciar MongoDB na porta 27017
- Iniciar Redis na porta 6379
- Iniciar RabbitMQ na porta 5672 (Management UI: 15672)
- Construir e executar a API na porta 5050

### 3. Acesse a aplicação
- **API**: http://localhost:5050
- **Swagger**: http://localhost:5050/swagger
- **RabbitMQ Management**: http://localhost:15672 (venice_user/VeniceMQ2024)

### 4. Executar localmente (opcional)
```bash
cd src
dotnet restore
dotnet run --project Venice.Orders.WebApi
```

## 📊 Estrutura do Banco de Dados

### SQL Server (Dados Principais)
- **Orders**: Informações básicas dos pedidos (ID, CustomerId, Status, TotalAmount)
- **Users**: Usuários do sistema

> 📊 **Estrutura Detalhada**: Para schemas completos, índices e configurações, veja [📊 Estrutura do Banco de Dados](.doc/05-database-structure.md)

### MongoDB (Itens dos Pedidos)
- **OrderItems**: Detalhes dos itens de cada pedido (ProductId, Quantity, UnitPrice)

### Redis
- Cache de sessões e dados frequentemente acessados

### RabbitMQ
- Filas para processamento assíncrono de pedidos

## 🔌 API Endpoints

### Autenticação
Todos os endpoints requerem autenticação JWT Bearer.

> 🔌 **Referência Completa**: Para documentação completa da API com exemplos e códigos de resposta, veja [🔌 API Reference](.doc/04-api-reference.md)

### Pedidos

#### POST /api/orders
Cria um novo pedido.

**Request Body:**
```json
{
  "customerId": "guid",
  "items": [
    {
      "productId": "guid",
      "quantity": 2,
      "unitPrice": 29.99
    }
  ]
}
```

**Response (201 Created):**
```json
{
  "id": "guid",
  "customerId": "guid",
  "createdAt": "2024-01-01T00:00:00Z",
  "status": "Pending",
  "totalAmount": 59.98,
  "items": [...]
}
```

#### GET /api/orders/{id}
Obtém um pedido específico por ID.

#### GET /api/orders
Lista todos os pedidos.

### Status dos Pedidos
- **Pending**: Pedido criado, aguardando confirmação
- **Confirmed**: Pedido confirmado, aguardando entrega
- **Delivered**: Pedido entregue
- **Cancelled**: Pedido cancelado

## 🔧 Configuração

### Variáveis de Ambiente
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=VeniceOrders;User Id=sa;Password=VeniceOrders@2024;TrustServerCertificate=true",
    "MongoConnection": "mongodb://mongodb:27017",
    "RedisConnection": "redis:6379",
    "RabbitMQConnection": "amqp://venice_user:VeniceMQ2024@rabbitmq:5672"
  }
}
```

### Health Checks
- **SQL Server**: Verificação de conectividade e migrações
- **MongoDB**: Ping do banco
- **Redis**: Ping do cache
- **RabbitMQ**: Ping da mensageria

## 🧪 Testes

```bash
cd src/tests
dotnet test
```

> 🧪 **Desenvolvimento**: Para padrões de desenvolvimento, testes e CI/CD, veja [🧪 Desenvolvimento e Testes](.doc/06-development-testing.md)

## 📁 Estrutura de Arquivos

```
src/
├── Venice.Orders.WebApi/
│   ├── Features/Orders/          # Controllers e ViewModels
│   ├── Configuration/            # Configurações da aplicação
│   ├── HealthChecks/            # Verificações de saúde
│   ├── Middleware/              # Middlewares customizados
│   └── Program.cs               # Ponto de entrada
├── Venice.Orders.Application/
│   ├── Orders/                  # Casos de uso dos pedidos
│   ├── Dtos/                    # Objetos de transferência
│   └── Interfaces/              # Contratos da aplicação
├── Venice.Orders.Domain/
│   ├── Entities/                # Entidades de domínio
│   ├── Repositories/            # Interfaces dos repositórios
│   ├── Events/                  # Eventos de domínio
│   └── Enums/                   # Enumerações
├── Venice.Orders.Infrastructure/
│   ├── Persistence/             # Implementações dos repositórios
│   ├── Services/                # Serviços de infraestrutura
│   └── Messaging/               # Implementações de mensageria
└── Venice.Orders.Common/
    ├── Extensions/              # Extensões de métodos
    ├── Filters/                 # Filtros customizados
    └── Middleware/              # Middlewares compartilhados
```

> 📚 **Documentação Completa**: Para detalhes sobre cada camada e padrões implementados, veja [🏗️ Arquitetura](.doc/01-architecture.md)

## 🚀 Deploy

### Docker
```bash
# Build da imagem
docker build -t venice-orders-api ./src

# Execução
docker run -p 5050:80 venice-orders-api
```

### Kubernetes
```bash
kubectl apply -f k8s/
```

## 📈 Monitoramento

- **Health Checks**: `/health` endpoint para verificação de saúde
- **Logs**: Logs estruturados com Serilog
- **Métricas**: Métricas básicas de performance

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 👥 Autores

- **Venice Dev Team** - Desenvolvimento inicial

## 🙏 Agradecimentos

- .NET Community
- Clean Architecture patterns
- CQRS community

---

**Venice Dev Challenge** - Sistema de Pedidos com Arquitetura Limpa 🚀
