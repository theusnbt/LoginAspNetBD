namespace LoginAspNetBD.Models;

public class UserViewModel
{
    public int Id {get; set;}
    public string Email {get; set;}
    public string SenhaHash {get; set;}
    public string Nome {get; set;}
}