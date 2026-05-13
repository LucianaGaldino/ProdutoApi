#  Produto API

API REST para gerenciamento de produtos desenvolvida com **ASP.NET Core 8**, **C#** e **MySQL**, seguindo arquitetura em camadas com boas práticas de mercado.

---

##  Tecnologias

![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

- ASP.NET Core 8 (Web API)
- MySQL 8.0 via `MySql.Data`
- Swagger UI via `Swashbuckle.AspNetCore`
- LINQ para consultas, ordenação e cálculos

---

##  Arquitetura

O projeto segue o padrão **Controller → Service → Repository**:

```
Controller       Recebe a requisição HTTP, delega ao Service, retorna a resposta
    ↓
Service          Regras de negócio, validações, cálculos com LINQ
    ↓
Repository       Queries MySQL parametrizadas, mapeamento de entidades
```

### Estrutura de Pastas

```
ProdutoAPI/
├── Controllers/
│     └── ProdutoController.cs
├── Services/
│     └── ProdutoService.cs
├── Repositories/
│     └── ProdutoRepository.cs
├── Models/
│     ├── Produto.cs
│     └── ProdutoDtos.cs
├── Database/
│     ├── DatabaseConnection.cs
│     └── script.sql
├── Exceptions/
│     └── AppException.cs
├── Enums/
│     └── StatusCode.cs
├── appsettings.json
└── Program.cs
```

---

##  Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL Server 8.0+

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/produto-api.git
cd produto-api
```

### 2. Configure o banco de dados

Execute o script SQL para criar o banco e os dados iniciais:

```bash
mysql -u root -p < Database/script.sql
```

### 3. Configure a connection string

Edite o arquivo `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=produto_db;User Id=root;Password=SUA_SENHA;"
}
```

### 4. Restaure as dependências e execute

```bash
dotnet restore
dotnet run
```

### 5. Acesse o Swagger

Abra o navegador em **http://localhost:5000** para acessar a documentação interativa.

---

## 📡 Endpoints

| Método | Endpoint | Descrição | Status |
|--------|----------|-----------|--------|
| `GET` | `/api/produto` | Listar todos os produtos | 200, 500 |
| `GET` | `/api/produto/{id}` | Buscar produto por Id | 200, 400, 404 |
| `POST` | `/api/produto` | Cadastrar novo produto | 201, 400, 500 |
| `PUT` | `/api/produto/{id}` | Atualizar produto | 200, 400, 404 |
| `DELETE` | `/api/produto/{id}` | Remover produto | 204, 400, 404 |
| `GET` | `/api/produto/estoque` | Valor total do estoque | 200, 500 |

### Ordenação

O endpoint de listagem aceita o parâmetro `ordenarPor`:

```
GET /api/produto?ordenarPor=preco
GET /api/produto?ordenarPor=nome
GET /api/produto?ordenarPor=quantidade
```

### Exemplos de Request/Response

**POST /api/produto**
```json
// Request
{
  "nome": "Notebook Dell Inspiron",
  "preco": 3499.90,
  "quantidade": 10
}

// Response 201 Created
{
  "id": 1,
  "nome": "Notebook Dell Inspiron",
  "preco": 3499.90,
  "quantidade": 10
}
```

**GET /api/produto/estoque**
```json
// Response 200 OK
{
  "totalProdutos": 8,
  "valorTotal": 97430.50
}
```

**Erro (exemplo 404)**
```json
{
  "statusCode": 404,
  "mensagem": "Produto com Id 99 não encontrado."
}
```

---

##  Validações

Todas as validações são feitas na camada **Service**, antes de qualquer acesso ao banco:

| Campo | Regra |
|-------|-------|
| `nome` | Obrigatório, entre 2 e 100 caracteres |
| `preco` | Mínimo R$ 0,01 (não pode ser zero ou negativo) |
| `quantidade` | Não pode ser negativa (zero é permitido) |
| `id` | Deve ser um número inteiro positivo |

---

##  Tratamento de Erros

A aplicação utiliza uma exceção global personalizada `AppException` combinada com o enum `StatusCode`, eliminando números mágicos do código:

```csharp
public enum StatusCode
{
    Success       = 200,
    BadRequest    = 400,
    NotFound      = 404,
    InternalError = 500
}
```

- **Erros esperados** → `AppException` capturada no Controller → resposta com mensagem amigável
- **Erros inesperados** → `Exception` genérica → HTTP 500 + log no servidor

---

##  Dependências NuGet

```bash
dotnet add package MySql.Data --version 9.1.0
dotnet add package Swashbuckle.AspNetCore --version 6.9.0
```

---
