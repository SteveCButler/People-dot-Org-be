using Microsoft.EntityFrameworkCore;
using people_dot_org.Models;

namespace people_dot_org;

public class PeopleDotOrgDbContext : DbContext
{
    public DbSet<Person> People { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Request> Requests { get; set; }

    public PeopleDotOrgDbContext(DbContextOptions<PeopleDotOrgDbContext> context) : base(context)
    {

    }
}
