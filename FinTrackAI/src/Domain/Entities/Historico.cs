using System.Diagnostics.Tracing;

namespace FinTrackAI;

public enum OptionsTipoDaTransferencia
{
    PIX,
    TED,
    DOC,
    Deposito,
    Boleto,
}
public enum OptionsStatusTransacao
{
    CONCLUIDA,
    PENDENTE,
    CANCELADA,
    FALHOU
}
public class Historico
{
    public Guid ID { get; set; }
    public Guid UserID { get; set; }
    public DateTime DiaTransferencia { get; set; }
    public DateTime HorarioTransferencia { get; set; }
    public string? Remetente { get; set; }
    public string? Destinatario { get; set; }
    public string? Banco_Instituicao { get; set; }
    public string? Chave_PIX { get; set; }
    public OptionsStatusTransacao Status { get; set; }
    public string? Descricao { get; set; }
    public string? Saldo_Anterior { get; set; }
    public string? Saldo_Posterior { get; set; }
    public string? Tarifa { get; set; }
    public string? Comprovante { get; set; }
    public decimal ValorTransferencia { get; set; }
    public OptionsTipoDaTransferencia TipoDaTransferencia { get; set; }
    
}
