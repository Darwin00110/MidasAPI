using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public class TransacaoExtratoDTO
{
    public Guid ID { get; set; }
    public string Protocolo { get; set; }
    public decimal Valor { get; set; }
    public decimal SaldoOrigemAntes { get; set; }
    public decimal SaldoOrigemDepois { get; set; }
    public string Tipo { get; set; }
    public string Status { get; set; }
    public string ChavePix_ALVO { get; set; }
    public string? Descricao { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? ConcluidoEm { get; set; }
}
public class ExtratoResponse
{
    public List<TransacaoExtratoDTO> TransacaoEnviada { get; set; }
    public List<TransacaoExtratoDTO> TransacaoRecebida { get; set; }
}
