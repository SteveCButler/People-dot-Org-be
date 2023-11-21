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

//Get Single Person By Id
app.MapGet("/api/person/{id}", (PeopleDotOrgDbContext db, int id) =>
{
    Person person = db.People.SingleOrDefault(x => x.Id == id);
    if (person == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(person);
});

//Create Person
app.MapPost("/api/person", (PeopleDotOrgDbContext db, Person person) =>
{
    db.People.Add(person);
    db.SaveChanges();
    return Results.Created($"/api/people/{person.Id}", person);
});

//Edit Person
app.MapPut("/api/person/{personId}", (PeopleDotOrgDbContext db, int personId, Person person) =>
{
    var personToUpdate = db.People.FirstOrDefault(x => x.Id == personId);
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


// Delete an person
app.MapDelete("/api/person/{personId}", (PeopleDotOrgDbContext db, int personId) =>
{
    Person person = db.People.SingleOrDefault(u => u.Id == personId);
    if (person == null)
    {
        return Results.NotFound();
    }
    foreach (var team in person.Teams)
    {
        db.Teams.Remove(team);
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

//Get Single Team By Id
app.MapGet("/api/singleTeam/{id}", (PeopleDotOrgDbContext db, int id) =>
{
    Team team = db.Teams.Where(x => x.Id == id).Include(x => x.People).FirstOrDefault();
    if (team == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(team);
});

//Get team by TeamLeadId
app.MapGet("/api/team/{teamLeadId}", (PeopleDotOrgDbContext db, int teamLeadId) =>
{
    Team teamLead = db.Teams.Where(x => x.TeamLeadId == teamLeadId).Include(x => x.People).FirstOrDefault();
    if (teamLead == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(teamLead);
});

// Create Team
app.MapPost("/api/team", (PeopleDotOrgDbContext db, Team team) =>
{
    db.Teams.Add(team);
    db.SaveChanges();
    return Results.Created($"/api/people/{team.Id}", team);
});

// Update Team
app.MapPut("/api/team/{teamId}", (PeopleDotOrgDbContext db, int teamId, Team team) =>
{
    Team teamToUpdate = db.Teams.SingleOrDefault(s => s.Id == teamId);
    if (teamToUpdate == null)
    {
        return Results.NotFound();
    }

    teamToUpdate.Id = team.Id;
    teamToUpdate.Name = team.Name;
    teamToUpdate.Description = team.Description;
    teamToUpdate.PlanId = team.PlanId;

    db.SaveChanges();

    return Results.NoContent();
});

//Add Team Lead to Team
app.MapPut("/api/team/{teamId}/{personId}", (PeopleDotOrgDbContext db, int teamId, int personId,  Team team) =>
{
    Team teamToUpdate = db.Teams.SingleOrDefault(s => s.Id == teamId);
    Person personToUpdate = db.People.SingleOrDefault(p => p.Id == personId);
    if (teamToUpdate == null)
    {
        return Results.NotFound();
    }

    teamToUpdate.TeamLeadId = personId;

    personToUpdate.IsTeamLead = true;

    db.SaveChanges();

    return Results.NoContent();
});

// Delete Team
app.MapDelete("/api/team/{teamId}", (PeopleDotOrgDbContext db, int teamId) =>
{
    Team teamToRemove = db.Teams.SingleOrDefault(x => x.Id == teamId);
    if (teamToRemove == null)
    {
        return Results.NotFound();
    }

    db.Teams.Remove(teamToRemove);
    db.SaveChanges();

    return Results.NoContent();
});

//Add Person to Team

app.MapPost("/api/add-to-team/{teamId}/{personId}", async (PeopleDotOrgDbContext db, int teamId, int personId) =>
{
    var team = await db.Teams.FindAsync(teamId);
    var person = await db.People.FindAsync(personId);

    if (team == null || person == null)
    {
        return Results.NotFound("Team or person not found");
    }

    // Check if the person is already part of the team
    if (team.People.Any(p => p.Id == personId))
    {
        return Results.BadRequest("Person is already part of the team");
    }

    // Add the person to the team
    team.People.Add(person);

    try
    {
        await db.SaveChangesAsync();
        return Results.Ok($"Person {person.FirstName} {person.LastName} added to Team {team.Name} successfully");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error: {ex.Message}");
    }
});

//Remove Person From TEam
app.MapDelete("/api/remove-from-team/{teamId}/{personId}", async (PeopleDotOrgDbContext db, int teamId, int personId) =>
{
    // var team = await db.Teams.FindAsync(teamId);
    var team = db.Teams.Where(x => x.Id == teamId).Include(x => x.People).FirstOrDefault();
    var person = await db.People.FindAsync(personId);

    if (team == null || person == null)
    {
        return Results.NotFound("Team or person not found");
    }

    // Check if the person is part of the team
    if (!team.People.Any(p => p.Id == personId))
    {
        return Results.BadRequest("Person is not part of the team");
    }

    // Remove the person from the team
    team.People.Remove(person);

    try
    {
        await db.SaveChangesAsync();
        return Results.Ok($"Person {person.FirstName} {person.LastName} removed from Team {team.Name} successfully");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error: {ex.Message}");
    }
});





// ### Plan Endpoints ###

// Get all plans
app.MapGet("/api/plans", (PeopleDotOrgDbContext db) =>
{
    return db.Plans.ToList();

});

//Get Single Plan By Id
app.MapGet("/api/singlePlan/{id}", (PeopleDotOrgDbContext db, int id) =>
{
    Plan plan = db.Plans.Where(x => x.Id == id).FirstOrDefault();
    if (plan == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(plan);
});

//Get Teams by PlanId
app.MapGet("/api/teams-by-planId/{planId}", (PeopleDotOrgDbContext db, int planId) =>
{
    var teams = db.Teams.Where(x => x.PlanId == planId).ToList();
    if (teams == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(teams);
});

// Create Plan
app.MapPost("/api/plan", (PeopleDotOrgDbContext db, Plan plan) =>
{
    db.Plans.Add(plan);
    db.SaveChanges();
    return Results.Created($"/api/people/{plan.Id}", plan);
});

// Update Plan
app.MapPut("/api/plan/{planId}", (PeopleDotOrgDbContext db, int planId, Plan plan) =>
{
    Plan planToUpdate = db.Plans.SingleOrDefault(s => s.Id == planId);
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
app.MapDelete("/api/plan/{planId}", (PeopleDotOrgDbContext db, int planId) =>
{
    Plan planToRemove = db.Plans.SingleOrDefault(p => p.Id == planId);

    if (planToRemove == null)
    {
        return Results.NotFound();
    }

    db.Plans.Remove(planToRemove);
    db.SaveChanges();

    return Results.NoContent();
});

//Add Team to Plan
app.MapPut("/api/add-plan-to-team/{teamId}/{planId}", (PeopleDotOrgDbContext db, int teamId, int planId, Team team) =>
{
    Team teamToUpdate = db.Teams.SingleOrDefault(s => s.Id == teamId);
    if (teamToUpdate == null)
    {
        return Results.NotFound();
    }

    teamToUpdate.PlanId = planId;


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
app.MapDelete("/api/request/{requestId}", (PeopleDotOrgDbContext db, int requestId) =>
{
    Request requestToRemove = db.Requests.SingleOrDefault(x  => x.Id == requestId);

    if (requestToRemove == null)
    {
        return Results.NotFound();
    }

    db.Requests.Remove(requestToRemove);
    db.SaveChanges();
    return Results.NoContent();
});



app.Run();

