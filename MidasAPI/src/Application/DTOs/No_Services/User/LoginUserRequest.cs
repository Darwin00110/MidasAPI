using System.ComponentModel.DataAnnotations;

namespace MidasAPI;

public class LoginUserRequest
{
    [Required(ErrorMessage = "Email é obrigatorio")]
    [EmailAddress(ErrorMessage = "Formato do email invalido. Ex: ( exemplo@gmail.com ) ")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Senha é obrigatoria")]
    public string? Senha { get; set; }
}
