# Guia de Migrações do Entity Framework

Este guia explica como usar as migrações do Entity Framework Core para gerenciar o esquema do banco de dados SQL Server no projeto Venice Orders.

## 📋 Pré-requisitos

- .NET 9.0 SDK instalado
- SQL Server configurado e acessível
- String de conexão configurada no `appsettings.json`

## 🚀 Comandos Básicos

### Comandos Manuais

Se preferir usar os comandos diretamente:

```bash
# Navegar para o projeto de infraestrutura
cd src/Venice.Orders.Infrastructure

# Criar nova migração
dotnet ef migrations add "NomeDaMigracao" --startup-project ../Venice.Orders.WebApi

# Aplicar migrações
dotnet ef database update --startup-project ../Venice.Orders.WebApi

# Listar migrações
dotnet ef migrations list --startup-project ../Venice.Orders.WebApi

# Remover última migração
dotnet ef migrations remove --startup-project ../Venice.Orders.WebApi
```

## 📁 Estrutura das Migrações

As migrações são armazenadas em:
```
src/Venice.Orders.Infrastructure/Migrations/
├── [Timestamp]_[NomeDaMigracao].cs          # Migração principal
├── [Timestamp]_[NomeDaMigracao].Designer.cs # Designer da migração
└── VeniceOrdersContextModelSnapshot.cs      # Snapshot do modelo
```

## 🔄 Fluxo de Trabalho

### 1. Desenvolvimento
1. Faça alterações nas entidades do domínio
2. Crie uma nova migração: `dotnet ef migrations add "DescricaoDaAlteracao" --startup-project ../Venice.Orders.WebApi`
3. Revise o arquivo de migração gerado
4. Aplique a migração: `dotnet ef database update --startup-project ../Venice.Orders.WebApi`

### 2. Produção
- As migrações são aplicadas automaticamente quando o container Docker é iniciado
- O código no `Program.cs` chama `context.Database.Migrate()` em ambiente Docker

## 📊 Tabelas Criadas

### Orders
```sql
CREATE TABLE [Orders] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
);
```

**Nota**: Os itens do pedido (`OrderItem`) são armazenados no MongoDB, não no SQL Server.

## ⚠️ Importante

- **Nunca edite migrações já aplicadas** em produção
- **Sempre teste migrações** em ambiente de desenvolvimento
- **Faça backup** do banco antes de aplicar migrações em produção
- **Use nomes descritivos** para as migrações

## 🐛 Solução de Problemas

### Erro: "No database provider has been configured"
- Verifique se a string de conexão está configurada
- Confirme se o SQL Server está acessível

### Erro: "Migration already applied"
- Use `dotnet ef database update` para aplicar migrações pendentes
- Verifique o histórico de migrações com `dotnet ef migrations list`

### Erro: "Cannot create migration"
- Verifique se há erros de compilação no projeto
- Confirme se o DbContext está configurado corretamente

## 🔧 Configuração

### String de Conexão (appsettings.json)
```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=VeniceOrders;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Docker (appsettings.Docker.json)
```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=sqlserver;Database=VeniceOrders;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;"
  }
}
```

## 📚 Comandos Úteis

```bash
# Gerar script SQL das migrações (sem aplicar)
dotnet ef migrations script --startup-project ../Venice.Orders.WebApi

# Gerar script SQL de uma migração específica
dotnet ef migrations script --from [MigrationName] --startup-project ../Venice.Orders.WebApi

# Remover todas as migrações e recriar
dotnet ef migrations remove --startup-project ../Venice.Orders.WebApi
dotnet ef migrations add InitialCreate --startup-project ../Venice.Orders.WebApi
```

## 🎯 Boas Práticas

1. **Uma migração por alteração**: Não combine múltiplas alterações em uma única migração
2. **Nomes descritivos**: Use nomes que descrevam claramente a alteração
3. **Teste sempre**: Teste migrações em ambiente de desenvolvimento
4. **Versionamento**: Mantenha as migrações no controle de versão
5. **Reversibilidade**: Considere como reverter a migração se necessário
