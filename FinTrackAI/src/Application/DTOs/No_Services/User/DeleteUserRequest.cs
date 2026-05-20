using System.ComponentModel.DataAnnotations;

namespace FinTrackAI;

public class DeleteUserRequest
{
    [Required(ErrorMessage = "Email é obrigatorio para continuar")]
    [EmailAddress(ErrorMessage = "Formato de Email invalido. Ex: ( exemplo@gmail.com ) ")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Nome é obrigatorio para continuar")]
    public string Nome { get; set; }
}
