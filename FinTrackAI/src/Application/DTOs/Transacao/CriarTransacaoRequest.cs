using System.ComponentModel.DataAnnotations;
using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public class CriarTransacaoRequest
{
    [Required(ErrorMessage = "Chave é obrigatoria para continuar")]
    public string Chave_Alvo { get; set; }
    [Required(ErrorMessage = "Valor é obrigatorio para continuar")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
    [Required(ErrorMessage = "Tipo de transação é obrigatorio para continuar")]
    public OptionsTipoDaTransferencia TipoTransacao { get; set; }
    public string? Descricao { get; set; } = string.Empty;
}
