namespace FinTrackAI;

public interface IAccountsUseCase_NoService
{
    public Task<bool> CreateAccount(Guid id, CreateAccountRequest request);
    public Task<string> GetSaldo(Guid id);
    public Task<AccountsGetDataUserResponse> GetDataUser(Guid id);
}
