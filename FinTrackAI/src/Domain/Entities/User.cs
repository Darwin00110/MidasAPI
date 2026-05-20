using System.Data;
using System.Xml.Linq;

namespace FinTrackAI;

public enum OptionsRole
{
    USER,
    ADMIN
}

public enum OptionsStatusUser
{
    ATIVO,
    DESATIVADO
}

public class User
{
    public Guid ID { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Data_nascimento { get; set; }
    public string? Senha { get; set; }
    public OptionsRole Role { get; set; }
    public string? Telefone { get; set; }
    public string? CPF { get; set; }
    public OptionsStatusUser StatusUsuario { get; set; }

    public void Validate_Create()
    {
        Validate_ID();
        Validate_Nome();
        Validate_Email();
        Validate_Senha();
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
    public void Validate_Senha()
    {
        if (string.IsNullOrEmpty(Senha))
        {
            throw new DomainException("Senha não pode estar vazio.");
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
    public void Validate_DataNascimento()
    {
        if (string.IsNullOrWhiteSpace(Data_nascimento))
        {
            throw new DomainException("Data nascimento não pode estar vazio.");
        }
        if (!Data_nascimento.Contains("//"))
        {
            throw new DomainException("Data nascimento invalida, ex: ( 30/01/2008 )");
        }
    }
}