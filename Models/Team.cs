namespace people_dot_org.Models;

public class Team
{
    public Team()
    {
        this.Persons = new HashSet<Person>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int PlanId { get; set; }

    public ICollection<Person> Persons { get; set; }

}
