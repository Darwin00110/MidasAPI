using FinTrackAI.src.Domain.Interfaces.IServices;

namespace FinTrackAI;

public class AdminUseCase_NoService : IAdminUseCase_NoService
{
    private readonly IUserRepository _repo;
    private readonly IHashService _hash;
    private readonly ITokenJWT _token;
    public AdminUseCase_NoService(IUserRepository userRepository, IHashService hash, ITokenJWT token)
    {
        _repo = userRepository;
        _hash = hash;
        _token = token;
    }
    public async Task<User> CreateAdmin(CreateAdminRequest request)
    {
        var verifyExistsUser = await _repo.VerifyExistsUser_withEmail(request.Email);
        if (verifyExistsUser)
        {
            throw new UseCaseException("Email ja cadastrado. ");
        }
        var VerifyExistsUserWithCPF = await _repo.VerifyExistsUser_withCPF(request.CPF);
        if (VerifyExistsUserWithCPF)
        {
            throw new UseCaseException("CPF ja cadastrado. ");
        }
        var hashedSenha = await _hash.HashPassword(request.Senha);
        User DataUser = new User
        {
            ID = Guid.NewGuid(),
            Nome = request.Nome,
            Email = request.Email,
            Data_nascimento = request.DataNascimento,
            Telefone = request.Telefone,
            CPF = request.CPF,
            Senha = hashedSenha.ToString(),
            Role = OptionsRole.ADMIN,
            StatusUsuario = OptionsStatusUser.ATIVO
        };
        DataUser.Validate_Create();
        var result = await _repo.CreateUser(DataUser);
        return result;
    }

    public async Task<ReadResponse> ReadAdmin(Guid id)
    {
        await _repo.VerifyExistsUser(id);
        var result = await _repo.ReadUser(id);
        return result;
    }
    public async Task<bool> UpdateAdmin(Guid id, UpdateAdminRequest request)
    {
        var VerifyUser_ID = await _repo.VerifyExistsUser_withID(id);
        if (!VerifyUser_ID)
        {
            throw new UseCaseException("Usuario não existe");
        }
        var VerifyUser_Email = await _repo.VerifyExistsUser_withEmail(request.Email);
        if (VerifyUser_Email)
        {
            throw new UseCaseException("Email ja cadastrado");
        }
        var VerifyUser_Telefone = await _repo.VerifyExistsUser_withTelephone(request.Telefone);
        if (VerifyUser_Telefone)
        {
            throw new UseCaseException("Telefone ja cadastrado");
        }
        var hashedSenha = await _hash.HashPassword(request.Senha);
        User DataUser = new User
        {
            Email = request.Email,
            Telefone = request.Telefone,
            Senha = hashedSenha
        };
        DataUser.Validate_Senha();
        DataUser.Validate_Email();
        DataUser.Validate_Telefone();
        var result = await _repo.UpdateUser(id, DataUser);
        return result;
    }
    public async Task<bool> DeleteAdmin(Guid id)
    {
       await _repo.VerifyExistsUser(id);
       var result = await _repo.DeleteUser(id);
       return result;
    }

    public async Task<List<GetAllUsersResponse>> ReadAllUsers()
    {
        var result = await _repo.GetAllUsers();
        return result;
    }

    public async Task<bool> BlockAcessUser(Guid id)
    {
        var verifyUserData = _repo.VerifyExistsUser(id);
        if (verifyUserData.Result.StatusUsuario == OptionsStatusUser.DESATIVADO)
        {
            throw new UseCaseException("Usuario já esta bloqueado");
        }
        var result = await _repo.BlockAcessUser(id);
        return result;
    }
    public async Task<string> LoginAdm(LoginUserRequest request)
    {
        var GetDataUser = await _repo.GetDataUserEmail(request.Email);
        var VerifyPasswordhash = await _hash.VerifyPassword(request.Senha, GetDataUser.Senha);
        if (!VerifyPasswordhash)
        {
            throw new UseCaseException("Senha incorreta.");
        }
        User usuario = new User
        {
            ID = GetDataUser.ID,
            Email = GetDataUser.Email,
            Nome = GetDataUser.Nome,
            Role = GetDataUser.Role,
            StatusUsuario = GetDataUser.StatusUsuario
        };
        usuario.Validate_LoginAdm();
        var GenerateToken = await _token.GenerativeToken(usuario);
        return GenerateToken;
    }
    public async Task<bool> UnlockedAcessUser(Guid id)
    {
        var verifyUserData = _repo.VerifyExistsUser(id);
        if (verifyUserData.Result.StatusUsuario == OptionsStatusUser.ATIVO)
        {
            throw new UseCaseException("Usuario já esta desbloqueado");
        }
        var result = await _repo.UnlockedAcessUser(id);
        return result;
    }
    public async Task<ReadResponse> GetDataUser_Adm(Guid id)
    {
        var VerifyExistsUser = await _repo.VerifyExistsUser_withID(id);
        if (!VerifyExistsUser)
        {
            throw new UseCaseException("Usuario não existe");
        }
        var result = await _repo.ReadUser(id);
        return result;
    }
}
