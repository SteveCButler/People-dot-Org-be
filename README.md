# People-dot-Org Backend 

This is the backend API for https://github.com/SteveCButler/People-dot-Org-fe

## Setup
1. Clone or download repo to your machine
2. Install PostgreSQL server on your machine
4. Open solution in Visual Studio and run the following:
   ```
    dotnet user-secrets set "PeopleDotOrgDbConnectionString" "Host=localhost;Port=5432;Username=postgres;Password=<your_postgresql_password>;Database=peopledotorg"
   ```
   
```
dotnet ef database update
```
5. Open PgAdmin to verify database
6. Start backend
