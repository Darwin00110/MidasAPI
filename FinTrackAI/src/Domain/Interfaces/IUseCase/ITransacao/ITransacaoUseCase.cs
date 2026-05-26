using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public interface ITransacaoUseCase
{
    public Task<Transacao> CriarTransacao(Guid ID, CriarTransacaoRequest request);
}
