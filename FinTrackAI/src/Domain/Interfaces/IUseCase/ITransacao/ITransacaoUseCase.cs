using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public interface ITransacaoUseCase
{
    public Task<Transacao> CriarTransacao(CriarTransacaoRequest request, Guid userID);
}
