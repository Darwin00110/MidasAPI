using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public class TransferirResponse
{
    public string? Nome_Origem {get; set;}
    public string? Nome_Destino {get; set;}
    public string? Descricao {get; set;}
    public decimal Valor {get; set;}
    public string? Tipo {get; set;}
    public string? Status {get; set;}
    public DateTime CriadoEm {get; set;}
    public string? ChavePix_Alvo {get; set;}
}
