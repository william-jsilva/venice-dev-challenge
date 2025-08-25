# 🧪 Desenvolvimento e Testes

## Visão Geral

Este documento descreve os padrões de desenvolvimento, estratégias de teste e práticas de CI/CD utilizados no Venice Dev Challenge.

## 🎯 Padrões de Desenvolvimento

### Clean Code Principles

#### 1. Nomenclatura
```csharp
// ✅ Bom
public class OrderService
public async Task<Order> CreateOrderAsync(CreateOrderCommand command)
public interface IOrderRepository

// ❌ Ruim
public class OrderSvc
public async Task<Order> Create(CreateOrderCommand cmd)
public interface IRepo
```

#### 2. Métodos Pequenos e Focados
```csharp
// ✅ Bom - Método focado em uma responsabilidade
public async Task<Order> CreateOrderAsync(CreateOrderCommand command)
{
    var order = await _orderRepository.CreateAsync(command);
    await _eventBus.PublishAsync(new OrderCreatedEvent(order.Id));
    await _cacheService.InvalidateAsync($"order:{order.Id}");
    return order;
}

// ❌ Ruim - Método fazendo muitas coisas
public async Task<Order> ProcessOrderAsync(CreateOrderCommand command)
{
    // Validação
    // Criação
    // Notificação
    // Cache
    // Log
    // Métricas
    // etc...
}
```

#### 3. SOLID Principles
```csharp
// Single Responsibility Principle
public class OrderService
{
    // ✅ Responsável apenas por operações de pedido
    public async Task<Order> CreateAsync(CreateOrderCommand command) { }
    public async Task<Order> GetByIdAsync(Guid id) { }
    public async Task UpdateStatusAsync(Guid id, OrderStatus status) { }
}

// Open/Closed Principle
public abstract class OrderValidator
{
    public abstract ValidationResult Validate(CreateOrderCommand command);
}

public class CreateOrderValidator : OrderValidator
{
    public override ValidationResult Validate(CreateOrderCommand command)
    {
        // Implementação específica
    }
}
```

### CQRS Implementation

#### 1. Commands
```csharp
public class CreateOrderCommand : IRequest<CreateOrderResult>
{
    public Guid CustomerId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;

    public CreateOrderHandler(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.CustomerId, request.Items);
        await _orderRepository.AddAsync(order);
        await _eventBus.PublishAsync(new OrderCreatedEvent(order.Id));
        
        return new CreateOrderResult(order.Id);
    }
}
```

#### 2. Queries
```csharp
public class GetOrderQuery : IRequest<OrderDto>
{
    public Guid Id { get; }

    public GetOrderQuery(Guid id)
    {
        Id = id;
    }
}

public class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id);
        return _mapper.Map<OrderDto>(order);
    }
}
```

### Dependency Injection

#### 1. Service Registration
```csharp
// Program.cs
builder.Services
    .AddScoped<IOrderService, OrderService>()
    .AddScoped<IOrderRepository, OrderRepository>()
    .AddScoped<IEventBus, EventBus>()
    .AddScoped<ICacheService, RedisCacheService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));
```

#### 2. Constructor Injection
```csharp
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public OrdersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
}
```

## 🧪 Estratégias de Teste

### Test Pyramid

```
        /\
       /  \     E2E Tests (Poucos)
      /____\    
     /      \   Integration Tests (Alguns)
    /________\  
   /          \ Unit Tests (Muitos)
  /____________\
```

### 1. Unit Tests

#### Configuração xUnit
```xml
<!-- Venice.Orders.UnitTests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
  </ItemGroup>
</Project>
```

#### Teste de Entidade
```csharp
[Fact]
public void Order_CalculateTotalAmount_ShouldCalculateCorrectly()
{
    // Arrange
    var order = new Order
    {
        CustomerId = Guid.NewGuid(),
        Items = new List<OrderItem>
        {
            new() { Quantity = 2, UnitPrice = 10.00m },
            new() { Quantity = 1, UnitPrice = 15.00m }
        }
    };

    // Act
    order.CalculateTotalAmount();

    // Assert
    order.TotalAmount.Should().Be(35.00m);
}

[Fact]
public void Order_Confirm_WhenPending_ShouldChangeStatusToConfirmed()
{
    // Arrange
    var order = new Order { Status = OrderStatus.Pending };

    // Act
    order.Confirm();

    // Assert
    order.Status.Should().Be(OrderStatus.Confirmed);
}

[Fact]
public void Order_Confirm_WhenNotPending_ShouldThrowException()
{
    // Arrange
    var order = new Order { Status = OrderStatus.Confirmed };

    // Act & Assert
    var action = () => order.Confirm();
    action.Should().Throw<InvalidOperationException>()
        .WithMessage("Order can only be confirmed when in Pending status");
}
```

#### Teste de Handler
```csharp
[Fact]
public async Task CreateOrderHandler_Handle_ShouldCreateOrder()
{
    // Arrange
    var command = new CreateOrderCommand
    {
        CustomerId = Guid.NewGuid(),
        Items = new List<OrderItemRequest>
        {
            new() { ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 10.00m }
        }
    };

    var mockRepository = new Mock<IOrderRepository>();
    var mockEventBus = new Mock<IEventBus>();
    var handler = new CreateOrderHandler(mockRepository.Object, mockEventBus.Object);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().NotBeEmpty();
    
    mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderCreatedEvent>()), Times.Once);
}
```

#### Teste de Validação
```csharp
[Fact]
public void CreateOrderCommandValidator_ValidCommand_ShouldPass()
{
    // Arrange
    var command = new CreateOrderCommand
    {
        CustomerId = Guid.NewGuid(),
        Items = new List<OrderItemRequest>
        {
            new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 10.00m }
        }
    };

    var validator = new CreateOrderCommandValidator();

    // Act
    var result = validator.Validate(command);

    // Assert
    result.IsValid.Should().BeTrue();
}

[Theory]
[InlineData("00000000-0000-0000-0000-000000000000")]
[InlineData("invalid-guid")]
public void CreateOrderCommandValidator_InvalidCustomerId_ShouldFail(string customerId)
{
    // Arrange
    var command = new CreateOrderCommand
    {
        CustomerId = Guid.Parse(customerId),
        Items = new List<OrderItemRequest>()
    };

    var validator = new CreateOrderCommandValidator();

    // Act
    var result = validator.Validate(command);

    // Assert
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateOrderCommand.CustomerId));
}
```

### 2. Integration Tests

#### Configuração
```csharp
public class OrdersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrdersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<OrderItemRequest>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 10.00m }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        order.Should().NotBeNull();
        order.Id.Should().NotBeEmpty();
    }
}
```

### 3. Test Data Builders

#### Builder Pattern para Testes
```csharp
public class OrderBuilder
{
    private Guid _customerId = Guid.NewGuid();
    private List<OrderItem> _items = new();
    private OrderStatus _status = OrderStatus.Pending;

    public OrderBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public OrderBuilder WithItem(int quantity, decimal unitPrice)
    {
        _items.Add(new OrderItem
        {
            ProductId = Guid.NewGuid(),
            Quantity = quantity,
            UnitPrice = unitPrice
        });
        return this;
    }

    public OrderBuilder WithStatus(OrderStatus status)
    {
        _status = status;
        return this;
    }

    public Order Build()
    {
        var order = new Order
        {
            CustomerId = _customerId,
            Status = _status,
            Items = _items
        };

        order.CalculateTotalAmount();
        return order;
    }
}

// Uso
var order = new OrderBuilder()
    .WithCustomerId(Guid.NewGuid())
    .WithItem(2, 10.00m)
    .WithItem(1, 15.00m)
    .WithStatus(OrderStatus.Pending)
    .Build();
```

## 🔄 CI/CD Pipeline

### GitHub Actions

#### 1. Build e Test
```yaml
# .github/workflows/build.yml
name: Build and Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Test
      run: dotnet test --no-build --verbosity normal --configuration Release
    
    - name: Test Coverage
      run: dotnet test --collect:"XPlat Code Coverage" --results-directory coverage
    
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
      with:
        file: ./coverage/**/coverage.cobertura.xml
```

#### 2. Docker Build
```yaml
# .github/workflows/docker.yml
name: Docker Build

on:
  push:
    tags: [ 'v*' ]

jobs:
  docker:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3
    
    - name: Login to Docker Hub
      uses: docker/login-action@v3
      with:
        username: ${{ secrets.DOCKER_USERNAME }}
        password: ${{ secrets.DOCKER_PASSWORD }}
    
    - name: Build and push
      uses: docker/build-push-action@v5
      with:
        context: ./src
        push: true
        tags: |
          venice/orders-api:latest
          venice/orders-api:${{ github.ref_name }}
```

### Docker Multi-Stage Build

#### Dockerfile Otimizado
```dockerfile
# Multi-stage build para produção
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Venice.Orders.WebApi/Venice.Orders.WebApi.csproj", "Venice.Orders.WebApi/"]
COPY ["Venice.Orders.Application/Venice.Orders.Application.csproj", "Venice.Orders.Application/"]
COPY ["Venice.Orders.Domain/Venice.Orders.Domain.csproj", "Venice.Orders.Domain/"]
COPY ["Venice.Orders.Infrastructure/Venice.Orders.Infrastructure.csproj", "Venice.Orders.Infrastructure/"]
COPY ["Venice.Orders.Common/Venice.Orders.Common.csproj", "Venice.Orders.Common/"]
RUN dotnet restore "Venice.Orders.WebApi/Venice.Orders.WebApi.csproj"

COPY . .
WORKDIR "/src/Venice.Orders.WebApi"
RUN dotnet build "Venice.Orders.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Venice.Orders.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Venice.Orders.WebApi.dll"]
```

## 📊 Code Quality

### SonarQube Integration

#### 1. Configuração
```yaml
# sonar-project.properties
sonar.projectKey=venice-dev-challenge
sonar.projectName=Venice Dev Challenge
sonar.projectVersion=1.0.0

sonar.sources=src
sonar.tests=tests
sonar.language=cs
sonar.dotnet.version=9.0

sonar.coverage.exclusions=**/*Test.cs,**/*Tests.cs
sonar.test.exclusions=**/*Test.cs,**/*Tests.cs
```

#### 2. GitHub Action para SonarQube
```yaml
- name: SonarQube Analysis
  uses: sonarqube-quality-gate-action@master
  env:
    GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
  with:
    args: >
      -Dsonar.projectKey=venice-dev-challenge
      -Dsonar.sources=src
      -Dsonar.tests=tests
```

### Code Analysis

#### 1. EditorConfig
```ini
# .editorconfig
root = true

[*]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
trim_trailing_whitespace = true

[*.cs]
indent_style = space
indent_size = 4

# C# formatting
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
```

#### 2. Analyzers
```xml
<!-- Venice.Orders.WebApi.csproj -->
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="9.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

## 🚀 Deploy

### Kubernetes

#### 1. Deployment
```yaml
# k8s/deployment.yml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: venice-orders-api
  labels:
    app: venice-orders-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: venice-orders-api
  template:
    metadata:
      labels:
        app: venice-orders-api
    spec:
      containers:
      - name: venice-orders-api
        image: venice/orders-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: venice-secrets
              key: sqlserver-connection
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5
```

#### 2. Service
```yaml
# k8s/service.yml
apiVersion: v1
kind: Service
metadata:
  name: venice-orders-api-service
spec:
  selector:
    app: venice-orders-api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: LoadBalancer
```

### Helm Charts

#### 1. Chart Structure
```
venice-orders/
├── Chart.yaml
├── values.yaml
├── templates/
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── ingress.yaml
│   └── configmap.yaml
└── charts/
```

#### 2. Values
```yaml
# values.yaml
replicaCount: 3

image:
  repository: venice/orders-api
  tag: latest
  pullPolicy: IfNotPresent

service:
  type: LoadBalancer
  port: 80

ingress:
  enabled: true
  className: nginx
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /
  hosts:
    - host: venice-orders.local
      paths:
        - path: /
          pathType: Prefix

resources:
  limits:
    cpu: 500m
    memory: 512Mi
  requests:
    cpu: 250m
    memory: 256Mi
```

## 📈 Monitoramento

### Application Insights

#### 1. Configuração
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();

// appsettings.json
{
  "ApplicationInsights": {
    "ConnectionString": "YOUR_CONNECTION_STRING",
    "EnableAdaptiveSampling": true,
    "EnablePerformanceCounterCollectionModule": true
  }
}
```

#### 2. Custom Telemetry
```csharp
public class OrderService
{
    private readonly TelemetryClient _telemetryClient;

    public async Task<Order> CreateOrderAsync(CreateOrderCommand command)
    {
        using var operation = _telemetryClient.StartOperation<RequestTelemetry>("CreateOrder");
        
        try
        {
            var order = await _orderRepository.CreateAsync(command);
            _telemetryClient.TrackEvent("OrderCreated", new Dictionary<string, string>
            {
                ["OrderId"] = order.Id.ToString(),
                ["CustomerId"] = order.CustomerId.ToString(),
                ["TotalAmount"] = order.TotalAmount.ToString()
            });
            
            return order;
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(ex);
            throw;
        }
    }
}
```

---

**Anterior**: [Estrutura do Banco de Dados](05-database-structure.md) | **Voltar ao Índice**: [📚 Índice da Documentação](00-index.md)
