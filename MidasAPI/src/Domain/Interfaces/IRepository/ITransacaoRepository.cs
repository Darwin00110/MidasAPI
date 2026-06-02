using MidasAPI.src.Domain.Entities;

namespace MidasAPI;

public interface ITransacaoRepository
{
    public Task<TransferirResponse> Transferir(Transacao Data, Guid UserID);
    public Task<DepositarResponse> Depositar(Transacao Data, Guid UserID);
    public Task<decimal> Sacar(Transacao Data);
    public Task<ExtratoResponse> Extrato(Guid UserID);
    public Task<TransacaoExtratoDTO> ExtratoPorID(Guid transacaoID, Guid UserID);
}
