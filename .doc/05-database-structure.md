# 📊 Estrutura do Banco de Dados

## Visão Geral

O Venice Dev Challenge utiliza uma estratégia de **multi-database** para otimizar o armazenamento e consulta de dados, combinando as vantagens de bancos relacionais e NoSQL.

## 🏗️ Arquitetura de Dados

### Estratégia Multi-Database
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   SQL Server   │    │    MongoDB      │    │     Redis      │
│   (Relacional) │    │   (Documentos)  │    │    (Cache)      │
├─────────────────┤    ├─────────────────┤    ├─────────────────┤
│ - Orders       │    │ - OrderItems    │    │ - Sessions      │
│ - Users        │    │ - OrderHistory  │    │ - Cache         │
│ - Metadata     │    │ - Analytics     │    │ - Rate Limiting │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Princípios de Design
- **SQL Server**: Dados transacionais e relacionais
- **MongoDB**: Dados flexíveis e não estruturados
- **Redis**: Cache e sessões de alta performance
- **Separação de responsabilidades**: Cada banco para seu propósito específico

## 🗄️ SQL Server 2022

### Database: `VeniceOrders`

#### 1. Tabela: `Orders`
```sql
CREATE TABLE Orders (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    UpdatedAt DATETIME2 NULL,
    CancelledAt DATETIME2 NULL,
    Notes NVARCHAR(MAX) NULL,
    
    -- Constraints
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Users(Id),
    CONSTRAINT CK_Orders_Status CHECK (Status IN ('Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered', 'Cancelled')),
    CONSTRAINT CK_Orders_TotalAmount CHECK (TotalAmount >= 0)
);

-- Índices
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt);
CREATE INDEX IX_Orders_Status_CreatedAt ON Orders(Status, CreatedAt);
```

#### 2. Tabela: `Users`
```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    LastLoginAt DATETIME2 NULL,
    
    -- Constraints
    CONSTRAINT CK_Users_Email CHECK (Email LIKE '%_@_%._%')
);

-- Índices
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);
CREATE INDEX IX_Users_CreatedAt ON Users(CreatedAt);
```

#### 3. Tabela: `OrderStatusHistory`
```sql
CREATE TABLE OrderStatusHistory (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OrderId UNIQUEIDENTIFIER NOT NULL,
    PreviousStatus NVARCHAR(50) NULL,
    NewStatus NVARCHAR(50) NOT NULL,
    ChangedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ChangedBy UNIQUEIDENTIFIER NULL,
    Notes NVARCHAR(MAX) NULL,
    
    -- Constraints
    CONSTRAINT FK_OrderStatusHistory_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    CONSTRAINT FK_OrderStatusHistory_Users FOREIGN KEY (ChangedBy) REFERENCES Users(Id)
);

-- Índices
CREATE INDEX IX_OrderStatusHistory_OrderId ON OrderStatusHistory(OrderId);
CREATE INDEX IX_OrderStatusHistory_ChangedAt ON OrderStatusHistory(ChangedAt);
```

#### 4. Tabela: `Products`
```sql
CREATE TABLE Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    SKU NVARCHAR(50) UNIQUE NULL,
    Category NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL
);

-- Índices
CREATE INDEX IX_Products_Category ON Products(Category);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);
CREATE INDEX IX_Products_SKU ON Products(SKU);
```

### Relacionamentos
```sql
-- Diagrama de relacionamentos
Orders (1) ←→ (1) Users
Orders (1) ←→ (N) OrderStatusHistory
OrderStatusHistory (N) ←→ (1) Users
```

### Migrações
```bash
# Criar migração inicial
dotnet ef migrations add InitialCreate --project Venice.Orders.Infrastructure

# Aplicar migrações
dotnet ef database update --project Venice.Orders.Infrastructure

# Remover migração
dotnet ef migrations remove --project Venice.Orders.Infrastructure
```

## 🍃 MongoDB 7.0

### Database: `VeniceOrders`

#### 1. Coleção: `OrderItems`
```javascript
// Schema do OrderItem
{
  "_id": ObjectId("..."),
  "orderId": "123e4567-e89b-12d3-a456-426614174000",
  "productId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
  "productName": "Produto Exemplo",
  "quantity": 2,
  "unitPrice": 29.99,
  "totalPrice": 59.98,
  "discount": 0,
  "tax": 5.99,
  "createdAt": ISODate("2024-01-15T10:30:00Z"),
  "metadata": {
    "color": "Azul",
    "size": "M",
    "weight": 0.5
  }
}

// Índices
db.OrderItems.createIndex({ "orderId": 1 });
db.OrderItems.createIndex({ "productId": 1 });
db.OrderItems.createIndex({ "createdAt": 1 });
db.OrderItems.createIndex({ "orderId": 1, "productId": 1 });
```

#### 2. Coleção: `OrderHistory`
```javascript
// Schema do OrderHistory
{
  "_id": ObjectId("..."),
  "orderId": "123e4567-e89b-12d3-a456-426614174000",
  "eventType": "StatusChanged",
  "timestamp": ISODate("2024-01-15T11:00:00Z"),
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "data": {
    "previousStatus": "Pending",
    "newStatus": "Confirmed",
    "notes": "Pedido confirmado pelo cliente"
  },
  "metadata": {
    "ipAddress": "192.168.1.100",
    "userAgent": "Mozilla/5.0...",
    "sessionId": "session-123"
  }
}

// Índices
db.OrderHistory.createIndex({ "orderId": 1 });
db.OrderHistory.createIndex({ "eventType": 1 });
db.OrderHistory.createIndex({ "timestamp": 1 });
db.OrderHistory.createIndex({ "orderId": 1, "timestamp": 1 });
```

#### 3. Coleção: `ProductCatalog`
```javascript
// Schema do ProductCatalog
{
  "_id": ObjectId("..."),
  "productId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
  "name": "Produto Exemplo",
  "description": "Descrição detalhada do produto",
  "category": "Eletrônicos",
  "subcategory": "Smartphones",
  "brand": "Marca Exemplo",
  "specifications": {
    "color": ["Azul", "Preto", "Branco"],
    "size": ["S", "M", "L", "XL"],
    "weight": 0.5,
    "dimensions": {
      "length": 15.5,
      "width": 7.8,
      "height": 0.8
    }
  },
  "pricing": {
    "basePrice": 29.99,
    "salePrice": 24.99,
    "currency": "BRL",
    "taxRate": 0.20
  },
  "inventory": {
    "stock": 100,
    "reserved": 5,
    "available": 95
  },
  "isActive": true,
  "createdAt": ISODate("2024-01-01T00:00:00Z"),
  "updatedAt": ISODate("2024-01-15T10:30:00Z")
}

// Índices
db.ProductCatalog.createIndex({ "productId": 1 });
db.ProductCatalog.createIndex({ "category": 1 });
db.ProductCatalog.createIndex({ "brand": 1 });
db.ProductCatalog.createIndex({ "isActive": 1 });
db.ProductCatalog.createIndex({ "category": 1, "subcategory": 1 });
```

### Agregações MongoDB

#### 1. Total de Vendas por Categoria
```javascript
db.OrderItems.aggregate([
  {
    $lookup: {
      from: "ProductCatalog",
      localField: "productId",
      foreignField: "productId",
      as: "product"
    }
  },
  {
    $unwind: "$product"
  },
  {
    $group: {
      _id: "$product.category",
      totalSales: { $sum: "$totalPrice" },
      totalOrders: { $sum: 1 }
    }
  },
  {
    $sort: { totalSales: -1 }
  }
]);
```

#### 2. Histórico de Status de Pedido
```javascript
db.OrderHistory.aggregate([
  {
    $match: {
      orderId: "123e4567-e89b-12d3-a456-426614174000"
    }
  },
  {
    $sort: { timestamp: 1 }
  },
  {
    $project: {
      eventType: 1,
      timestamp: 1,
      status: "$data.newStatus",
      notes: "$data.notes"
    }
  }
]);
```

## 🔴 Redis 7.2

### Estrutura de Chaves

#### 1. Cache de Pedidos
```
# Pedido por ID
order:{orderId} → JSON do pedido completo
order:{orderId}:items → Lista de itens do pedido
order:{orderId}:customer → Dados do cliente

# Exemplo
order:123e4567-e89b-12d3-a456-426614174000 → {"id": "...", "status": "Pending", ...}
```

#### 2. Cache de Usuários
```
# Usuário por ID
user:{userId} → JSON do usuário
user:{userId}:orders → Lista de IDs de pedidos
user:{userId}:profile → Perfil completo

# Exemplo
user:550e8400-e29b-41d4-a716-446655440000 → {"id": "...", "name": "João Silva", ...}
```

#### 3. Cache de Produtos
```
# Produto por ID
product:{productId} → JSON do produto
product:{productId}:inventory → Estoque atual
product:{productId}:pricing → Preços e descontos

# Exemplo
product:6ba7b810-9dad-11d1-80b4-00c04fd430c8 → {"id": "...", "name": "Produto", ...}
```

#### 4. Sessões e Autenticação
```
# Sessão do usuário
session:{sessionId} → Dados da sessão
user:{userId}:sessions → Lista de sessões ativas
token:{tokenHash} → Dados do token JWT

# Exemplo
session:abc123 → {"userId": "...", "expiresAt": "...", "permissions": [...]}
```

#### 5. Rate Limiting
```
# Rate limit por IP
ratelimit:ip:{ipAddress} → Contador de requests
ratelimit:user:{userId} → Contador por usuário
ratelimit:endpoint:{endpoint} → Contador por endpoint

# Exemplo
ratelimit:ip:192.168.1.100 → 95
```

### Configuração Redis
```bash
# Configuração básica
redis-cli config set maxmemory 512mb
redis-cli config set maxmemory-policy allkeys-lru

# Configuração de persistência
redis-cli config set save "900 1 300 10 60 10000"
redis-cli config set appendonly yes
```

## 🐰 RabbitMQ 3.12

### Exchanges e Filas

#### 1. Exchange: `orders.direct`
```yaml
# Configuração
type: direct
durable: true
auto_delete: false

# Routing Keys
- order.created
- order.status.changed
- order.cancelled
- order.delivered
```

#### 2. Exchange: `orders.fanout`
```yaml
# Configuração
type: fanout
durable: true
auto_delete: false

# Usado para notificações broadcast
- order.notifications
- system.alerts
```

#### 3. Filas Principais
```yaml
# Fila de processamento de pedidos
queue: order.processing
durable: true
auto_delete: false
arguments:
  x-message-ttl: 300000  # 5 minutos
  x-dead-letter-exchange: orders.dlq

# Fila de notificações
queue: order.notifications
durable: true
auto_delete: false

# Fila de dead letter
queue: orders.dlq
durable: true
auto_delete: false
```

### Mensagens

#### 1. Order Created Event
```json
{
  "eventId": "event-123",
  "eventType": "OrderCreated",
  "timestamp": "2024-01-15T10:30:00Z",
  "orderId": "123e4567-e89b-12d3-a456-426614174000",
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "data": {
    "totalAmount": 109.97,
    "itemCount": 2,
    "status": "Pending"
  }
}
```

#### 2. Status Changed Event
```json
{
  "eventId": "event-124",
  "eventType": "OrderStatusChanged",
  "timestamp": "2024-01-15T11:00:00Z",
  "orderId": "123e4567-e89b-12d3-a456-426614174000",
  "data": {
    "previousStatus": "Pending",
    "newStatus": "Confirmed",
    "changedBy": "550e8400-e29b-41d4-a716-446655440000",
    "notes": "Pedido confirmado pelo cliente"
  }
}
```

## 🔄 Sincronização de Dados

### Estratégia de Consistência

#### 1. Eventual Consistency
- **SQL Server**: Fonte da verdade para dados transacionais
- **MongoDB**: Sincronizado via eventos de domínio
- **Redis**: Cache invalidado automaticamente
- **RabbitMQ**: Garantia de entrega de mensagens

#### 2. Padrão Saga
```csharp
// Exemplo de Saga para criação de pedido
public class CreateOrderSaga
{
    public async Task ExecuteAsync(CreateOrderCommand command)
    {
        // 1. Criar pedido no SQL Server
        var order = await _orderRepository.CreateAsync(command);
        
        // 2. Publicar evento
        await _eventBus.PublishAsync(new OrderCreatedEvent(order.Id));
        
        // 3. Processar itens no MongoDB
        await _orderItemService.CreateItemsAsync(order.Id, command.Items);
        
        // 4. Atualizar cache
        await _cacheService.InvalidateAsync($"order:{order.Id}");
    }
}
```

### Backup e Recuperação

#### 1. SQL Server
```sql
-- Backup completo
BACKUP DATABASE VeniceOrders TO DISK = 'C:\Backups\VeniceOrders.bak'

-- Backup diferencial
BACKUP DATABASE VeniceOrders TO DISK = 'C:\Backups\VeniceOrders_diff.bak' WITH DIFFERENTIAL

-- Restore
RESTORE DATABASE VeniceOrders FROM DISK = 'C:\Backups\VeniceOrders.bak'
```

#### 2. MongoDB
```bash
# Backup
mongodump --db VeniceOrders --out /backup/

# Restore
mongorestore --db VeniceOrders /backup/VeniceOrders/
```

#### 3. Redis
```bash
# Backup automático
redis-cli BGSAVE

# Backup manual
redis-cli SAVE
```

## 📊 Monitoramento e Métricas

### Health Checks
```csharp
// SQL Server Health Check
services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "sqlserver")
    .AddMongoDb(mongoConnectionString, name: "mongodb")
    .AddRedis(redisConnectionString, name: "redis")
    .AddRabbitMQ(rabbitConnectionString, name: "rabbitmq");
```

### Métricas de Performance
```sql
-- SQL Server Performance
SELECT 
    DB_NAME(database_id) as DatabaseName,
    COUNT(*) as NumberOfConnections,
    SUM(num_reads) as TotalReads,
    SUM(num_writes) as TotalWrites
FROM sys.dm_exec_sessions s
JOIN sys.dm_exec_connections c ON s.session_id = c.session_id
JOIN sys.dm_exec_requests r ON s.session_id = r.session_id
GROUP BY database_id;
```

---

**Anterior**: [API Reference](04-api-reference.md) | **Próximo**: [Desenvolvimento e Testes](06-development-testing.md)
