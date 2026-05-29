namespace FinTrackAI.src.Domain.Entities
{

    public enum OptionsStatusDaTransferencia
    {
        EFETUADA,
        PENDENTE,
        REJEITADA,
        CANCELADA
    }

    public enum OptionsTipoDaTransferencia
    {
        PIX,
        TED,
        DOC,
        Deposito,
        Saque,
        Transferencia,
        Estorno
    }
    public class Transacao
    {
        public Guid ID { get; set; }
        public Guid ContaOrigemId { get; set; }
        public Guid ContaDestinoId { get; set; }
        public string? Nome_Origem { get; set; }
        public string? Nome_Destino { get; set; }
        public string ChavePix_ALVO { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Taxa { get; set; }
        public decimal ValorLiquido { get; set; }
        public string? CPF { get; set; }
        public OptionsTipoDaTransferencia? Tipo { get; set; }
        public OptionsStatusDaTransferencia Status { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Protocolo { get; set; } = string.Empty;
        public decimal SaldoOrigemAntes { get; set; }
        public decimal SaldoOrigemDepois { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? ConcluidoEm { get; set; }
        public DateTime? CanceladoEm { get; set; }
        public string? MotivoCancelamento { get; set; }
        public Accounts? ContaOrigem { get; set; }
        public Accounts? ContaDestino { get; set; }
        private void Validate_ID()
        {
            if (ID == Guid.Empty)
                throw new DomainException("ID da transferência é inválido.");
        }

        private void Validate_ContaOrigemId()
        {
            if (ContaOrigemId == Guid.Empty)
                throw new DomainException("ID da conta de origem é inválido.");
        }

        private void Validate_ContaDestinoId()
        {
            if (ContaDestinoId == Guid.Empty)
                throw new DomainException("ID da conta de destino é inválido.");
        }

        private void Validate_Valor()
        {
            if (Valor <= 0)
                throw new DomainException("O valor da transferência deve ser maior que zero.");
        }

        private void Validate_ContasDiferentes()
        {
            if (ContaOrigemId == ContaDestinoId)
                throw new DomainException("Conta de origem e destino não podem ser iguais.");
        }

        private void Validate_Taxa()
        {
            if (Taxa < 0)
                throw new DomainException("A taxa da transferência não pode ser negativa.");
        }

        private void Validate_ValorLiquido()
        {
            if (ValorLiquido <= 0)
                throw new DomainException("O valor líquido da transferência deve ser maior que zero.");
        }

        private void Validate_Protocolo()
        {
            if (string.IsNullOrWhiteSpace(Protocolo))
                throw new DomainException("Protocolo da transferência é obrigatório.");
        }

        // Chama todas de uma vez
        public void Validate_Transferencia()
        {
            Validate_ID();
            Validate_ContaOrigemId();
            Validate_ContaDestinoId();
            Validate_ContasDiferentes();
            Validate_Valor();
            Validate_Taxa();
            Validate_ValorLiquido();
            Validate_Protocolo();
        }

    }
}
