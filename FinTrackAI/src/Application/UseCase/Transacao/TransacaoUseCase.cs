using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public class TransacaoUseCase : ITransacaoUseCase
{
    private readonly ITransacaoRepository _transferRepo;
    private readonly IUserRepository _repo;
    private readonly IAccountsRepository _accountsRepo;
    public TransacaoUseCase(IUserRepository repo, ITransacaoRepository transferRepo, IAccountsRepository accountsRepo)
    {
        _repo = repo;
        _transferRepo = transferRepo;
        _accountsRepo = accountsRepo;
    }
    public async Task<Transacao> CriarTransacao(CriarTransacaoRequest request, Guid userID)
    {
        var ContaOrigem = await _accountsRepo.GetDataAccounts(userID);
        var Data = new Transacao
        {
            
        };
        return Data;
    }
}
