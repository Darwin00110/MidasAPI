using System.ComponentModel.DataAnnotations;
using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public class TransferirRequest
{
    [Required(ErrorMessage = "Chave é obrigatoria para continuar")]
    public required string Chave_Alvo { get; set; }
    [Range(typeof(decimal), "0,01", "999999999999,99" ,ErrorMessage = "Valor deve ser maior que zero.")]
    [Required(ErrorMessage = "Valor é obrigatorio para continuar")]
    public required decimal Valor { get; set; }
    public string? Descricao { get; set; } = string.Empty;
}
