using FinTrackAI.src.Domain.Entities;

namespace FinTrackAI;

public enum OptionsTipoDaConta
{
    CORRENTE = 1,
    POUPANÇA = 2,
    CONTA_SALARIO = 3,
    CONTA_PAGAMENTO = 4
}

public class Accounts
{
    public Guid ID { get; set; }
    public Guid UserID { get; set; }
    public string NumeroConta { get; set; } = string.Empty;
    public string NumeroAgencia { get; set; } = string.Empty;
    public string ChavePix { get; set; }
    public decimal Saldo { get; set; }
    public OptionsTipoDaConta TipoConta { get; set; }
    public OptionsStatus Status { get; set; }
    public DateTime CriadoEm { get; set; }

    // Navegação
    public User? User { get; set; }
    public ICollection<Transacao> TransacoesEnviadas { get; set; } = new List<Transacao>();
    public ICollection<Transacao> TransacoesRecebidas { get; set; } = new List<Transacao>();

    public void Validate_Conta()
    {
        if (UserID == Guid.Empty)
            throw new DomainException("ID do usuário é inválido.");
        if (string.IsNullOrWhiteSpace(NumeroConta))
            throw new DomainException("Número da conta é obrigatório.");
        if (string.IsNullOrWhiteSpace(NumeroAgencia))
            throw new DomainException("Número da agência é obrigatório.");
        if (Saldo < 0)
            throw new DomainException("Saldo não pode ser negativo.");
        if (!Enum.IsDefined(typeof(OptionsTipoDaConta), TipoConta))
            throw new DomainException("Tipo de conta inválido.");
    }
}
