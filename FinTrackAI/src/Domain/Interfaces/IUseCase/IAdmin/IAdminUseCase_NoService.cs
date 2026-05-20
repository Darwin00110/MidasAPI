namespace FinTrackAI;

public interface IAdminUseCase_NoService
{
    public Task<User> CreateAdmin(CreateAdminRequest request);
    public Task<string> LoginAdm(LoginUserRequest request);
    public Task<ReadResponse> ReadAdmin(Guid id);
    public Task<bool> UpdateAdmin(Guid id, UpdateAdminRequest request);
    public Task<bool> DeleteAdmin(Guid id);


    public Task<List<GetAllUsersResponse>> ReadAllUsers();
    public Task<ReadResponse> GetDataUser_Adm(Guid id);
    public Task<bool> BlockAcessUser(Guid id);
    public Task<bool> UnlockedAcessUser(Guid id);

}
