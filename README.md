# People-dot-Org Backend 

## Setup
1. Clone or download repo to your machine
2. Install PostgreSQL server on your machine
   


5. Open solution in Visual Studio and run the following:
   Add secrets:
    dotnet user-secrets set "PeopleDotOrgDbConnectionString" "Host=localhost;Port=5432;Username=postgres;Password=<your_postgresql_password>;Database=peopledotorg"
   Run database update to build DB:
 ```dotnet ef database update
7. Open PgAdmin to verify database
