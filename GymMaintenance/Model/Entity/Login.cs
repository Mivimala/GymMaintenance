using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class Login
    {

        [Key]
        public int LoginId { get; set; }

        public string? Role { get; set; }

        public string? UserName { get; set; }
       
        public string? Password { get; set; }
        
    }
}
