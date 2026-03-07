# Processo Seletivo Maxiprod
### Tecnologias usadas
**Webapi:**
- [ASP.NET Core](https://dotnet.microsoft.com/pt-br/apps/aspnet) 10.0;
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/);
- [EF Core SQLite](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/?tabs=dotnet-core-cli);
- [FluentValidator](https://docs.fluentvalidation.net/en/latest/);

**Frontend:**
- [Vite](https://vite.dev/) com React Typescript;
- [React Bootstrap](https://react-bootstrap.netlify.app/);
- [React Router](https://reactrouter.com/);
- [Axios](https://axios-http.com/docs/intro);

### Rodar o sistema
**Webapi:**

Abra um terminal na pasta *webapi*. Em seguida, execute os seguintes comandos:

*Rodar as migrations:*
```bash
dotnet ef database update
```

*Executar o Webapi:*
```bash
dotnet run
```

**Frontend:**
Abra outro terminal na pasta *frontend*. Em seguida, execute os seguintes comandos:

*Instalar as dependencias:*
```bash
npm install
```

*Executar o frontend:*
```bash
npm run dev
```
