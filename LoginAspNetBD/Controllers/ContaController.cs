using Microsoft.AspNetCore.Mvc;

namespace LoginAspNetBD.Controllers;

public class ContaController : Controller
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ContaController(IUsuarioRepositorio usuarioRepositorio) // criando método que recebe o atributo privado
    {
        _usuarioRepositorio = usuarioRepositorio;
    }
}