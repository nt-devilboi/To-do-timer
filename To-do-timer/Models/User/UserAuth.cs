using System.ComponentModel.DataAnnotations;

namespace To_do_timer.Models;

public class UserAuth
{
    [Required(ErrorMessage = "фигня давай по новой")] public string Username { get; set; }
    [Required(ErrorMessage = "это не привильная")] [EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }
}