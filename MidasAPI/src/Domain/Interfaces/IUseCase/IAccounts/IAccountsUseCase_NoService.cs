namespace MidasAPI;

public interface IAccountsUseCase_NoService
{
    public Task<bool> CreateAccount(Guid idUser, CreateAccountRequest request);
    public Task<string> GetSaldo(Guid id_Conta);
    public Task<AccountsGetDataUserResponse> GetDataConta(Guid id);
}
