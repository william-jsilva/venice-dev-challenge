# 🚀 Como Executar o Projeto

## 📋 Pré-requisitos

### Software Necessário
- **Docker Desktop**: Versão 4.0+ (Windows/Mac) ou Docker Engine 20.10+ (Linux)
- **.NET 9 SDK**: Versão 9.0.100+
- **Git**: Versão 2.30+
- **Visual Studio 2022**: Versão 17.0+ (opcional)
- **VS Code**: Versão 1.80+ (opcional)

### Requisitos do Sistema
- **RAM**: Mínimo 8GB, recomendado 16GB+
- **CPU**: 4 cores ou mais
- **Storage**: 10GB de espaço livre
- **OS**: Windows 10/11, macOS 12+, Ubuntu 20.04+

## 🔧 Instalação

### 1. Docker Desktop
```bash
# Windows/Mac: Baixar do site oficial
# https://www.docker.com/products/docker-desktop

# Linux (Ubuntu)
sudo apt-get update
sudo apt-get install docker.io docker-compose
sudo usermod -aG docker $USER
```

### 2. .NET 9 SDK
```bash
# Windows: Baixar do site oficial
# https://dotnet.microsoft.com/download/dotnet/9.0

# macOS
brew install dotnet

# Linux (Ubuntu)
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0
```

### 3. Verificar Instalações
```bash
# Docker
docker --version
docker-compose --version

# .NET
dotnet --version

# Git
git --version
```

## 📥 Clonando o Repositório

### 1. Clone
```bash
git clone <repository-url>
cd venice-dev-challenge
```

### 2. Verificar Estrutura
```bash
ls -la
# Deve mostrar:
# - src/ (código fonte)
# - .doc/ (documentação)
# - docker-compose.yml
# - README.md
```

## 🐳 Executando com Docker

### 1. Iniciar Serviços
```bash
# Iniciar todos os serviços em background
docker-compose up -d

# Verificar status dos containers
docker-compose ps
```

### 2. Verificar Logs
```bash
# Logs de todos os serviços
docker-compose logs

# Logs de um serviço específico
docker-compose logs venice-orders-api
docker-compose logs sqlserver
```

### 3. Parar Serviços
```bash
# Parar todos os serviços
docker-compose down

# Parar e remover volumes
docker-compose down -v
```

## 🔍 Verificando Serviços

### 1. SQL Server
```bash
# Verificar container
docker ps | grep sqlserver

# Testar conexão
docker exec -it venice-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P VeniceOrders@2024 -Q "SELECT @@VERSION"
```

### 2. MongoDB
```bash
# Verificar container
docker ps | grep mongodb

# Acessar shell
docker exec -it venice-mongodb mongosh
```

### 3. Redis
```bash
# Verificar container
docker ps | grep redis

# Testar conexão
docker exec -it venice-redis redis-cli ping
```

### 4. RabbitMQ
```bash
# Verificar container
docker ps | grep rabbitmq

# Acessar Management UI
# http://localhost:15672
# Usuário: venice_user
# Senha: VeniceMQ2024
```

### 5. API
```bash
# Verificar container
docker ps | grep venice-orders-api

# Testar health check
curl http://localhost:5050/health
```

## 🚀 Executando Localmente

### 1. Configurar Variáveis de Ambiente
```bash
# Copiar arquivo de configuração
cp src/Venice.Orders.WebApi/appsettings.Development.json src/Venice.Orders.WebApi/appsettings.Local.json
```

**Editar `appsettings.Local.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=VeniceOrders;User Id=sa;Password=VeniceOrders@2024;TrustServerCertificate=true",
    "MongoConnection": "mongodb://localhost:27017",
    "RedisConnection": "localhost:6379",
    "RabbitMQConnection": "amqp://venice_user:VeniceMQ2024@localhost:5672"
  }
}
```

### 2. Executar Aplicação
```bash
cd src
dotnet restore
dotnet run --project Venice.Orders.WebApi --environment Local
```

### 3. Acessar Aplicação
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

## 🔧 Configuração Avançada

### 1. Variáveis de Ambiente
```bash
# Criar arquivo .env
cat > .env << EOF
SQLSERVER_PASSWORD=VeniceOrders@2024
MONGODB_PORT=27017
REDIS_PORT=6379
RABBITMQ_PORT=5672
API_PORT=5050
EOF
```

### 2. Docker Compose Personalizado
```bash
# Usar arquivo específico
docker-compose -f docker-compose.override.yml up -d

# Executar com variáveis específicas
SQLSERVER_PASSWORD=MinhaSenha123 docker-compose up -d
```

### 3. Portas Personalizadas
```yaml
# docker-compose.override.yml
version: '3.8'
services:
  sqlserver:
    ports:
      - "1434:1433"  # Porta personalizada
  
  mongodb:
    ports:
      - "27018:27017"  # Porta personalizada
```

## 🧪 Testando a Aplicação

### 1. Health Check
```bash
curl http://localhost:5050/health
```

**Resposta Esperada:**
```json
{
  "status": "Healthy",
  "checks": {
    "sqlserver": "Healthy",
    "mongodb": "Healthy",
    "redis": "Healthy",
    "rabbitmq": "Healthy"
  }
}
```

### 2. Swagger UI
- Acessar: http://localhost:5050/swagger
- Testar endpoints diretamente
- Verificar documentação da API

### 3. Testes Unitários
```bash
cd src/tests
dotnet test
```

## 🐛 Troubleshooting

### Problemas Comuns

#### 1. Porta já em uso
```bash
# Verificar portas em uso
netstat -an | findstr :5050  # Windows
lsof -i :5050                # Linux/Mac

# Parar processo
taskkill /PID <PID> /F       # Windows
kill -9 <PID>                # Linux/Mac
```

#### 2. Container não inicia
```bash
# Verificar logs
docker-compose logs <service-name>

# Verificar recursos
docker stats

# Reiniciar serviço
docker-compose restart <service-name>
```

#### 3. Problemas de Conexão
```bash
# Verificar rede Docker
docker network ls
docker network inspect venice-dev-challenge_venice_network

# Testar conectividade entre containers
docker exec venice-orders-api ping sqlserver
```

#### 4. Problemas de Permissão
```bash
# Linux: Ajustar permissões
sudo chown -R $USER:$USER .

# Windows: Executar como administrador
```

### Logs e Debug

#### 1. Habilitar Logs Verbosos
```bash
# Docker Compose
docker-compose up -d --verbose

# .NET
dotnet run --project Venice.Orders.WebApi --environment Development --verbosity detailed
```

#### 2. Logs de Container
```bash
# Logs em tempo real
docker-compose logs -f

# Logs com timestamp
docker-compose logs -t
```

## 📊 Monitoramento

### 1. Docker Stats
```bash
# Estatísticas em tempo real
docker stats

# Estatísticas de um container específico
docker stats venice-orders-api
```

### 2. Health Checks
```bash
# Verificar health de todos os serviços
curl http://localhost:5050/health

# Verificar health específico
curl http://localhost:5050/health/sqlserver
```

### 3. Métricas do Sistema
```bash
# Uso de CPU e memória
docker system df
docker system prune
```

## 🔄 Desenvolvimento

### 1. Hot Reload
```bash
# .NET Hot Reload
dotnet watch --project Venice.Orders.WebApi

# Docker com volumes
docker-compose -f docker-compose.dev.yml up -d
```

### 2. Debugging
```bash
# Visual Studio Code
code .

# Visual Studio
start Venice.Orders.sln
```

### 3. Database Migrations
```bash
# Aplicar migrações
dotnet ef database update --project Venice.Orders.Infrastructure

# Criar nova migração
dotnet ef migrations add InitialCreate --project Venice.Orders.Infrastructure
```

---

**Anterior**: [Tecnologias e Dependências](02-technologies.md) | **Próximo**: [API Reference](04-api-reference.md)
