using System.ComponentModel.DataAnnotations;

namespace To_do_timer.Models;

public class UserAuth
{
    [Required] public string Username { get; set; }
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }
}