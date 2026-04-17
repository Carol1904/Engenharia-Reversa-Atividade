using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaRefatorado
{
    // 1. ENTIDADE DE DADOS: Agrupa as informações que antes eram parâmetros soltos
    public class Pedido
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; }
        public string EmailCliente { get; set; }
        public List<ItemPedido> Itens { get; set; }
        public string Endereco { get; set; }
        public string FormaPagamento { get; set; }
        public double PesoTotal { get; set; }
        public string Pais { get; set; }
        public int Parcelas { get; set; }
        public bool EntregaExpressa { get; set; }
    }

    // 2. SERVIÇO ORQUESTRADOR: O "Cérebro" que coordena o processo
    public class PedidoService
    {
        private readonly CalculadoraFinanceira _calculadora = new CalculadoraFinanceira();
        private readonly ServicoLogistica _logistica = new ServicoLogistica();
        private readonly Notificador _notificador = new Notificador();

        public string Processar(Pedido pedido)
        {
            // Validação inicial
            if (pedido.Id <= 0) return "Erro: Pedido inválido.";

            // Cálculos utilizando classes especialistas
            double subtotal = _calculadora.CalcularSubtotal(pedido.Itens);
            double desconto = _calculadora.CalcularDescontos(pedido, subtotal);
            double frete = _logistica.CalcularFrete(pedido.PesoTotal, pedido.Pais, pedido.EntregaExpressa);
            double juros = _calculadora.CalcularJuros(subtotal, pedido.FormaPagamento, pedido.Parcelas);

            double total = subtotal - desconto + frete + juros;

            // Notificação
            _notificador.EnviarConfirmacao(pedido.EmailCliente, total);

            return $"Sucesso! Total: {total:C2}";
        }
    }

    // 3. CLASSE ESPECIALISTA: Cálculos Financeiros (Subtotal, Desconto, Juros)
    public class CalculadoraFinanceira
    {
        public double CalcularSubtotal(List<ItemPedido> itens)
        {
            return itens.Sum(i => (i.PrecoUnitario * i.Quantidade) + (i.Categoria == "ALIMENTO" ? 2 : 0));
        }

        public double CalcularDescontos(Pedido p, double subtotal)
        {
            // Lógica de cupons e fidelidade aqui
            return subtotal * 0.05; // Exemplo simplificado
        }

        public double CalcularJuros(double subtotal, string forma, int parcelas)
        {
            if (forma == "CARTAO" && parcelas > 6) return subtotal * 0.05;
            return 0;
        }
    }

    // 4. CLASSE ESPECIALISTA: Logística e Frete
    public class ServicoLogistica
    {
        public double CalcularFrete(double peso, string pais, bool expressa)
        {
            double baseFrete = (pais == "BR") ? 20 : 100;
            if (peso > 5) baseFrete += 50;
            return expressa ? baseFrete + 30 : baseFrete;
        }
    }

    // 5. CLASSE ESPECIALISTA: Notificações
    public class Notificador
    {
        public void EnviarConfirmacao(string email, double total)
        {
            Console.WriteLine($"Enviando e-mail para {email} com o total de {total:C2}");
        }
    }

    public class ItemPedido
    {
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public int Quantidade { get; set; }
        public double PrecoUnitario { get; set; }
    }
}
