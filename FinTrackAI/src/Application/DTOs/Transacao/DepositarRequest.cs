using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public class DepositarRequest
{
    public string? Chave_Alvo { get; set; }
    public decimal Valor { get; set; }
    public string? Descricao { get; set; } = string.Empty;
}
