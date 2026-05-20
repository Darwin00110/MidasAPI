namespace FinTrackAI.src.Domain.Interfaces.UserUseCase_NoService
{
    public interface IUserUseCase_NoService
    {
        public Task<User> CreateUser(CreateUserRequest request);
        public Task<ReadResponse> ReadUser(Guid id);
        public Task<bool> UpdateUser(Guid id, UpdateUserRequest request);
        public Task<bool> DeleteUser(Guid id);
        public Task<string> LoginUser(LoginUserRequest request);
    }
}
