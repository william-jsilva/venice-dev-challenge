# 🚀 Início Rápido - Venice Orders

## ⚡ Execução em 3 Passos

### 1️⃣ **Pré-requisitos**
- ✅ Docker Desktop instalado e rodando
- ✅ Git instalado

### 2️⃣ **Clonar e Executar**
```bash
# Clone o repositório
git clone <repository-url>
cd venice-dev-challenge

# Parar containers existentes (se houver)
docker-compose down

# Build e iniciar todos os serviços
docker-compose up --build -d

# Verificar se os containers estão rodando
docker-compose ps
```

### 3️⃣ **Acessar a Aplicação**
- 🌐 **API**: http://localhost:5000
- 📚 **Swagger**: http://localhost:5000/swagger
- 💚 **Health Check**: http://localhost:5000/health
- 🐰 **RabbitMQ**: http://localhost:15672 (guest/guest)

## 🔧 Comandos Essenciais

```bash
# Iniciar aplicação
docker-compose up --build -d

# Ver status
docker-compose ps

# Ver logs
docker-compose logs -f

# Parar aplicação
docker-compose down

# Rebuild completo
docker-compose down -v
docker-compose up --build -d
```

## 🚨 Problemas Comuns

### Docker não inicia
```bash
# Verificar se está rodando
docker info
```

### Porta 5000 ocupada
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5000
kill -9 <PID>
```

### Aplicação não responde
```bash
# Verificar health check
curl http://localhost:5000/health/live

# Ver logs
docker-compose logs venice-orders-api
```

## 📋 O que está incluído

- ✅ **SQL Server** - Banco de dados principal
- ✅ **MongoDB** - Armazenamento de documentos
- ✅ **Redis** - Cache em memória
- ✅ **RabbitMQ** - Mensageria
- ✅ **Health Checks** - Monitoramento automático
- ✅ **Swagger** - Documentação da API
- ✅ **Comandos manuais** - Inicialização via Docker Compose

## 🎯 Próximos Passos

1. **Testar a API**: Acesse http://localhost:5000/swagger
2. **Criar um pedido**: Use o endpoint POST /api/orders
3. **Verificar health**: Acesse http://localhost:5000/health
4. **Monitorar logs**: `docker-compose logs -f`

## 📞 Suporte

- 📖 **Documentação completa**: [README.md](README.md)
- 🐳 **Guia Docker**: [DOCKER_GUIDE.md](DOCKER_GUIDE.md)
- 🧪 **Exemplos de teste**: [HEALTH_CHECKS_EXAMPLES.md](HEALTH_CHECKS_EXAMPLES.md)

---

**🎉 Pronto! Sua aplicação Venice Orders está rodando!**
