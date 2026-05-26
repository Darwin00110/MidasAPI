namespace FinTrackAI;

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
        Accounts Data = new Accounts
        {
            ID = Guid.NewGuid(),
            UserID = idUser,
            Status = OptionsStatus.ATIVO,
            Saldo = 0

        };

        var result = await _repoBank.CreateAccount(Data);
        return result;
    }
    public async Task<string> GetSaldo(Guid id_Conta)
    {
        var verifyUserExists = await _repoUser.VerifyExistsUser(id_Conta);
        if (!verifyUserExists)
            throw new UseCaseException("Usuario não existe.");
        return await _repoBank.GetSaldo(id_Conta);
    }
    public async Task<AccountsGetDataUserResponse> GetDataUser(Guid id)
    {
        var verifyUserExists = await _repoUser.VerifyExistsUser(id);
        if (!verifyUserExists)
        {
            throw new UseCaseException("Usuario não existe. ");
        }
        var result = await _repoUser.VerifyExistsUser(id);
        var GetDataAccount = await _repoBank.GetDataAccounts(id);
        return new AccountsGetDataUserResponse
        {
            Nome = GetDataAccount.User.Nome,
            Saldo = GetDataAccount.Saldo.ToString(),
        };
    }
}
