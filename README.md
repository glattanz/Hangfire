# Hangfire

Este projeto é uma API Web ASP.NET Core que utiliza o Hangfire para gerenciamento de tarefas em segundo plano.

## Comandos Utilizados

### Adicionando SQLClient ao projeto
```bash
dotnet add package Microsoft.Data.SqlClient
```

### Adicionando o framework Hangfire (Console do Gerenciador de Pacotes)
```bash
NuGet\Install-Package Hangfire.Core -Version 1.8.11
Install-Package Hangfire.SqlServer -Version 1.8.11
Install-Package Hangfire.AspNetCore -Version 1.8.11
```

### Referência
```bash
Seguindo esta referência: Hangfire Documentation
```

### Criando a conexão com o banco de dados via cmd
```bash
sqllocaldb create "Hangfire"
```

### Criando o banco de dados via query
```bash
CREATE DATABASE Hangfire
```