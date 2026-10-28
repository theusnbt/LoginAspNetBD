using Microsoft.AspNetCore.Mvc;
using LoginAspNetBD.Models.LoginViewModel;

namespace LoginAspNetBD.Controllers
{
    public class ContaController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public ContaController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string nome, string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Preencha todos os campos.";

                return View();
            }

            var usuarioExistente = await _usuarioRepositorio.BuscarEmail(email);

            if (usuarioExistente != null)
            {
                ViewBag.Erro = "Este e-mail já está cadastrado.";
                return View();
            }

            var novoUsuario = new UsuarioModel { Nome = nome, Email = email };
            var sucesso = await _usuarioRepositorio.Cadastrar(novoUsuario, senha);

            if (sucesso)
            {
                TempData["Sucesso"] = "Cadastro realizado com sucesso! Faça o login.";
                return RedirectToAction(nameof(Login));
            }

            ViewBag.Erro = "Erro ao cadastrar usuário.";
            return View();
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _usuarioRepositorio.BuscarEmail(model.Email);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View(model);
            }

            var passwordHasher = new PasswordHasher<UsuarioModel>();
            var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, model.Senha);

            if (resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}