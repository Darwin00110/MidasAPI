using System.ComponentModel.DataAnnotations;

namespace FinTrackAI;

public class CreateAccountRequest
{
    [Required(ErrorMessage = "Tipo da conta é obrigatoria")]
    public OptionsTipoDaConta TipoDaConta { get; set; }
    [Required(ErrorMessage = "Numero da agencia é obrigatoria")]
    public string NumeroAgencia { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email é obrigatorio")]
    public required string Email { get; set; }
}
