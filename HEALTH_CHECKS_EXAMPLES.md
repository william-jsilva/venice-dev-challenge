# Exemplos de Teste dos Health Checks

## 🧪 Como Testar os Health Checks

### 1. Teste Básico com curl

#### Health Check Principal
```bash
curl -X GET http://localhost:5000/health
```

**Resposta esperada:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-01T12:00:00Z",
  "duration": "00:00:00.1234567",
  "checks": [
    {
      "name": "application",
      "status": "Healthy",
      "description": "Application is healthy",
      "duration": "00:00:00.0012345",
      "tags": ["app"],
      "data": {
        "timestamp": "2024-01-01T12:00:00Z",
        "version": "1.0.0",
        "environment": "Development"
      }
    },
    {
      "name": "sqlserver",
      "status": "Healthy",
      "description": "SQL Server is responding normally",
      "duration": "00:00:00.0123456",
      "tags": ["database", "sql"]
    }
  ]
}
```

#### Health Check de Readiness
```bash
curl -X GET http://localhost:5000/health/ready
```

#### Health Check de Liveness
```bash
curl -X GET http://localhost:5000/health/live
```

### 2. Teste via API REST

#### Status Completo
```bash
curl -X GET http://localhost:5000/api/health/status
```

**Resposta esperada:**
```json
{
  "success": true,
  "message": "Application is healthy",
  "data": {
    "status": "Healthy",
    "timestamp": "2024-01-01T12:00:00Z",
    "duration": "00:00:00.1234567",
    "checks": [...]
  }
}
```

#### Informações do Sistema
```bash
curl -X GET http://localhost:5000/api/health/info
```

**Resposta esperada:**
```json
{
  "success": true,
  "message": "Application information retrieved successfully",
  "data": {
    "application": "Venice Orders API",
    "version": "1.0.0",
    "environment": "Development",
    "timestamp": "2024-01-01T12:00:00Z",
    "uptime": 1234567,
    "framework": "9.0.0",
    "os": "Microsoft Windows NT 10.0.19045.0",
    "processorCount": 8,
    "workingSet": 12345678,
    "gcMemoryInfo": {
      "heapSizeBytes": 1234567,
      "totalAvailableMemoryBytes": 987654321
    }
  }
}
```

### 3. Teste com PowerShell

```powershell
# Health check principal
Invoke-RestMethod -Uri "http://localhost:5000/health" -Method GET

# Health check de readiness
Invoke-RestMethod -Uri "http://localhost:5000/health/ready" -Method GET

# Health check de liveness
Invoke-RestMethod -Uri "http://localhost:5000/health/live" -Method GET

# Status via API
Invoke-RestMethod -Uri "http://localhost:5000/api/health/status" -Method GET

# Informações do sistema
Invoke-RestMethod -Uri "http://localhost:5000/api/health/info" -Method GET
```

### 4. Teste com JavaScript/Node.js

```javascript
const axios = require('axios');

async function testHealthChecks() {
  const baseUrl = 'http://localhost:5000';
  
  try {
    // Health check principal
    const health = await axios.get(`${baseUrl}/health`);
    console.log('Health Check:', health.data);
    
    // Health check de readiness
    const ready = await axios.get(`${baseUrl}/health/ready`);
    console.log('Ready Check:', ready.data);
    
    // Health check de liveness
    const live = await axios.get(`${baseUrl}/health/live`);
    console.log('Live Check:', live.data);
    
    // Status via API
    const status = await axios.get(`${baseUrl}/api/health/status`);
    console.log('Status API:', status.data);
    
    // Informações do sistema
    const info = await axios.get(`${baseUrl}/api/health/info`);
    console.log('System Info:', info.data);
    
  } catch (error) {
    console.error('Error testing health checks:', error.response?.data || error.message);
  }
}

testHealthChecks();
```

### 5. Teste com Python

```python
import requests
import json

def test_health_checks():
    base_url = 'http://localhost:5000'
    
    endpoints = [
        '/health',
        '/health/ready',
        '/health/live',
        '/api/health/status',
        '/api/health/info'
    ]
    
    for endpoint in endpoints:
        try:
            response = requests.get(f"{base_url}{endpoint}")
            print(f"\n{endpoint}:")
            print(f"Status Code: {response.status_code}")
            print(f"Response: {json.dumps(response.json(), indent=2)}")
        except Exception as e:
            print(f"\n{endpoint}: Error - {e}")

if __name__ == "__main__":
    test_health_checks()
```

### 6. Teste de Cenários de Falha

#### Simular Falha no SQL Server
1. Pare o container do SQL Server:
```bash
docker stop sqlserver
```

2. Teste o health check:
```bash
curl -X GET http://localhost:5000/health
```

**Resposta esperada (parcial):**
```json
{
  "status": "Unhealthy",
  "checks": [
    {
      "name": "sqlserver",
      "status": "Unhealthy",
      "description": "SQL Server connection failed",
      "duration": "00:00:05.1234567"
    }
  ]
}
```

#### Simular Falha no Redis
1. Pare o container do Redis:
```bash
docker stop redis
```

2. Teste o health check:
```bash
curl -X GET http://localhost:5000/health
```

### 7. Teste de Performance

#### Teste de Concorrência
```bash
# Teste com múltiplas requisições simultâneas
for i in {1..10}; do
  curl -X GET http://localhost:5000/health &
done
wait
```

#### Teste de Timeout
```bash
# Teste com timeout baixo
curl -X GET http://localhost:5000/health --max-time 1
```

### 8. Monitoramento Contínuo

#### Script de Monitoramento
```bash
#!/bin/bash

while true; do
  echo "$(date): Testing health check..."
  
  response=$(curl -s -w "%{http_code}" http://localhost:5000/health)
  http_code="${response: -3}"
  body="${response%???}"
  
  if [ "$http_code" -eq 200 ]; then
    echo "✅ Health check passed"
  else
    echo "❌ Health check failed (HTTP $http_code)"
    echo "Response: $body"
  fi
  
  sleep 30
done
```

#### Script PowerShell
```powershell
while ($true) {
    Write-Host "$(Get-Date): Testing health check..."
    
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:5000/health" -Method GET
        Write-Host "✅ Health check passed" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Health check failed" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    Start-Sleep -Seconds 30
}
```

### 9. Integração com Ferramentas de Monitoramento

#### Prometheus
```yaml
# prometheus.yml
scrape_configs:
  - job_name: 'venice-orders-health'
    static_configs:
      - targets: ['localhost:5000']
    metrics_path: '/health'
    scrape_interval: 30s
```

#### Grafana Dashboard
```json
{
  "dashboard": {
    "title": "Venice Orders Health",
    "panels": [
      {
        "title": "Health Status",
        "type": "stat",
        "targets": [
          {
            "expr": "up{job=\"venice-orders-health\"}"
          }
        ]
      }
    ]
  }
}
```

### 10. Teste de Integração

#### Teste Completo da Aplicação
```bash
#!/bin/bash

echo "🧪 Starting comprehensive health check test..."

# 1. Verificar se a aplicação está rodando
echo "1. Testing application liveness..."
liveness=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/health/live)
if [ "$liveness" -eq 200 ]; then
    echo "✅ Application is alive"
else
    echo "❌ Application is not responding"
    exit 1
fi

# 2. Verificar readiness
echo "2. Testing application readiness..."
readiness=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/health/ready)
if [ "$readiness" -eq 200 ]; then
    echo "✅ Application is ready"
else
    echo "❌ Application is not ready"
    exit 1
fi

# 3. Verificar health completo
echo "3. Testing complete health check..."
health=$(curl -s http://localhost:5000/health)
echo "Health status: $health"

# 4. Verificar informações do sistema
echo "4. Testing system information..."
info=$(curl -s http://localhost:5000/api/health/info)
echo "System info: $info"

echo "🎉 All health checks passed!"
```

Este script pode ser executado como parte de um pipeline CI/CD ou para monitoramento contínuo da aplicação.
