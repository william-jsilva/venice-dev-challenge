# 🗄️ Arquitetura de Banco de Dados - Venice Orders

## 📋 Visão Geral

O projeto Venice Orders implementa um **armazenamento híbrido** conforme especificado no desafio, utilizando dois bancos de dados diferentes para otimizar performance e escalabilidade.

## 🏗️ Arquitetura Híbrida

### **SQL Server** - Dados Principais dos Pedidos
- **Tabela**: `Orders`
- **Dados armazenados**:
  - `Id` (Guid) - Chave primária
  - `CustomerId` (Guid) - ID do cliente
  - `CreatedAt` (DateTime) - Data de criação
  - `Status` (OrderStatus) - Status do pedido
  - `TotalAmount` (decimal) - Valor total do pedido

### **MongoDB** - Itens dos Pedidos
- **Collection**: `OrderItems`
- **Dados armazenados**:
  - `Id` (Guid) - Chave primária
  - `OrderId` (Guid) - Referência ao pedido
  - `ProductName` (string) - Nome do produto
  - `Quantity` (int) - Quantidade
  - `UnitPrice` (decimal) - Preço unitário
  - `TotalPrice` (decimal) - Preço total do item

## 🔄 Fluxo de Dados

### **Criação de Pedido**
```
1. Recebe dados do pedido via API
2. Cria entidade Order (dados principais)
3. Cria entidades OrderItem (itens)
4. Calcula TotalAmount
5. Salva Order no SQL Server
6. Salva OrderItems no MongoDB
7. Publica evento OrderCreatedEvent
8. Invalida cache
```

### **Consulta de Pedido**
```
1. Verifica cache Redis
2. Se não encontrado:
   - Busca Order no SQL Server
   - Busca OrderItems no MongoDB
   - Combina os dados
   - Salva no cache por 2 minutos
3. Retorna dados integrados
```

## 🛠️ Implementação Técnica

### **Entity Framework (SQL Server)**
```csharp
public class VeniceOrdersContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            
            // Ignorar Items pois será salvo no MongoDB
            entity.Ignore(e => e.Items);
        });
    }
}
```

### **MongoDB Driver**
```csharp
public class OrderItemRepository : IOrderItemRepository
{
    private readonly IMongoCollection<OrderItem> _collection;

    public OrderItemRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<OrderItem>("OrderItems");
    }
    
    // Implementação dos métodos CRUD
}
```

### **Entidade Order (Domain)**
```csharp
public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    
    // NotMapped: Os itens são salvos no MongoDB
    [NotMapped]
    public List<OrderItem> Items { get; set; } = new();
}
```

## 📊 Vantagens da Arquitetura Híbrida

### **Performance**
- **SQL Server**: Otimizado para consultas estruturadas e relacionamentos
- **MongoDB**: Otimizado para documentos flexíveis e consultas de itens

### **Escalabilidade**
- **SQL Server**: Escala verticalmente para dados principais
- **MongoDB**: Escala horizontalmente para grandes volumes de itens

### **Flexibilidade**
- **SQL Server**: Garantia de ACID para dados críticos
- **MongoDB**: Flexibilidade de schema para itens de pedido

### **Manutenibilidade**
- Separação clara de responsabilidades
- Facilita otimizações independentes
- Permite evolução independente dos schemas

## 🔧 Configuração

### **Connection Strings**
```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=VeniceOrders;...",
    "MongoDB": "mongodb://localhost:27017"
  },
  "MongoDB": {
    "DatabaseName": "VeniceOrders"
  }
}
```

### **Docker Compose**
```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "1433:1433"
      
  mongodb:
    image: mongo:7.0
    ports:
      - "27017:27017"
```

## 🧪 Testes

### **Testes Unitários**
- ✅ Testes de entidades de domínio
- ✅ Testes de handlers de comando
- ✅ Testes de integração com mocks

### **Testes de Integração**
- ✅ Criação de pedido com armazenamento híbrido
- ✅ Consulta de pedido com dados integrados
- ✅ Cache Redis funcionando
- ✅ Eventos publicados no RabbitMQ

## 📈 Monitoramento

### **Métricas Importantes**
- Tempo de resposta das consultas
- Performance do cache Redis
- Latência entre SQL Server e MongoDB
- Volume de dados em cada banco

### **Logs**
- Operações de criação/atualização
- Consultas aos bancos de dados
- Performance do cache
- Eventos publicados

## 🚀 Próximos Passos

- [ ] Implementar índices otimizados no MongoDB
- [ ] Adicionar métricas de performance
- [ ] Implementar backup automático
- [ ] Adicionar health checks para ambos os bancos
- [ ] Implementar retry policies para operações críticas
- [ ] Adicionar monitoramento de queries lentas

