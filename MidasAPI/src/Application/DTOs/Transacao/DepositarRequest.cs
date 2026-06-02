using MidasAPI.src.Domain.Entities;

namespace MidasAPI;

public class DepositarRequest
{
    public string? Chave_Alvo { get; set; }
    public decimal Valor { get; set; }
    public string? Descricao { get; set; } = string.Empty;
}
