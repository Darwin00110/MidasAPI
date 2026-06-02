using System.ComponentModel.DataAnnotations;

namespace MidasAPI;

public class CreateUserRequest_withCPF
{
    [Required(ErrorMessage = "Email é obrigatorio para continuar")]
    [EmailAddress(ErrorMessage = "Formato de email invalido.")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Senha é obrigatoria para continuar")]
    public string Senha { get; set; }
    [Required(ErrorMessage = "Telefone é obrigatorio para continuar")]
    public string Telefone { get; set; }
    [Required(ErrorMessage = "CPF é obrigatorio para continuar")]
    public string CPF { get; set; }
}


