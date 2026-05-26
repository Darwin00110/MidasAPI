using System.Security.Principal;
using System.Transactions;

namespace FinTrackAI.src.Domain.Entities
{
    public enum OptionsTipoTransacao
    {
        DESPESA = 0,
        RECEITA = 1
    }

    public class Transacao
    {
        public Guid ID { get; set; }

        public Guid SenderAccountId { get; set; }

        public Guid ReceiverAccountId { get; set; }

        public decimal Amount { get; set; }

        public OptionsTipoDaTransferencia Type { get; set; }

        public TransactionStatus Status { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public Accounts SenderAccount { get; set; }

        public Accounts ReceiverAccount { get; set; }
        private void Validate_ID()
        {
            if (ID == Guid.Empty)
            {
                throw new DomainException("ID da transação é inválido.");
            }
        }
        private void Validate_SenderAccountId()
        {
            if (SenderAccountId == Guid.Empty)
            {
                throw new DomainException("ID da conta remetente é inválido.");
            }
        }
        private void Validate_ReceiverAccountId()
        {
            if (ReceiverAccountId == Guid.Empty)
            {
                throw new DomainException("ID da conta destinatária é inválido.");
            }
        }
        private void Validate_Amount()
        {
            if (Amount <= 0)
            {
                throw new DomainException("Valor da transação deve ser maior que zero.");
            }
        }

    }
}
