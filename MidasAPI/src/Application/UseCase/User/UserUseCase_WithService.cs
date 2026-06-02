using MidasAPI.src.Domain.Interfaces.IUserUseCase;

namespace MidasAPI.src.Application.UseCase;

public class UserUseCase_WithService : IUserUseCase_WithService
{
    private readonly IUserRepository _repo;
    private readonly ICPF_USER _validate_cpf;
    public UserUseCase_WithService(IUserRepository repo, ICPF_USER validate_cpf)
    {
        _repo = repo;
        _validate_cpf = validate_cpf;
    }

    public async Task<User> CreateUser_withCPF(CreateUserRequest_withCPF request)
    {
        var GetDataUser_cpf = await _validate_cpf.GetDataUser(request.CPF);
        if (GetDataUser_cpf == null)
            throw new UseCaseException("CPF invalido.");
        var DataUser = new User
        {
            ID = Guid.NewGuid(),
            Role = OptionsRole.USER,
            Email = request.Email,
            PasswordHash = request.Senha,
            CPF = request.CPF,
            Telefone = request.Telefone,
            Nome = GetDataUser_cpf.Data.Nome,
            DataNascimento = DateTime.Parse(GetDataUser_cpf.Data.Data_nascimento),
        };
        DataUser.Validate_Create();
        var result = await _repo.CreateUser(DataUser);
        return result;
    }
    public async Task<ReadResponse> ReadUser_withCPF(Guid id)
    {
        await _repo.VerifyExistsUser(id);

        var result = await _repo.ReadUser(id);
        return result;
    }
}
