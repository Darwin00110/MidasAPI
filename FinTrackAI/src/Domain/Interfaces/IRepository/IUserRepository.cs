namespace FinTrackAI;

public interface IUserRepository
{
    public Task<bool> VerifyExistsUser(Guid id);
    public Task<bool> VerifyExistsUser_withEmail(string email);
    public Task<bool> VerifyExistsUser_withCPF(string CPF);
   

    public Task<User> CreateUser(User user);
    public Task<ReadResponse> ReadUser(Guid id);
    public Task<bool> UpdateUser(Guid id, User user);
    public Task<bool> DeleteUser(Guid id);

    public Task<List<GetAllUsersResponse>> GetAllUsers();
    public Task<User> GetDataUser(Guid id);
    public Task<User> GetDataUserEmail(string email);
    public Task<bool> PatchUpdateUser(Guid id, User user);

    public Task<bool> BlockAcessUser(Guid id);
    public Task<bool> UnlockedAcessUser(Guid id);

    public Task<bool> BlockAcessAccount(Guid id);
    public Task<bool> UnlockedAcessAccount(Guid id);
}
