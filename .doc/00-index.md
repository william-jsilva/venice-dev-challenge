# 📚 Índice da Documentação - Venice Dev Challenge

## 🚀 Visão Geral

Bem-vindo à documentação completa do **Venice Dev Challenge**! Este sistema de pedidos implementa uma arquitetura moderna com Clean Architecture, CQRS e múltiplas tecnologias de banco de dados.

## 📖 Documentos Disponíveis

### 1. 🏗️ [Arquitetura do Sistema](01-architecture.md)
- **Clean Architecture** e princípios SOLID
- **CQRS** e padrões implementados
- Estrutura de camadas e responsabilidades
- Fluxo de dados e comunicação
- Estratégias de escalabilidade e monitoramento

### 2. 🛠️ [Tecnologias e Dependências](02-technologies.md)
- **.NET 9** e ASP.NET Core
- Bancos de dados: SQL Server, MongoDB, Redis, RabbitMQ
- Pacotes NuGet e versões
- Ferramentas de desenvolvimento
- Compatibilidade e requisitos

### 3. 🚀 [Como Executar](03-getting-started.md)
- **Pré-requisitos** e instalação
- Execução com **Docker Compose**
- Configuração local
- Troubleshooting e monitoramento
- Desenvolvimento e debugging

### 4. 🔌 [API Reference](04-api-reference.md)
- **Endpoints** completos da API
- Autenticação JWT Bearer
- Exemplos de request/response
- Códigos de erro e validação
- Rate limiting e paginação

### 5. 📊 [Estrutura do Banco de Dados](05-database-structure.md)
- **Multi-database** strategy
- Schemas SQL Server e MongoDB
- Configuração Redis e RabbitMQ
- Sincronização e backup
- Monitoramento e métricas

### 6. 🧪 [Desenvolvimento e Testes](06-development-testing.md)
- **Padrões de desenvolvimento**
- Testes unitários e integração
- CI/CD e deploy
- Code quality e standards
- Contribuição e guidelines

## 🎯 Quick Start

### Para Desenvolvedores
1. **Clone o repositório**
2. **Leia** [Como Executar](03-getting-started.md)
3. **Execute** com Docker: `docker-compose up -d`
4. **Acesse** Swagger: http://localhost:5050/swagger

### Para Arquitetos
1. **Leia** [Arquitetura do Sistema](01-architecture.md)
2. **Analise** [Estrutura do Banco](05-database-structure.md)
3. **Entenda** os padrões implementados

### Para DevOps
1. **Configure** [Como Executar](03-getting-started.md)
2. **Monitore** com health checks
3. **Deploy** com Docker/Kubernetes

## 🔍 Navegação Rápida

### Por Tópico
- **🏗️ Arquitetura**: [01](01-architecture.md) | [02](02-technologies.md)
- **🚀 Execução**: [03](03-getting-started.md)
- **🔌 API**: [04](04-api-reference.md)
- **📊 Dados**: [05](05-database-structure.md)
- **🧪 Desenvolvimento**: [06](06-development-testing.md)

### Por Tecnologia
- **.NET**: [02](02-technologies.md) | [01](01-architecture.md)
- **Docker**: [03](03-getting-started.md)
- **SQL Server**: [05](05-database-structure.md)
- **MongoDB**: [05](05-database-structure.md)
- **Redis**: [05](05-database-structure.md)
- **RabbitMQ**: [05](05-database-structure.md)

## 📋 Checklist de Configuração

### ✅ Pré-requisitos
- [ ] Docker Desktop instalado
- [ ] .NET 9 SDK instalado
- [ ] Git configurado
- [ ] Portas disponíveis (1433, 27017, 6379, 5672, 5050)

### ✅ Execução
- [ ] Repositório clonado
- [ ] Docker Compose executado
- [ ] Serviços rodando
- [ ] Health checks passando
- [ ] Swagger acessível

### ✅ Desenvolvimento
- [ ] IDE configurado
- [ ] Dependências restauradas
- [ ] Testes executando
- [ ] Hot reload funcionando

## 🆘 Suporte

### Problemas Comuns
- **Porta em uso**: Ver [Troubleshooting](03-getting-started.md#🐛- troubleshooting)
- **Container não inicia**: Verificar logs com `docker-compose logs`
- **API não responde**: Verificar health checks em `/health`

### Recursos Adicionais
- **Swagger UI**: http://localhost:5050/swagger
- **RabbitMQ Management**: http://localhost:15672
- **Health Checks**: http://localhost:5050/health

## 🔄 Atualizações

### Versão Atual
- **Documentação**: v1.0.0
- **Projeto**: Venice Dev Challenge
- **Última atualização**: Janeiro 2024

### Histórico de Mudanças
- **v1.0.0**: Documentação inicial completa
- **v1.0.1**: Adicionados exemplos de código
- **v1.0.2**: Melhorias na navegação

## 📞 Contato

### Equipe de Desenvolvimento
- **Projeto**: Venice Dev Challenge
- **Arquitetura**: Clean Architecture + CQRS
- **Tecnologias**: .NET 9, Docker, Multi-Database

### Contribuição
1. **Fork** o projeto
2. **Crie** uma branch para sua feature
3. **Commit** suas mudanças
4. **Push** para a branch
5. **Abra** um Pull Request

---

**🎯 Comece por aqui**: [Como Executar](03-getting-started.md) para configurar o ambiente rapidamente!

**📚 Documentação completa**: Navegue pelos documentos usando os links acima ou o menu lateral.

**🚀 Pronto para começar?** Execute `docker-compose up -d` e acesse http://localhost:5050/swagger
