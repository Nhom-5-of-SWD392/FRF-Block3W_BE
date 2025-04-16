using Data.Enum;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Entities;

public class ModeratorApplication : BaseEntities
{
	public string? Reason { get; set; }
	public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

	//Foreign Keys 
	public Guid RegisterById { get; set; }
	[ForeignKey("RegisterById")]
	public User? Registrant { get; set; } 

	public Guid? ConfirmedById { get; set; }
	[ForeignKey("ConfirmedById")]
	public User? Confirmer { get; set; } 
}
