# ShortLinks

Lightweight and extensible **URL shortener for .NET / ASP.NET Core**.

ShortLinks helps you create, resolve, and manage short URLs with support for  
**SQL Server (EF Core)** and **Redis**, without requiring ABP or any heavy framework.

> 🚀 **New to ShortLinks?**  
> Start here: [docs/README.md](docs/README.md)  
> Prefer PDF? [Download User Manual](https://github.com/MohamedErfan1998/ShortLinks/blob/main/docs/ShortLinks.UserManual.pdf)

---

## ✨ Features

- Create short links (auto-generated or custom codes)
- Resolve & redirect short links
- **EF Core (SQL Server) support**
- **Redis support (high performance & distributed)**
- Expiring links (`ExpireAtUtc`)
- One-time & limited-use links (`OneTime`, `MaxUses`)
- Hit tracking (`HitCount`, `LastAccessedAtUtc`)
- Controller-based (no ABP, no magic)
- Production-ready & concurrency-safe

---

## 📦 Packages

### Core
- `ShortLinks.Core`
- `ShortLinks.Abstractions`

### Storage Providers (choose one)
- `ShortLinks.Storage.EFCore` — SQL Server
- `ShortLinks.Storage.Redis` — Redis (recommended for production)

---

## 📥 Installation

### Core (required)

```bash
dotnet add package ShortLinks.Core
```

---

### EF Core (SQL Server)

```bash
dotnet add package ShortLinks.Storage.EFCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

Use this option if you want relational storage with migrations and easy querying.

---

### Redis

```bash
dotnet add package ShortLinks.Storage.Redis
dotnet add package StackExchange.Redis
```
