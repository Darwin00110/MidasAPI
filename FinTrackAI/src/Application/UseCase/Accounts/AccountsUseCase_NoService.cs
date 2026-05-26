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
    public async Task<bool> CreateAccount(Guid id, CreateAccountRequest request)
    {
        var verifyUserExists = await _repoUser.VerifyExistsUser_withID(id);
        if (!verifyUserExists)
            throw new UseCaseException("Usuario não existe.");
        Accounts Data = new Accounts
        {
            ID = Guid.NewGuid(),
            UserID = id,
            Status = OptionsStatus.ATIVO,
            Saldo = 0

        };
        Data.Validate_TipoDaConta(request.TipoDaConta);

        var result = await _repoBank.CreateAccount(Data);
        return result;
    }
    public async Task<string> GetSaldo(Guid id)
    {
        var verifyUserExists = await _repoUser.VerifyExistsUser_withID(id);
        if (!verifyUserExists)
            throw new UseCaseException("Usuario não existe.");
        return await _repoBank.GetSaldo(id);
    }
    public async Task<AccountsGetDataUserResponse> GetDataUser(Guid id)
    {
        var verifyUserExists = await _repoUser.VerifyExistsUser_withID(id);
        if (!verifyUserExists)
        {
            throw new UseCaseException("Usuario não existe. ");
        }
        var result = await _repoUser.VerifyExistsUser(id);
        var GetDataAccount = await _repoBank.GetDataAccounts(id);
        return new AccountsGetDataUserResponse
        {
            Nome = result.Nome,
            Saldo = GetDataAccount.Saldo.ToString(),
        };
    }
}
