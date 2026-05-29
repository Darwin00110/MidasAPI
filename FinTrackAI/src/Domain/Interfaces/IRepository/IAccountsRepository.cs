namespace FinTrackAI;

public interface IAccountsRepository
{
    public Task<bool> CreateAccount(Accounts Data);
    public Task<string> GetSaldo(Guid id);
    public Task<Accounts> GetDataAccounts(Guid id);
    public Task<bool> VerifyAccountExists(Guid id);
    public Task<Accounts> GetDataAccounts_WithKey(string CPF);
}
