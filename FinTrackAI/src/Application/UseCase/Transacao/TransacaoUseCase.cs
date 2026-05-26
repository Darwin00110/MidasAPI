using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public class TransacaoUseCase : ITransacaoUseCase
{
    private readonly ITransacaoRepository _transferRepo;
    private readonly IUserRepository _repo;
    public TransacaoUseCase(IUserRepository repo, ITransacaoRepository transferRepo)
    {
        _repo = repo;
        _transferRepo = transferRepo;
    }
    public async Task<Transacao> CriarTransacao(Guid ID, CriarTransacaoRequest request)
    {
        var verifyUserTransfer = await _transferRepo.GetUserByKey(request.Chave_Alvo);
        return new Transacao{

        };
    }
}
