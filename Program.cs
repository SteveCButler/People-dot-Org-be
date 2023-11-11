using people_dot_org.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using people_dot_org;
using System;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//ADD CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000",
                                "http://localhost:7287")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<PeopleDotOrgDbContext>(builder.Configuration["PeopleDotOrgDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

//Add for Cors
app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//  ### USER Endpoints ###

//CHECK USER EXISTS
app.MapGet("/api/checkuser/{uid}", (PeopleDotOrgDbContext db, string uid) =>
{
    var userExists = db.People.Where(x => x.Uid == uid).FirstOrDefault();
    if (userExists == null)
    {
        return Results.StatusCode(204);
    }
    return Results.Ok(userExists);
});

//Get All People
app.MapGet("/api/people", (PeopleDotOrgDbContext db) =>
{
    return db.People.ToList();
});

//Create Person
app.MapPost("/api/person", (PeopleDotOrgDbContext db, Person person) =>
{
    db.People.Add(person);
    db.SaveChanges();
    return Results.Created($"/api/people/{person.Id}", person);
});

//Edit Person
app.MapPut("/api/person/{personId}", (PeopleDotOrgDbContext db, int id, Person person) =>
{
    var personToUpdate = db.People.FirstOrDefault(x => x.Id == id);
    if (personToUpdate == null)
    {
        return Results.NotFound();
    }

    personToUpdate.Id = person.Id;
    personToUpdate.FirstName = person.FirstName;
    personToUpdate.LastName = person.LastName;
    personToUpdate.Email = person.Email;
    personToUpdate.Phone = person.Phone;
    personToUpdate.IsAdmin = person.IsAdmin;
    personToUpdate.IsTeamLead = person.IsTeamLead;

    db.SaveChanges();

    return Results.NoContent();
});


// Delete an user
app.MapDelete("/api/person/{personId}", (PeopleDotOrgDbContext db, int id) =>
{
    Person person = db.People.SingleOrDefault(u => u.Id == id);
    if (person == null)
    {
        return Results.NotFound();
    }
    db.People.Remove(person);
    db.SaveChanges();
    return Results.NoContent();
});

// ### TEAM Endpoints ###

// Get All Teams
app.MapGet("/api/teams", (PeopleDotOrgDbContext db) =>
{
    return db.Teams.ToList();
});

// Create Team
app.MapPost("/api/team", (PeopleDotOrgDbContext db, Team team) =>
{
    db.Teams.Add(team);
    db.SaveChanges();
    return Results.Created($"/api/people/{team.Id}", team);
});

// Update Team
app.MapPut("/api/team/{teamId}", (PeopleDotOrgDbContext db, int id, Team team) =>
{
    Team teamToUpdate = db.Teams.SingleOrDefault(s => s.Id == id);
    if (teamToUpdate == null)
    {
        return Results.NotFound();
    }

    teamToUpdate.Id = team.Id;
    teamToUpdate.Name = team.Name;
    teamToUpdate.Description = team.Description;

    db.SaveChanges();

    return Results.NoContent();
});

// Delete Team
app.MapDelete("/api/team/{teamId}", (PeopleDotOrgDbContext db, int id) =>
{
    Team teamToRemove = db.Teams.SingleOrDefault(x => x.Id == id);
    if (teamToRemove == null)
    {
        return Results.NotFound();
    }

    db.Teams.Remove(teamToRemove);
    db.SaveChanges();

    return Results.NoContent();
});

app.Run();

