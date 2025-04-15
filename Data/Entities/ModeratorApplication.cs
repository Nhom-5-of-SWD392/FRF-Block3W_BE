using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class ModeratorApplication : BaseEntities
	{
		[Required(ErrorMessage = "Reason is required")]
		public string Reason { get; set; } = null!;

		//TODO: Add validation to check if the user is younger than 18 years old
		[Required(ErrorMessage = "DateOfBirth is required")]
		public DateTime DateOfBirth { get; set; }

		public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

		//Foreign Keys 
		public Guid UserId { get; set; } //Which user create this application

		[ForeignKey("UserId")]
		public User? User { get; set; } 

		public Guid? ConfirmId { get; set; } //Which admin review this application
		[ForeignKey("ConfirmId")]
		public User? Admin { get; set; } 

	}
}
