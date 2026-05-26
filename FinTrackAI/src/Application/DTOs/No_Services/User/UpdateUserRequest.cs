using System.ComponentModel.DataAnnotations;

namespace FinTrackAI;

public class UpdateUserRequest
{
    [EmailAddress(ErrorMessage = "Formato de email invalido.")]
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Senha { get; set; }
}
