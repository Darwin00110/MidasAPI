using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public interface ITransacaoRepository
{
    public Task<Transacao> CriarTransacao(Guid ID, CriarTransacaoRequest request);
    public Task<Transacao> GetUserByKey(string key);
}
