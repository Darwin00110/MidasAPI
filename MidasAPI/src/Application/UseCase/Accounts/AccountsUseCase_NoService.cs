using MidasAPI.src.Domain.Entities;

namespace MidasAPI;

public class AccountsUseCase_NoService : IAccountsUseCase_NoService
{
    private readonly IAccountsRepository _repoBank;
    private readonly IUserRepository _repoUser;
    public AccountsUseCase_NoService(IAccountsRepository repoBank, IUserRepository repoUser)
    {
        _repoBank = repoBank;
        _repoUser = repoUser;
    }
    public async Task<bool> CreateAccount(Guid idUser, CreateAccountRequest request)
    {
        var verifyUserExists = await _repoUser.VerifyExistsUser(idUser);
        if (!verifyUserExists)
            throw new UseCaseException("Usuario não existe.");
        var verifyAccountsExists = await _repoBank.VerifyAccountExists(idUser);
        if (verifyAccountsExists)
        {
            throw new UseCaseException("Conta ja cadastrada .");
        }
        var GetDataUser = await _repoUser.GetDataUser(idUser);
        if (GetDataUser.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Usuario desativado., entre em contato com o suporte.");
        if (GetDataUser.Email != request.Email)
            throw new UseCaseException("Email incorreto, tente novamente mais tarde.");

        Accounts Data = new Accounts
        {
            ID = Guid.NewGuid(),
            UserID = idUser,
            Status = OptionsStatus.ATIVO,
            Saldo = 0,
            ChavePix = GetDataUser.CPF,
            NumeroAgencia = request.NumeroAgencia,
            TipoConta = request.TipoDaConta,
            NumeroConta = new Random().Next(100000, 999999).ToString(),
            CriadoEm = DateTime.Now,
            User = GetDataUser,
            TransacoesEnviadas = new List<Transacao>(),
            TransacoesRecebidas = new List<Transacao>()
        };
        var result = await _repoBank.CreateAccount(Data);
        return result;
    }
    public async Task<string> GetSaldo(Guid id_Conta)
    {
        var verifyUserExists = await _repoUser.VerifyExistsUser(id_Conta);
        if (!verifyUserExists)
            throw new UseCaseException("Usuario não existe.");

        var verifyAccountsExists = await _repoBank.VerifyAccountExists(id_Conta);
        if (!verifyAccountsExists)
            throw new UseCaseException("Conta não existe.");
        var GetDataUser = await _repoUser.GetDataUser(id_Conta);
        if (GetDataUser.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Usuario desativado, entre em contato com o suporte.");
        var GetDataAccount = await _repoBank.GetDataAccounts(id_Conta);
        if (GetDataAccount.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Conta desativada, entre em contato com o suporte.");
        return await _repoBank.GetSaldo(id_Conta);
    }

    public async Task<AccountsGetDataUserResponse> GetDataConta(Guid id)
    {
        var verifyUserExists = await _repoUser.VerifyExistsUser(id);
        if (!verifyUserExists)
            throw new UseCaseException("Usuario não existe.");

        var verifyAccountsExists = await _repoBank.VerifyAccountExists(id);
        if (!verifyAccountsExists)
            throw new UseCaseException("Conta não existe.");

        var GetDataUser = await _repoUser.GetDataUser(id);
        if (GetDataUser.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Usuario desativado, entre em contato com o suporte.");

        var GetDataAccount = await _repoBank.GetDataAccounts(id);
        if (GetDataAccount.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Conta desativada, entre em contato com o suporte.");

        return new AccountsGetDataUserResponse
        {
            Nome = GetDataUser.Nome,
            N_Agencia = GetDataAccount.NumeroAgencia,
            Saldo = GetDataAccount.Saldo.ToString(),
            Chave_PIX = GetDataAccount.ChavePix,
            TransacaoEnviadas = GetDataAccount.TransacoesEnviadas,
            TransacaoRecebidas = GetDataAccount.TransacoesRecebidas
        };
    }
}
