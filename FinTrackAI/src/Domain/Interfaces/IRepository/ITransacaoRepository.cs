using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public interface ITransacaoRepository
{
    Task<Transacao> ExecutarTransferencia(
        Transacao transferencia,
        Accounts contaOrigem,
        Accounts contaDestino,
        decimal totalDebito);

    Task<Transacao?> BuscarPorId(Guid id);
    Task<Transacao?> BuscarPorProtocolo(string protocolo);
    Task<List<Transacao>> ListarPorConta(Guid contaId);
}
