namespace people_dot_org.Models;

public class Team
{
    public Team()
    {
        this.People = new HashSet<Person>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? PlanId { get; set; }
    public int TeamLeadId { get; set; }
    public ICollection<Person> People { get; set; }

}
