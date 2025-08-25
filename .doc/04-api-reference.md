# 🔌 API Reference

## Visão Geral

A API do Venice Dev Challenge é uma REST API que segue os padrões OpenAPI 3.0, implementando autenticação JWT Bearer e validação robusta de dados.

## 🔐 Autenticação

### JWT Bearer Token
Todos os endpoints requerem autenticação via JWT Bearer token no header `Authorization`.

```http
Authorization: Bearer <your-jwt-token>
```

### Obter Token
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-12-31T23:59:59Z",
  "refreshToken": "refresh-token-here"
}
```

## 📊 Endpoints

### Base URL
```
http://localhost:5050/api
```

### Content-Type
```
application/json
```

## 📦 Pedidos (Orders)

### 1. Criar Pedido

#### POST /api/orders
Cria um novo pedido no sistema.

**Headers:**
```http
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "items": [
    {
      "productId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
      "quantity": 2,
      "unitPrice": 29.99
    },
    {
      "productId": "6ba7b811-9dad-11d1-80b4-00c04fd430c8",
      "quantity": 1,
      "unitPrice": 49.99
    }
  ]
}
```

**Response (201 Created):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "createdAt": "2024-01-15T10:30:00Z",
  "status": "Pending",
  "totalAmount": 109.97,
  "items": [
    {
      "id": "item-1-id",
      "productId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
      "quantity": 2,
      "unitPrice": 29.99,
      "totalPrice": 59.98
    },
    {
      "id": "item-2-id",
      "productId": "6ba7b811-9dad-11d1-80b4-00c04fd430c8",
      "quantity": 1,
      "unitPrice": 49.99,
      "totalPrice": 49.99
    }
  ]
}
```

**Códigos de Erro:**
- `400 Bad Request`: Dados inválidos
- `401 Unauthorized`: Token inválido ou expirado
- `422 Unprocessable Entity`: Validação falhou

### 2. Obter Pedido por ID

#### GET /api/orders/{id}
Retorna um pedido específico pelo seu ID.

**Headers:**
```http
Authorization: Bearer <token>
```

**Path Parameters:**
- `id` (string, required): UUID do pedido

**Response (200 OK):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "createdAt": "2024-01-15T10:30:00Z",
  "status": "Pending",
  "totalAmount": 109.97,
  "items": [...],
  "customer": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "João Silva",
    "email": "joao@example.com"
  }
}
```

**Códigos de Erro:**
- `401 Unauthorized`: Token inválido
- `404 Not Found`: Pedido não encontrado

### 3. Listar Todos os Pedidos

#### GET /api/orders
Retorna uma lista paginada de todos os pedidos.

**Headers:**
```http
Authorization: Bearer <token>
```

**Query Parameters:**
- `page` (int, optional): Número da página (padrão: 1)
- `pageSize` (int, optional): Tamanho da página (padrão: 20, máximo: 100)
- `status` (string, optional): Filtrar por status
- `customerId` (string, optional): Filtrar por cliente
- `startDate` (string, optional): Data de início (ISO 8601)
- `endDate` (string, optional): Data de fim (ISO 8601)

**Exemplo de Request:**
```http
GET /api/orders?page=1&pageSize=10&status=Pending&startDate=2024-01-01
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "customerId": "550e8400-e29b-41d4-a716-446655440000",
      "createdAt": "2024-01-15T10:30:00Z",
      "status": "Pending",
      "totalAmount": 109.97,
      "customerName": "João Silva"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

### 4. Atualizar Status do Pedido

#### PUT /api/orders/{id}/status
Atualiza o status de um pedido.

**Headers:**
```http
Authorization: Bearer <token>
Content-Type: application/json
```

**Path Parameters:**
- `id` (string, required): UUID do pedido

**Request Body:**
```json
{
  "status": "Confirmed",
  "notes": "Pedido confirmado pelo cliente"
}
```

**Status Disponíveis:**
- `Pending`: Aguardando confirmação
- `Confirmed`: Confirmado
- `Processing`: Em processamento
- `Shipped`: Enviado
- `Delivered`: Entregue
- `Cancelled`: Cancelado

**Response (200 OK):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "status": "Confirmed",
  "statusChangedAt": "2024-01-15T11:00:00Z",
  "notes": "Pedido confirmado pelo cliente",
  "previousStatus": "Pending"
}
```

### 5. Cancelar Pedido

#### DELETE /api/orders/{id}
Cancela um pedido (soft delete).

**Headers:**
```http
Authorization: Bearer <token>
```

**Path Parameters:**
- `id` (string, required): UUID do pedido

**Response (200 OK):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "status": "Cancelled",
  "cancelledAt": "2024-01-15T12:00:00Z",
  "message": "Pedido cancelado com sucesso"
}
```

## 👥 Usuários (Users)

### 1. Obter Usuário

#### GET /api/users/{id}
Retorna informações de um usuário específico.

**Headers:**
```http
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "João Silva",
  "email": "joao@example.com",
  "phone": "+55 11 99999-9999",
  "createdAt": "2024-01-01T00:00:00Z",
  "isActive": true
}
```

## 📊 Health Checks

### 1. Health Geral

#### GET /health
Verifica a saúde geral de todos os serviços.

**Response (200 OK):**
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "checks": {
    "sqlserver": {
      "status": "Healthy",
      "description": "SQL Server is responding normally",
      "responseTime": "15ms"
    },
    "mongodb": {
      "status": "Healthy",
      "description": "MongoDB is responding normally",
      "responseTime": "8ms"
    },
    "redis": {
      "status": "Healthy",
      "description": "Redis is responding normally",
      "responseTime": "2ms"
    },
    "rabbitmq": {
      "status": "Healthy",
      "description": "RabbitMQ is responding normally",
      "responseTime": "12ms"
    }
  }
}
```

### 2. Health Específico

#### GET /health/{service}
Verifica a saúde de um serviço específico.

**Path Parameters:**
- `service`: sqlserver, mongodb, redis, rabbitmq

**Response (200 OK):**
```json
{
  "status": "Healthy",
  "description": "SQL Server is responding normally",
  "responseTime": "15ms",
  "details": {
    "version": "2022.16.0.4153",
    "database": "VeniceOrders",
    "connections": 5
  }
}
```

## 🔍 Filtros e Busca

### Filtros Disponíveis

#### Status
```http
GET /api/orders?status=Pending,Confirmed
```

#### Data
```http
GET /api/orders?startDate=2024-01-01&endDate=2024-01-31
```

#### Cliente
```http
GET /api/orders?customerId=550e8400-e29b-41d4-a716-446655440000
```

#### Valor
```http
GET /api/orders?minAmount=100&maxAmount=500
```

### Ordenação
```http
GET /api/orders?sortBy=createdAt&sortOrder=desc
```

**Campos de Ordenação:**
- `createdAt`: Data de criação
- `totalAmount`: Valor total
- `status`: Status do pedido
- `customerName`: Nome do cliente

## 📝 Códigos de Status HTTP

### Sucesso
- `200 OK`: Requisição bem-sucedida
- `201 Created`: Recurso criado com sucesso
- `204 No Content`: Requisição bem-sucedida sem conteúdo

### Cliente
- `400 Bad Request`: Requisição malformada
- `401 Unauthorized`: Não autenticado
- `403 Forbidden`: Não autorizado
- `404 Not Found`: Recurso não encontrado
- `409 Conflict`: Conflito de dados
- `422 Unprocessable Entity`: Validação falhou

### Servidor
- `500 Internal Server Error`: Erro interno do servidor
- `502 Bad Gateway`: Erro de gateway
- `503 Service Unavailable`: Serviço indisponível

## 🔒 Validação e Erros

### Validação de Entrada
```json
{
  "errors": [
    {
      "field": "customerId",
      "message": "CustomerId is required",
      "code": "Required"
    },
    {
      "field": "items",
      "message": "At least one item is required",
      "code": "MinLength"
    }
  ]
}
```

### Erro de Validação
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "customerId": ["CustomerId is required"],
    "items": ["At least one item is required"]
  }
}
```

### Erro Interno
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "traceId": "00-1234567890abcdef-1234567890abcdef-00"
}
```

## 📊 Rate Limiting

### Limites
- **Por IP**: 100 requests por minuto
- **Por Usuário**: 1000 requests por hora
- **Por Endpoint**: 50 requests por minuto

### Headers de Rate Limiting
```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1642233600
```

## 🔄 Paginação

### Headers de Paginação
```http
X-Pagination-CurrentPage: 1
X-Pagination-PageSize: 20
X-Pagination-TotalCount: 100
X-Pagination-TotalPages: 5
```

### Links de Paginação
```json
{
  "links": {
    "first": "/api/orders?page=1&pageSize=20",
    "previous": null,
    "next": "/api/orders?page=2&pageSize=20",
    "last": "/api/orders?page=5&pageSize=20"
  }
}
```

## 📱 Exemplos de Uso

### cURL
```bash
# Criar pedido
curl -X POST "http://localhost:5050/api/orders" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "550e8400-e29b-41d4-a716-446655440000",
    "items": [
      {
        "productId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
        "quantity": 2,
        "unitPrice": 29.99
      }
    ]
  }'

# Listar pedidos
curl -H "Authorization: Bearer YOUR_TOKEN" \
  "http://localhost:5050/api/orders?page=1&pageSize=10"
```

### JavaScript (Fetch)
```javascript
const response = await fetch('http://localhost:5050/api/orders', {
  method: 'POST',
  headers: {
    'Authorization': 'Bearer YOUR_TOKEN',
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    customerId: '550e8400-e29b-41d4-a716-446655440000',
    items: [
      {
        productId: '6ba7b810-9dad-11d1-80b4-00c04fd430c8',
        quantity: 2,
        unitPrice: 29.99
      }
    ]
  })
});

const order = await response.json();
```

### C# (HttpClient)
```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

var order = new CreateOrderRequest
{
    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
    Items = new List<OrderItemRequest>
    {
        new OrderItemRequest
        {
            ProductId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
            Quantity = 2,
            UnitPrice = 29.99m
        }
    }
};

var response = await client.PostAsJsonAsync("http://localhost:5050/api/orders", order);
var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
```

---

**Anterior**: [Como Executar](03-getting-started.md) | **Próximo**: [Estrutura do Banco de Dados](05-database-structure.md)
