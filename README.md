Projeto de Engenharia Reversa e Refatoração de Software
Aluna: Carol
Disciplina: Design e Melhoria de Software

Este repositório contém a atividade prática de engenharia reversa, redocumentação e reestruturação de um sistema legado de processamento de pedidos escrito em C#.

1. Parte 01: Reverse Engineering (Engenharia Reversa)
Nesta etapa, foi realizada a análise do código original para identificar as responsabilidades da classe e as regras de negócio implícitas.

Responsabilidade: Processar pedidos de venda, integrando cálculos financeiros e logísticos.

Regras Identificadas: Cálculo de juros por parcelamento, descontos progressivos por nível de cliente (VIP, Premium), taxas por categoria de produto (Alimento/Importado) e matriz de frete baseada em peso e destino.

Documentação: O código original foi comentado linha por linha para explicar o significado de cada parâmetro e regra.

2. Parte 02: Redocumentação
A redocumentação focou na melhoria da semântica do código e na modelagem visual do domínio.

Nomes Adequados: Variáveis genéricas e ambíguas foram substituídas por nomes significativos.

Diagrama de Classes: Foi elaborado um diagrama no Excalidraw para representar a nova estrutura do domínio, separando os dados das ações de cálculo e notificação.

3. Parte 03: Reestruturação
A reestruturação aplicou padrões de design para eliminar a "God Class" original.

SRP (Single Responsibility Principle): A classe única foi dividida em especialistas: CalculadoraFinanceira, ServicoLogistica e Notificador.

Melhoria Estrutural: Redução de ambiguidades e maior clareza lógica, facilitando a manutenção e a escalabilidade do sistema.
