namespace MidasAPI.src.Domain.Interfaces.IUserUseCase
{
    public interface IUserUseCase_WithService
    {
        public Task<User> CreateUser_withCPF(CreateUserRequest_withCPF request);
        public Task<ReadResponse> ReadUser_withCPF(Guid id);
    }
}
