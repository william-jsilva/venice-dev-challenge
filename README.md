# Venice Orders - Sistema de Gerenciamento de Pedidos

## 📋 Descrição

O Venice Orders é um microserviço de gerenciamento de pedidos desenvolvido para integrar com plataformas parceiras. O sistema recebe pedidos via API REST, armazena dados em bancos híbridos (SQL Server + MongoDB) e comunica com sistemas externos através de mensageria assíncrona.

## 🏗️ Arquitetura

### Padrão Arquitetural: Clean Architecture + CQRS

O projeto segue os princípios da **Clean Architecture** combinada com **CQRS (Command Query Responsibility Segregation)** e **DDD (Domain-Driven Design)**. Esta escolha foi baseada nos seguintes fatores:

#### Justificativa da Arquitetura

1. **Separação de Responsabilidades**: A Clean Architecture separa claramente as camadas de domínio, aplicação, infraestrutura e apresentação
2. **Independência de Frameworks**: O domínio não depende de frameworks externos, facilitando testes e manutenção
3. **CQRS**: Separação entre comandos (write) e queries (read) para otimizar performance e escalabilidade
4. **DDD**: Modelagem focada no domínio de negócio, com entidades e agregados bem definidos

### Estrutura do Projeto

```
src/
├── Venice.Orders.Domain/          # Camada de Domínio (Entidades, Interfaces)
├── Venice.Orders.Application/     # Camada de Aplicação (CQRS, DTOs)
├── Venice.Orders.Infrastructure/  # Camada de Infraestrutura (Repositórios, Serviços)
├── Venice.Orders.WebApi/          # Camada de Apresentação (Controllers, Middleware)
└── Venice.Orders.Common/          # Modelos compartilhados
```

## 🚀 Como Executar

### Pré-requisitos

- Docker Desktop
- .NET 9.0 SDK (para desenvolvimento local)
- PowerShell (para gerar certificado SSL)

### 1. Configuração SSL (HTTPS)

Para habilitar HTTPS, execute o script de geração de certificado:

```powershell
# Execute como Administrador
# (Certificado SSL não configurado - HTTPS desabilitado)
```

### 2. Executar com Docker

```bash
# Construir e iniciar todos os serviços
docker-compose up --build

# Executar em background
docker-compose up -d --build
```

### 3. Acessar a API

#### **Desenvolvimento Local**
- **HTTP**: http://localhost:7050
- **HTTPS**: https://localhost:7051
- **Swagger UI**: http://localhost:7050/swagger ou https://localhost:7051/swagger
- **Health Check**: http://localhost:7050/health

#### **Docker**
- **HTTP**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

### 4. Serviços Disponíveis

#### **Desenvolvimento Local**
- **API**: http://localhost:7050 (HTTP) / https://localhost:7051 (HTTPS)
- **Swagger UI**: http://localhost:7050/swagger

#### **Docker**
- **API**: http://localhost:5000 (HTTP)
- **Swagger UI**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433
- **MongoDB**: localhost:27017
- **Redis**: localhost:6379
- **RabbitMQ**: localhost:5672
- **RabbitMQ Management**: http://localhost:15672

## 🔐 Autenticação

### Credenciais

- **RabbitMQ**: `venice_user` / `VeniceMQ2024`
- **SQL Server**: `sa` / `VeniceOrders@2024`

### Obter Token JWT

```bash
# Registrar usuário
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "admin123"}'

# Fazer login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "admin123"}'
```

## 📡 Endpoints da API

### Autenticação
- `POST /api/auth/register` - Registrar usuário
- `POST /api/auth/login` - Fazer login

### Pedidos
- `POST /api/orders` - Criar pedido (requer autenticação)
- `GET /api/orders/{id}` - Buscar pedido por ID (requer autenticação)

### Health Checks
- `GET /health` - Status geral da aplicação
- `GET /health/ready` - Status dos serviços externos
- `GET /health/live` - Status da aplicação

## 🗄️ Armazenamento Híbrido

### SQL Server
- Dados principais dos pedidos (ID, ClienteID, Data, Status, TotalAmount)
- Tabelas: Orders, Users

### MongoDB
- Lista de itens dos pedidos (produto, quantidade, preço unitário)
- Coleção: OrderItems

## 🔄 Mensageria

### RabbitMQ
- Evento: `OrderCreatedEvent`
- Exchange: `venice.orders`
- Routing Key: `order.created`

## 🧪 Testes

```bash
# Executar testes unitários
cd src
dotnet test
```

## 📊 Monitoramento

### Health Checks
- **Application**: Status da aplicação
- **External Services**: Status dos serviços externos
- **SQL Server**: Conexão com banco de dados
- **MongoDB**: Conexão com MongoDB
- **Redis**: Conexão com cache
- **Entity Framework**: Status do contexto

## 🔧 Configurações

### Variáveis de Ambiente
- `ASPNETCORE_ENVIRONMENT`: Docker
- `ASPNETCORE_URLS`: http://+:80;https://+:443

### Certificado SSL
- **Arquivo**: `certs/venice-orders.pfx`
- **Senha**: `VeniceOrders2024`
- **Validade**: 1 ano

## 🐛 Troubleshooting

### Problemas Comuns

1. **Erro de CORS**: Verificar configuração CORS no `appsettings.Docker.json`
2. **Certificado SSL**: Executar `generate-cert.ps1` como Administrador
3. **Health Check falhando**: Aguardar inicialização completa dos serviços
4. **Migrations**: Executadas automaticamente no Docker

### Logs

```bash
# Ver logs da API
docker-compose logs venice-orders-api

# Ver logs de todos os serviços
docker-compose logs

# Ver logs em tempo real
docker-compose logs -f
```

## 📝 Notas de Implementação

- **Cache Redis**: Configurado para 2 minutos
- **Migrations**: Aplicadas automaticamente no Docker
- **CORS**: Configurado para permitir todas as origens
- **HTTPS**: Habilitado com certificado autoassinado
- **Health Checks**: Implementados para todos os serviços

## 🎯 Funcionalidades Implementadas

✅ Endpoint REST para criação de pedido  
✅ Armazenamento híbrido (SQL Server + MongoDB)  
✅ Publicação em fila RabbitMQ  
✅ Endpoint GET /pedidos/{id}  
✅ Cache Redis para GET  
✅ Testes unitários  
✅ Boas práticas (DDD, SOLID, Clean Architecture)  
✅ Autenticação JWT obrigatória  
✅ Docker Compose funcional  
✅ HTTPS habilitado  
✅ Health Checks completos  
✅ CORS configurado  
✅ Migrations automáticas  

## 📞 Suporte

Para dúvidas ou problemas, consulte os logs do Docker ou execute os health checks para diagnosticar problemas de conectividade.
