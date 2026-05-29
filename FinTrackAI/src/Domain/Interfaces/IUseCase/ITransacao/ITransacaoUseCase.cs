using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public interface ITransacaoUseCase
{
    public Task<TransferirResponse> Transferir(TransferirRequest request, Guid UserID);
    public Task<DepositarResponse> Depositar (DepositarRequest request, Guid UserID);
    public Task<decimal> Sacar(SacarRequest request, Guid UserID);
    public Task<ExtratoResponse> Extrato(Guid UserID);
    public Task<TransacaoExtratoDTO> ExtratoPorID(Guid transacaoID, Guid UserID);
}
