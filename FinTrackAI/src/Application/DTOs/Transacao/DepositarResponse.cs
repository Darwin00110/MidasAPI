using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public class DepositarResponse
{
    public Guid ID { get; set; }
    public decimal Saldo_Anterior { get; set; }
    public decimal Saldo_Atual { get; set; }
    public DateTime EfetuadoEm { get; set; }
    public OptionsStatusDaTransferencia Status { get; set; }
    public string? Descricao { get; set; }
}
