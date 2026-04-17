using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaLegadoPedidos
{
    public class PedidoProcessor
    {
        private List<string> _logs = new List<string>();

        // MÉTODO PRINCIPAL: Responsável por processar toda a lógica de um pedido.
        // POSSUI ALTA COMPLEXIDADE (God Method) e muitas responsabilidades misturadas.
        public string ProcessarPedido(
            int pedidoId,
            string nomeCliente,
            string emailCliente,
            string tipoCliente,
            List<ItemPedido> itens,
            string cupom,
            string formaPagamento,
            string enderecoEntrega,
            double pesoTotal,
            bool entregaExpressa,
            bool clienteBloqueado,
            bool enviarEmail,
            bool salvarLog,
            string pais,
            int parcelas)
        {
            string resultado = "";
            double subtotal = 0;
            double desconto = 0;
            double frete = 0;
            double juros = 0;
            double total = 0;
            bool temErro = false;

            // --- ETAPA DE VALIDAÇÃO DE DADOS ---
            if (pedidoId <= 0)
            {
                resultado += "Pedido inválido\n";
                temErro = true;
            }

            if (string.IsNullOrEmpty(nomeCliente))
            {
                resultado += "Nome do cliente não informado\n";
                temErro = true;
            }

            // REGRA DE SEGURANÇA: Bloqueia o processamento se o cliente tiver restrições.
            if (clienteBloqueado == true)
            {
                resultado += "Cliente bloqueado\n";
                temErro = true;
            }

            // --- CÁLCULO DE ITENS E TAXAS ESPECÍFICAS ---
            if (itens == null || itens.Count == 0)
            {
                resultado += "Pedido sem itens\n";
                temErro = true;
            }
            else
            {
                for (int i = 0; i < itens.Count; i++)
                {
                    // Validação de integridade do item
                    if (itens[i].Quantidade <= 0 || itens[i].PrecoUnitario < 0)
                    {
                        resultado += "Item com dados inválidos: " + itens[i].Nome + "\n";
                        temErro = true;
                    }

                    subtotal = subtotal + (itens[i].PrecoUnitario * itens[i].Quantidade);

                    // REGRA DE NEGÓCIO: Taxas adicionais por categoria de produto (Alimentos e Importados)
                    if (itens[i].Categoria == "ALIMENTO")
                    {
                        subtotal = subtotal + 2;
                    }

                    if (itens[i].Categoria == "IMPORTADO")
                    {
                        subtotal = subtotal + 5;
                    }
                }
            }

            if (temErro == false)
            {
                // --- REGRA DE FIDELIDADE: Descontos baseados no perfil do cliente ---
                if (tipoCliente == "VIP") { desconto = subtotal * 0.15; }
                else if (tipoCliente == "PREMIUM") { desconto = subtotal * 0.10; }
                else if (tipoCliente == "NORMAL") { desconto = subtotal * 0.02; }
                else if (tipoCliente == "NOVO") { desconto = 0; }
                else { desconto = 1; }

                // --- REGRAS DE CUPONS DE DESCONTO ---
                if (!string.IsNullOrEmpty(cupom))
                {
                    if (cupom == "DESC10") { desconto += (subtotal * 0.10); }
                    else if (cupom == "DESC20") { desconto += (subtotal * 0.20); }
                    else if (cupom == "FRETEGRATIS") { frete = 0; }
                    else if (cupom == "VIP50" && tipoCliente == "VIP") { desconto += 50; }
                    else { resultado += "Cupom inválido ou não aplicável\n"; }
                }

                // --- LOGÍSTICA E FRETE (Matriz de decisão: Peso x Destino) ---
                if (pais == "BR")
                {
                    if (pesoTotal <= 1) frete = 10;
                    else if (pesoTotal <= 5) frete = 25;
                    else if (pesoTotal <= 10) frete = 40;
                    else frete = 70;

                    if (entregaExpressa == true) frete += 30; // Taxa de urgência nacional
                }
                else
                {
                    // Regras para Frete Internacional
                    if (pesoTotal <= 1) frete = 50;
                    else if (pesoTotal <= 5) frete = 80;
                    else frete = 120;

                    if (entregaExpressa == true) frete += 70; // Taxa de urgência internacional
                }

                // --- REGRAS DE PAGAMENTO E JUROS ---
                if (formaPagamento == "CARTAO")
                {
                    if (parcelas > 1 && parcelas <= 6) juros = subtotal * 0.02;
                    else if (parcelas > 6) juros = subtotal * 0.05;
                }
                else if (formaPagamento == "PIX") { desconto += 10; } // Incentivo para pagamento à vista
                else if (formaPagamento == "BOLETO") { desconto += 5; }

                // CÁLCULO FINAL DA OPERAÇÃO
                total = subtotal - desconto + frete + juros;
                if (total < 0) total = 0;

                // --- ANÁLISE DE RISCO E ALERTAS ---
                if (subtotal > 1000) resultado += "Pedido de alto valor\n";
                if (subtotal > 5000 && tipoCliente == "NOVO") resultado += "Pedido suspeito para cliente novo\n";
                if (formaPagamento == "BOLETO" && subtotal > 3000) resultado += "Pedido acima do limite para boleto\n";

                // --- PERSISTÊNCIA E NOTIFICAÇÃO ---
                if (salvarLog == true)
                {
                    _logs.Add("Pedido: " + pedidoId + " | Total: " + total + " | Data: " + DateTime.Now.ToString());
                }

                if (enviarEmail == true && !string.IsNullOrEmpty(emailCliente))
                {
                    resultado += "Email enviado para " + emailCliente + "\
