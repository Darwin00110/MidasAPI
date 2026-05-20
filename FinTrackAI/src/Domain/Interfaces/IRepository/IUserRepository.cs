namespace FinTrackAI;

public interface IUserRepository
{
    public Task<User> VerifyExistsUser(Guid id);
    public Task<bool> VerifyExistsUser_withEmail(string email);
    public Task<User> GetDataUserEmail(string email);
    public Task<bool> VerifyExistsUser_withCPF(string cpf);
    public Task<bool> VerifyExistsUser_withID(Guid id);
    public Task<bool> VerifyExistsUser_withTelephone(string telefone);

    public Task<User> CreateUser(User user);
    public Task<ReadResponse> ReadUser(Guid id);
    public Task<bool> UpdateUser(Guid id, User user);
    public Task<bool> DeleteUser(Guid id);

    public Task<List<GetAllUsersResponse>> GetAllUsers();
    public Task<bool> BlockAcessUser(Guid id);
    public Task<bool> UnlockedAcessUser(Guid id);

}
