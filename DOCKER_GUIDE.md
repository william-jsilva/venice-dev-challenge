# 🐳 Guia de Execução com Docker - Venice Orders

## 📋 Pré-requisitos

### 1. Instalar Docker Desktop
- **Windows**: [Docker Desktop for Windows](https://docs.docker.com/desktop/install/windows/)
- **Mac**: [Docker Desktop for Mac](https://docs.docker.com/desktop/install/mac/)
- **Linux**: [Docker Engine](https://docs.docker.com/engine/install/)

### 2. Verificar Instalação
```bash
docker --version
docker-compose --version
```

### 3. Iniciar Docker Desktop
- Windows/Mac: Abrir Docker Desktop
- Linux: `sudo systemctl start docker`

## 🚀 Execução Rápida

### Comandos Manuais

```bash
# 1. Clonar repositório (se ainda não fez)
git clone <repository-url>
cd venice-dev-challenge

# 2. Parar containers existentes
docker-compose down

# 3. Build e iniciar
docker-compose up --build -d

# 4. Verificar status
docker-compose ps
```

## 📊 Verificação da Aplicação

### 1. Health Checks
```bash
# Verificar se a aplicação está viva
curl http://localhost:5000/health/live

# Verificar status completo
curl http://localhost:5000/health

# Verificar readiness
curl http://localhost:5000/health/ready
```

### 2. Endpoints Disponíveis
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

### 3. Verificar Containers
```bash
# Ver todos os containers
docker-compose ps

# Ver logs da aplicação
docker-compose logs venice-orders-api

# Ver logs de todos os serviços
docker-compose logs
```

## 🔧 Comandos Úteis

### Gerenciamento de Containers
```bash
# Parar todos os serviços
docker-compose down

# Parar e remover volumes (dados)
docker-compose down -v

# Rebuild e reiniciar
docker-compose up --build -d

# Ver logs em tempo real
docker-compose logs -f

# Ver logs de serviço específico
docker-compose logs sqlserver
docker-compose logs mongodb
docker-compose logs redis
docker-compose logs rabbitmq
```

### Debugging
```bash
# Executar comando em container
docker-compose exec venice-orders-api bash
docker-compose exec sqlserver bash
docker-compose exec mongodb mongosh

# Verificar rede Docker
docker network ls
docker network inspect venice-dev-challenge_venice_network
```

### Limpeza
```bash
# Remover containers parados
docker container prune

# Remover imagens não utilizadas
docker image prune

# Remover volumes não utilizados
docker volume prune

# Limpeza completa
docker system prune -a
```

## 🚨 Troubleshooting

### Problema: Docker não inicia
```bash
# Verificar se o Docker está rodando
docker info

# Reiniciar Docker Desktop
# Windows/Mac: Reiniciar Docker Desktop
# Linux: sudo systemctl restart docker
```

### Problema: Porta 5000 em uso
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5000
kill -9 <PID>

# Ou alterar porta no docker-compose.yml
# ports:
#   - "5001:80"  # Usar porta 5001
```

### Problema: Containers não iniciam
```bash
# Verificar logs detalhados
docker-compose logs

# Verificar se há conflitos
docker-compose down
docker system prune
docker-compose up --build
```

### Problema: Banco de dados não conecta
```bash
# Verificar SQL Server
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT 1"

# Verificar MongoDB
docker-compose exec mongodb mongosh --eval "db.adminCommand('ping')"

# Verificar Redis
docker-compose exec redis redis-cli ping

# Verificar RabbitMQ
curl -u guest:guest http://localhost:15672/api/overview
```

### Problema: Aplicação não responde
```bash
# Verificar se todos os serviços estão saudáveis
docker-compose ps

# Verificar logs da aplicação
docker-compose logs venice-orders-api

# Verificar health checks
curl http://localhost:5000/health
```

## 📋 Configurações dos Serviços

### SQL Server
- **Porta**: 1433
- **Usuário**: sa
- **Senha**: YourStrong@Passw0rd
- **Database**: VeniceOrders

### MongoDB
- **Porta**: 27017
- **Database**: VeniceOrders
- **Sem autenticação** (desenvolvimento)

### Redis
- **Porta**: 6379
- **Sem autenticação** (desenvolvimento)

### RabbitMQ
- **Porta AMQP**: 5672
- **Porta Management**: 15672
- **Usuário**: guest
- **Senha**: guest

### Venice Orders API
- **Porta**: 5000
- **Ambiente**: Docker
- **Health Check**: http://localhost:5000/health

## 🔄 Fluxo de Inicialização

1. **Docker Compose** inicia todos os serviços
2. **Health Checks** verificam se cada serviço está pronto
3. **Aplicação** aguarda todos os serviços ficarem saudáveis
4. **Banco de dados** é criado automaticamente
5. **API** fica disponível em http://localhost:5000

## 📝 Logs Importantes

### Logs de Sucesso
```
✅ Docker is running
✅ Application is ready!
🎉 Venice Orders Application is ready!
```

### Logs de Erro Comuns
```
❌ Docker is not running
❌ API is not responding
❌ Connection failed
```

## 🆘 Suporte

Se encontrar problemas:

1. **Verificar logs**: `docker-compose logs`
2. **Reiniciar**: `docker-compose down && docker-compose up --build`
3. **Limpar cache**: `docker system prune`
4. **Verificar recursos**: CPU, RAM, disco
5. **Verificar rede**: Firewall, proxy, VPN
