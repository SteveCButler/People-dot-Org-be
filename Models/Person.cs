using System.Security.Cryptography.X509Certificates;

namespace people_dot_org.Models;

public class Person
{
    public Person()
    {
        this.Teams = new HashSet<Team>();
        this.CreatedOn = DateTime.Now;
    }
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Uid { get; set; }
    public DateTime CreatedOn { get; set; }
    public Boolean IsAdmin { get; set; }
    public Boolean IsTeamLead { get; set; }
    public ICollection<Team> Teams { get; set; }

    
}
