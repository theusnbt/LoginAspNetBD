namespace LoginAspNetBD.Repositories;

public interface IUserViewModel
{
    Task<bool> Cadastrar(UsuarioModel usuario, string senhaPura);
    
    Task<UsuarioModel?> BuscarEmail(string email);
}