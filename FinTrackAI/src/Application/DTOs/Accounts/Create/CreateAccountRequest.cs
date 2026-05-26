using System.ComponentModel.DataAnnotations;

namespace FinTrackAI;

public class CreateAccountRequest
{
    [Required(ErrorMessage = "Tipo da conta é obrigatoria")]
    public string? TipoDaConta { get; set; }
    [Required(ErrorMessage = "Numero da conta é obrigatoria")]
    public string? NumeroConta { get; set; }
    [Required(ErrorMessage = "Numero da agencia é obrigatoria")]
    public string NumeroAgencia { get; set; } = string.Empty;
    [Required(ErrorMessage = "Chave_PIX é obrigatoria")]
    public string ChavePix { get; set; } = string.Empty;

    
}
