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
            DataNascimento = DateTime.Parse(request.DataNascimento),
            Telefone = request.Telefone,
            CPF = request.CPF,
            PasswordHash = hashedSenha.ToString(),
            Role = OptionsRole.ADMIN,
            Status = OptionsStatus.ATIVO,
        };
        DataUser.Validate_Create();
        var result = await _repo.CreateUser(DataUser);
        return result;
    }


    public async Task<ReadResponse> ReadAdmin(Guid id)
    {
        var verifyUserExist = await _repo.VerifyExistsUser_withID(id);
        if (!verifyUserExist)
        {
            throw new UseCaseException("Usuario não existe. ");
        }
        var GetDataUser = await _repo.GetDataUser_ID(id);
        if (GetDataUser.Status == OptionsStatus.DESATIVADO)
        {
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");
        }
        var result = await _repo.ReadUser(id);
        return result;
    }
    public async Task<bool> UpdateAdmin(Guid id, UpdateAdminRequest request)
    {
        var GetDataUser = await _repo.GetDataUser_ID(id);
        if (GetDataUser.Status == OptionsStatus.DESATIVADO)
        {
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");
        }
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
            PasswordHash = hashedSenha
        };
        DataUser.Validate_Email();
        DataUser.Validate_Telefone();
        var result = await _repo.UpdateUser(id, DataUser);
        return result;
    }
    public async Task<bool> DeleteAdmin(Guid id)
    {
        var verifyUserExists = await _repo.VerifyExistsUser_withID(id);
        if (!verifyUserExists)
        {
            throw new UseCaseException("Usuario não existe. ");
        }
        var result = await _repo.DeleteUser(id);
        return result;
    }
    public async Task<bool> PathUpdateADM(Guid id, UpdateAdminRequest request)
    {
        var verifyUserExists = await _repo.VerifyExistsUser_withID(id);
        if (!verifyUserExists)
        {
            throw new UseCaseException("O usuario não existe");
        }
        var DataUser = await _repo.VerifyExistsUser(id);
        if (request.Email?.Equals(DataUser.Email) == true)
            throw new UseCaseException("Email ja cadastrado anteriormente.");

        if (request.Telefone?.Equals(DataUser.Telefone) == true)
            throw new UseCaseException("Telefone ja cadastrado anteriormente.");
        var user = new User
        {
            Email = request.Email ?? DataUser.Email,
            Telefone = request.Telefone ?? DataUser.Telefone,
            PasswordHash = request.Senha ?? DataUser.PasswordHash,
        };
        return await _repo.PatchUpdateUser(id, user);
    }
    public async Task<List<GetAllUsersResponse>> ReadAllUsers(Guid id)
    {
        var verifyUserExists = await _repo.VerifyExistsUser_withID(id);
        if (!verifyUserExists)
        {
            throw new UseCaseException("Usuario não existe. ");
        }
        var DataUser = await _repo.VerifyExistsUser(id);
        if (DataUser.Role != OptionsRole.ADMIN)
        {
            throw new UseCaseException("Operação negada, Usuario não é um administrador");
        }
        var result = await _repo.GetAllUsers();
        return result;
    }

    public async Task<bool> BlockAcessUser(Guid id)
    {
        var verifyUserData = await _repo.VerifyExistsUser(id);
        if (verifyUserData.Status == OptionsStatus.DESATIVADO)
        {
            throw new UseCaseException("Usuario já esta bloqueado");
        }
        var result = await _repo.BlockAcessUser(id);
        return result;
    }
    public async Task<string> LoginAdm(LoginUserRequest request)
    {
        var GetDataUser = await _repo.GetDataUserEmail(request.Email);
        if (GetDataUser.Status == OptionsStatus.DESATIVADO)
        {
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");
        }
        var VerifyPasswordhash = await _hash.VerifyPassword(request.Senha, GetDataUser.PasswordHash);
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
            Status = GetDataUser.Status
        };
        usuario.Validate_LoginAdm();
        var GenerateToken = await _token.GenerativeToken(usuario);
        return GenerateToken;
    }
    public async Task<bool> UnlockedAcessUser(Guid id)
    {
        var verifyUserData = await _repo.VerifyExistsUser(id);
        if (verifyUserData.Status == OptionsStatus.ATIVO)
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
