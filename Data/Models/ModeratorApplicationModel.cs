using Data.Enum;

namespace Data.Models;


public class RequestQueryModel : QueryStringParameters
{
    public RequestQueryModel()
    {

    }
    public string? Search { get; set; }
    public ApplicationStatus? Status { get; set; }
}
public class ModeratorApplicationApproveModel
{
    public bool IsApproved { get; set; }
    public string? Reason { get; set; }
}

public class RequestViewModel : BaseModel
{
    public Guid RegisterById { get; set; }
    public string? RegistrantName { get; set; }
    public string? RegistrantEmail { get; set; }
    public ApplicationStatus Status { get; set; }
    public string? Reason { get; set; }
    public Guid? ConfirmedById { get; set; }
    public string? ConfirmerName { get; set; }
}

