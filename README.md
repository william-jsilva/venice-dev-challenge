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

- **Docker Desktop** instalado e rodando
- **Docker Compose** (incluído no Docker Desktop)
- **Git** para clonar o repositório
- **.NET 9.0 SDK** (para desenvolvimento local)

### 🗄️ Migrações do Banco de Dados

O projeto usa **Entity Framework Core Migrations** para gerenciar o esquema do banco de dados SQL Server.

#### Aplicar Migrações (Desenvolvimento Local)
```powershell
# Windows
cd src\Venice.Orders.Infrastructure
dotnet ef database update --startup-project ..\Venice.Orders.WebApi

# Linux/macOS
cd src/Venice.Orders.Infrastructure
dotnet ef database update --startup-project ../Venice.Orders.WebApi
```

#### Criar Nova Migração
```powershell
# Windows
cd src\Venice.Orders.Infrastructure
dotnet ef migrations add "NomeDaMigracao" --startup-project ..\Venice.Orders.WebApi

# Linux/macOS
cd src/Venice.Orders.Infrastructure
dotnet ef migrations add "NomeDaMigracao" --startup-project ../Venice.Orders.WebApi
```

**📚 Documentação Completa**: [MIGRATIONS_README.md](./MIGRATIONS_README.md) | [EF_MIGRATIONS_GUIDE.md](./EF_MIGRATIONS_GUIDE.md)

### 🐳 Execução com Docker (Passo a Passo)

#### **Passo 1: Preparar o Ambiente**
```bash
# 1. Clone o repositório
git clone <repository-url>
cd venice-dev-challenge

# 2. Verificar se o Docker está rodando
docker --version
docker-compose --version
```

#### **Passo 2: Execução com Docker**

```bash
# 1. Parar containers existentes (se houver)
docker-compose down

# 2. Build e iniciar todos os serviços
docker-compose up --build -d

# 3. Verificar se os containers estão rodando
docker-compose ps

# 4. Ver logs em tempo real (opcional)
docker-compose logs -f
```

#### **Passo 3: Verificar se a Aplicação Está Funcionando**

```bash
# Verificar health check da aplicação
curl http://localhost:5000/health/live

# Verificar status completo
curl http://localhost:5000/health

# Abrir no navegador
# http://localhost:5000/swagger
```

### 📋 Comandos Úteis do Docker

```bash
# Ver containers rodando
docker-compose ps

# Ver logs de um serviço específico
docker-compose logs venice-orders-api
docker-compose logs sqlserver
docker-compose logs mongodb

# Ver logs em tempo real
docker-compose logs -f

# Parar todos os serviços
docker-compose down

# Parar e remover volumes (dados)
docker-compose down -v

# Rebuild e reiniciar
docker-compose up --build -d

# Executar comando em um container
docker-compose exec venice-orders-api bash
```

### 🔍 Troubleshooting

#### **Problema: Porta já em uso**
```bash
# Verificar o que está usando a porta 5000
netstat -ano | findstr :5000  # Windows
lsof -i :5000                 # Linux/Mac

# Parar o processo ou usar porta diferente no docker-compose.yml
```

#### **Problema: Containers não iniciam**
```bash
# Verificar logs detalhados
docker-compose logs

# Verificar se há conflitos de rede
docker network ls
docker network prune
```

#### **Problema: Banco de dados não conecta**
```bash
# Verificar se o SQL Server está pronto
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT 1"

# Verificar se o MongoDB está pronto
docker-compose exec mongodb mongosh --eval "db.adminCommand('ping')"
```

### Execução Local (Desenvolvimento)

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

O sistema utiliza autenticação JWT com usuários armazenados no banco de dados SQL Server.

### Inicializar Dados de Teste
```bash
# Criar usuário admin padrão
curl -X POST http://localhost:5000/api/auth/init-test-data
```

### Registrar Novo Usuário
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "novousuario",
    "email": "usuario@exemplo.com",
    "password": "senha123"
  }'
```

### Fazer Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "password"
  }'
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

### Auth

#### POST /api/auth/register
Registra um novo usuário.

**Body:**
```json
{
  "username": "novousuario",
  "password": "senha123",
  "email": "usuario@exemplo.com"
}
```

#### POST /api/auth/login
Faz login de um usuário.

**Body:**
```json
{
  "username": "admin",
  "password": "password"
}
```

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

#### GET /api/orders/{id}
Busca um pedido pelo ID.

**Headers:**
```
Authorization: Bearer {token}
```

## 🧪 Testes

### Testes Unitários

Execute os testes unitários:

```bash
cd src
dotnet test
```

### Testes de Integração

Para testar se a API está funcionando corretamente, você pode usar ferramentas como Postman, curl ou qualquer cliente HTTP de sua preferência.

Teste os seguintes endpoints:
- ✅ Health checks da aplicação
- ✅ Registro de novos usuários
- ✅ Login de usuários
- ✅ Criação de pedidos
- ✅ Busca de pedidos
- ✅ Health checks específicos

## 📊 Monitoramento

### Endpoints Disponíveis

- **API Principal**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`
- **RabbitMQ Management**: `http://localhost:15672` (guest/guest)

### Health Checks

- **Status Completo**: `http://localhost:5000/health`
- **Readiness**: `http://localhost:5000/health/ready`
- **Liveness**: `http://localhost:5000/health/live`
- **Status via API**: `http://localhost:5000/api/health/status`
- **Informações do Sistema**: `http://localhost:5000/api/health/info`

### Inicialização Manual

Para inicializar o projeto manualmente:

- ✅ Verificar se o Docker está rodando
- ✅ Parar containers existentes: `docker-compose down`
- ✅ Build e inicialização: `docker-compose up --build -d`
- ✅ Aguardar serviços ficarem prontos
- ✅ Verificar saúde da API: `curl http://localhost:5000/health/live`
- ✅ Acessar Swagger UI: `http://localhost:5000/swagger`

## 🔧 Correções Implementadas

### Problemas Resolvidos

1. **CreateOrder não funcionando**: Corrigido mapeamento do AutoMapper e lógica de criação de pedidos
2. **Falta de rota de registro**: Adicionada rota `POST /api/auth/register` no AuthController
3. **Health checks do Docker**: Corrigidos comandos de health check para RabbitMQ, SQL Server e API

### Arquivos Modificados

- `src/Venice.Orders.WebApi/Features/Orders/CreateOrder/CreateOrderProfile.cs` - Mapeamento AutoMapper simplificado
- `src/Venice.Orders.Application/Orders/CreateOrder/CreateOrderCommand.cs` - Construtor removido
- `src/Venice.Orders.Application/Orders/CreateOrder/CreateOrderHandler.cs` - Lógica de criação melhorada
- `src/Venice.Orders.WebApi/Features/Auth/AuthController.cs` - Rota de registro adicionada
- `docker-compose.yml` - Health checks corrigidos

Para mais detalhes, consulte o arquivo `CORREÇÕES_IMPLEMENTADAS.md`.

## 🔧 Configuração

### Configuração Automática (Docker)

Quando executado com Docker, a aplicação usa automaticamente:

- **SQL Server**: `sqlserver:1433` (usuário: `sa`, senha: `YourStrong@Passw0rd`)
- **MongoDB**: `mongodb:27017`
- **Redis**: `redis:6379`
- **RabbitMQ**: `rabbitmq:5672` (usuário: `guest`, senha: `guest`)

### Configuração Local (Desenvolvimento)

As configurações estão no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=VeniceOrders;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True",
    "MongoDB": "mongodb://localhost:27017",
    "Redis": "localhost:6379",
    "RabbitMQ": "amqp://guest:guest@localhost:5672"
  },
  "Jwt": {
    "Key": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "VeniceOrders",
    "Audience": "VeniceOrders"
  }
}
```

### Configuração Docker

Para Docker, use o arquivo `appsettings.Docker.json` que contém as configurações otimizadas para containers.

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




