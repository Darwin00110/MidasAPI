
namespace FinTrackAI;

public enum OptionsRole
{
    USER,
    ADMIN
}

public enum OptionsStatus
{
    ATIVO,
    DESATIVADO
}

public class User
{
    public Guid ID { get; set; }
    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string CPF { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public DateTime DataNascimento { get; set; }

    public OptionsRole Role { get; set; }

    public OptionsStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public Accounts? Accounts { get; set; }

    public void Validate_Create()
    {
        Validate_ID();
        Validate_Nome();
        Validate_Email();
        Validate_Telefone();
        Validate_CPF();
    }

    public void Validate_LoginUser()
    {
        if (Role == OptionsRole.ADMIN)
        {
            throw new DomainException("Acesso negado, Usuario tem privilegios administrativos. ");
        }
        Validate_ID();
        Validate_Email();
        Validate_Nome();
    }

    public void Validate_LoginAdm()
    {
        if (Role == OptionsRole.USER)
        {
            throw new DomainException("Acesso negado. Usuário não é um administrador.");
        }
        Validate_ID();
        Validate_Email();
        Validate_Nome();
    }

    public void Validate_Nome()
    {
        if (string.IsNullOrEmpty(Nome))
        {
            throw new DomainException("Nome não pode estar vazio.");
        }
    }
    public void Validate_ID()
    {
        if (ID == Guid.Empty)
        {
            throw new DomainException("ID não pode ser vazio.");
        }
    }

    public void Validate_Email()
    {
        if (string.IsNullOrEmpty(Email))
        {
            throw new DomainException("Email não pode estar vazio.");
        }
        if (!Email.Contains("@gmail.com"))
        {
            throw new DomainException("Email esta no formato invalido, ex: (exemplo@gmail.com)");
        }
    }

    public void Validate_Telefone()
    {
        if (string.IsNullOrEmpty(Telefone))
        {
            throw new DomainException("Telefone não pode estar vazio.");
        }
        if (!Telefone.All(char.IsDigit))
        {
            throw new DomainException("Telefone deve conter somente numeros");
        }
        if (Telefone.Length != 9)
        {
            throw new DomainException("Telefone deve conter 9 caracteres");
        }
    }

    public void Validate_CPF()
    {
        if (string.IsNullOrEmpty(CPF))
        {
            throw new DomainException("CPF não pode estar vazio.");
        }
        if (!CPF.All(char.IsDigit))
        {
            throw new DomainException("CPF deve conter somente numeros");
        }
        if (CPF.Length != 11)
        {
            throw new DomainException("CPF deve conter 11 caracteres");
        }
    }
}