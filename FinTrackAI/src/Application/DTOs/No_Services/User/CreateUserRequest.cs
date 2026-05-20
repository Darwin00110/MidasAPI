using System.ComponentModel.DataAnnotations;

namespace FinTrackAI;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Nome é obrigatorio para continuar")]
    public string? Nome { get; set; }
    [Required(ErrorMessage = "Email é obrigatorio para continuar")]
    [EmailAddress(ErrorMessage = "Formato invalido do Email, ex: (exemplo@gmail.com)")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Data de nascimento é obrigatorio para continuar")]
    public string? DataNascimento { get; set; }
    [Required(ErrorMessage = "Telefone é obrigatorio para continuar")]
    public string? Telefone { get; set; }
    [Required(ErrorMessage = "CPF é obrigatorio para continuar")]
    public string? CPF { get; set; }
    [Required(ErrorMessage = "Senha é obrigatorio para continuar")]
    public string? Senha { get; set; }
}
