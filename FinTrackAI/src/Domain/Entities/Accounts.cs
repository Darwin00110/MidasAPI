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

    public string ChavePix { get; set; } = string.Empty;

    public decimal Saldo { get; set; }

    public OptionsTipoDaConta TipoConta { get; set; }

    public OptionsStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public User User { get; set; }

    public ICollection<Transacao> TransacoesEnviadas { get; set; }
        = new List<Transacao>();
    public ICollection<Transacao> TransacoesRecebidas { get; set; }
        = new List<Transacao>();
    public void Validate_TipoDaConta(string tipo_conta)
    {
        if (!Enum.TryParse<OptionsTipoDaConta>(tipo_conta, out var tipoParsed) &&
            !Enum.IsDefined(typeof(OptionsTipoDaConta), tipoParsed))
        {
            throw new DomainException("Tipo de conta invalido, Ex: ( CORRENTE, POUPANÇA, CONTA_SALARIO, CONTA_PAGAMENTO )");
        }

        TipoConta = tipoParsed; 
    }
}
