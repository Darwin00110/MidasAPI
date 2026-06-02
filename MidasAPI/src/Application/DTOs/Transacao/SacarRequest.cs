using System.ComponentModel.DataAnnotations;

namespace MidasAPI;

public class SacarRequest
{
    [Required(ErrorMessage = "Chave é obrigatoria para continuar")]
    public string? Chave_Alvo { get; set; }
    [Required(ErrorMessage = "Valor é obrigatorio para continuar")]
    [Range(0, int.MaxValue, ErrorMessage = "Valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
    public string? Descricao { get; set; } = string.Empty;
    [Required(ErrorMessage = "CPF é obrigatorio para continuar")]
    public required string CPF { get; set; }
}
