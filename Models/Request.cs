namespace people_dot_org.Models;

public class Request
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public int PersonId { get; set; }
    public int RequestState { get; set; } 
    public Plan? Plan { get; set; }
    public Person? Person { get; set; }

     enum RequestResponse
    {
        Awaiting,
        Accepted,
        Declined

    }
}
