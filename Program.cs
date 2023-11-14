using people_dot_org.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using people_dot_org;
using System;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//ADD CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000",
                                "http://localhost:7120",
                                "http://localhost:5432")
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

// ### Plan Endpoints ###

// Get all plans
app.MapGet("/api/plans", (PeopleDotOrgDbContext db) =>
{
    return db.Plans.ToList();

});

// Create Plan
app.MapPost("/api/plan", (PeopleDotOrgDbContext db, Plan plan) =>
{
    db.Plans.Add(plan);
    db.SaveChanges();
    return Results.Created($"/api/people/{plan.Id}", plan);
});

// Update Plan
app.MapPut("/api/plan/{planId}", (PeopleDotOrgDbContext db, int id, Plan plan) =>
{
    Plan planToUpdate = db.Plans.SingleOrDefault(s => s.Id == id);
    if (planToUpdate == null)
    {
        return Results.NotFound();
    }

    planToUpdate.Id = plan.Id;
    planToUpdate.Name = plan.Name;
    planToUpdate.Details = plan.Details;

    db.SaveChanges();

    return Results.NoContent();
});

// Delete Plan
app.MapDelete("/api/plan/{planId}", (PeopleDotOrgDbContext db, int id) =>
{
    Plan planToRemove = db.Plans.SingleOrDefault(p => p.Id == id);

    if (planToRemove == null)
    {
        return Results.NotFound();
    }

    db.Plans.Remove(planToRemove);
    db.SaveChanges();

    return Results.NoContent();
});

// ### REQUEST Endpoints

// Get all requests
app.MapGet("/api/requests", (PeopleDotOrgDbContext db) =>
{
    return db.Requests.ToList();
});

// Post a new request
app.MapPost("/api/request", (PeopleDotOrgDbContext db, Request request) =>
{
    db.Requests.Add(request);
    db.SaveChanges();

    return Results.Created($"/api/request/{request.Id}", request);
});


// Delete Request
app.MapDelete("/api/request/{requestId}", (PeopleDotOrgDbContext db, int id) =>
{
    Request requestToRemove = db.Requests.SingleOrDefault(x  => x.Id == id);

    if (requestToRemove == null)
    {
        return Results.NotFound();
    }

    db.Requests.Remove(requestToRemove);
    db.SaveChanges();
    return Results.NoContent();
});



app.Run();

