# 🍻 Controle de Bar

* O **Controle de Bar** é um sistema de gerenciamento de estabelecimentos que reúne em um só lugar funcionalidades essenciais para organização de mesas, garçons, produtos, contas e pedidos.

* O sistema permite que cada dono de bar administre seu próprio estabelecimento, garantindo o isolamento e a segurança dos dados entre diferentes bares.

* Todas as operações realizadas pelo sistema consideram o bar ao qual o usuário está vinculado.

<p align="center">
<img src="./.docs/home.gif">
</p>

## Funcionalidades

### 🍻 1. Módulo de Bar / Multi-Tenant

### Requisitos Funcionais

* O sistema deve permitir que um dono de bar realize seu cadastro na plataforma
* O sistema deve permitir que o dono de bar acesse e administre seu estabelecimento
* O sistema deve permitir que cada bar mantenha seus próprios cadastros de mesas, garçons, produtos e contas
* O sistema deve permitir que o dono de bar visualize somente os dados pertencentes ao seu estabelecimento

### Regras de Negócio

>** Cada dono deve possuir seu próprio estabelecimento dentro da plataforma  
>** Cada cadastro realizado deve estar vinculado ao respectivo bar  
>** Os dados de mesas, garçons, produtos, contas e pedidos devem ser isolados por bar  
>** Um dono de bar não pode visualizar dados pertencentes a outro estabelecimento  
>** Um dono de bar não pode editar ou excluir dados pertencentes a outro estabelecimento  
>** Todas as operações realizadas pelo sistema devem considerar o bar ao qual o usuário está vinculado  
>** O isolamento entre estabelecimentos deve ser aplicado a todos os módulos do sistema

---

### 🪑 2. Módulo de Mesas

<!-- <p align="center">
<img src="./.docs/mesas.gif">
</p> -->

### Requisitos Funcionais

* O sistema deve permitir cadastrar novas mesas
* O sistema deve permitir editar mesas já cadastradas
* O sistema deve permitir excluir mesas já cadastradas
* O sistema deve permitir visualizar as mesas cadastradas

### Regras de Negócio

* Campos obrigatórios:

  * Número da mesa
  * Quantidade de lugares
  * Status da mesa (Livre ou Ocupada)

>** O número da mesa deve ser um identificador único dentro do bar  
>** Não pode haver duas mesas com o mesmo número no mesmo bar  
>** A quantidade de lugares deve ser maior que zero  
>** Uma mesa cadastrada deve iniciar com o status Livre  
>** A mesa deve ser considerada Ocupada quando estiver vinculada a uma conta em atendimento  
>** A mesa deve voltar a ser considerada Livre quando a conta vinculada a ela for fechada  
>** Não permitir excluir uma mesa que esteja vinculada a uma conta em aberto

---

### 👨‍🍳 3. Módulo de Garçons

<!-- <p align="center">
<img src="./.docs/garcons.gif">
</p> -->

### Requisitos Funcionais

* O sistema deve permitir cadastrar novos garçons
* O sistema deve permitir editar garçons já cadastrados
* O sistema deve permitir excluir garçons já cadastrados
* O sistema deve permitir visualizar os garçons cadastrados

### Regras de Negócio

* Campos obrigatórios:

  * Nome (3 e 100 caracteres)
  * Telefone (formatos válidos)
  * CPF (11 dígitos)

>** Não permitir excluir um garçom que esteja vinculado a uma conta em aberto

---

### 🍔 4. Módulo de Produtos

<!-- <p align="center">
<img src="./.docs/produtos.gif">
</p> -->

### Requisitos Funcionais

* O sistema deve permitir cadastrar novos produtos
* O sistema deve permitir editar produtos já cadastrados
* O sistema deve permitir excluir produtos já cadastrados
* O sistema deve permitir visualizar os produtos cadastrados

### Regras de Negócio

* Campos obrigatórios:

  * Nome
  * Preço de venda

>** O nome do produto deve ser obrigatório  
>** O preço de venda deve ser maior que zero  
>** Não permitir excluir um produto que esteja vinculado a algum pedido

---

### 🧾 5. Módulo de Contas

<!-- <p align="center">
<img src="./.docs/contas.gif">
</p> -->

### Requisitos Funcionais

* O sistema deve permitir abrir novas contas
* O sistema deve permitir editar informações de uma conta em aberto
* O sistema deve permitir visualizar as contas cadastradas
* O sistema deve permitir consultar os detalhes de uma conta
* O sistema deve permitir fechar uma conta
* O sistema deve permitir visualizar os pedidos vinculados a uma conta
* O sistema deve calcular automaticamente o valor total da conta

### Regras de Negócio

* Campos obrigatórios:

  * Nome do cliente
  * Mesa
  * Garçom
  * Data de abertura
  * Situação (Aberta ou Fechada)

>** Toda conta deve estar vinculada a uma mesa  
>** Toda conta deve possuir um garçom responsável pelo atendimento  
>** Uma nova conta deve iniciar com a situação Aberta  
>** A data de abertura deve ser registrada no momento da abertura da conta  
>** Ao abrir uma conta, a mesa vinculada deve passar para o status Ocupada  
>** Ao fechar uma conta, a mesa vinculada deve voltar para o status Livre  
>** Uma conta fechada não pode receber novos pedidos ou sofrer alterações que modifiquem seu atendimento  
>** O valor total da conta deve ser calculado automaticamente a partir dos pedidos vinculados a ela

---

### 🛒 5.1 Módulo de Pedidos

<!-- <p align="center">
<img src="./.docs/pedidos.gif">
</p> -->

### Requisitos Funcionais

* O sistema deve permitir adicionar pedidos a uma conta em aberto
* O sistema deve permitir visualizar os pedidos de uma conta
* O sistema deve permitir alterar a quantidade de um produto solicitado
* O sistema deve permitir remover produtos de uma conta em aberto
* O sistema deve calcular o valor de cada pedido com base no preço do produto e na quantidade solicitada
* O sistema deve atualizar automaticamente o valor total da conta conforme os pedidos forem adicionados, alterados ou removidos

### Regras de Negócio

* Campos obrigatórios:

  * Conta
  * Produto
  * Quantidade

>** Todo pedido deve estar vinculado a uma conta  
>** Todo pedido deve estar vinculado a um produto  
>** A quantidade solicitada deve ser maior que zero  
>** Só é permitido adicionar pedidos a contas que estejam Abertas  
>** Não é permitido adicionar, alterar ou remover pedidos de uma conta Fechada  
>** O valor do pedido deve ser calculado automaticamente utilizando o preço de venda do produto e sua quantidade  
>** O valor total da conta deve considerar todos os pedidos vinculados a ela

---

## 🔐 Isolamento de Dados

O sistema possui uma arquitetura **Multi-Tenant**, na qual cada dono de bar possui seu próprio estabelecimento e os dados são isolados entre os diferentes bares.

Cada cadastro deve estar vinculado ao respectivo estabelecimento, incluindo:

* Mesas
* Garçons
* Produtos
* Contas
* Pedidos

Um dono de bar não pode visualizar, editar ou excluir dados pertencentes a outro estabelecimento. O isolamento deve ser aplicado a todos os módulos do sistema.

---

## 📋 Resumo dos Módulos

| Módulo                | Principais funcionalidades                                        |
| --------------------- | ----------------------------------------------------------------- |
| 🍻 Bar / Multi-Tenant | Cadastro e administração do estabelecimento e isolamento de dados |
| 🪑 Mesas              | Cadastro, edição, exclusão e visualização de mesas                |
| 👨‍🍳 Garçons         | Cadastro, edição, exclusão e visualização de garçons              |
| 🍔 Produtos           | Cadastro, edição, exclusão e visualização de produtos             |
| 🧾 Contas             | Abertura, edição, consulta, fechamento e cálculo automático       |
| 🛒 Pedidos            | Adição, alteração, remoção e cálculo automático dos pedidos       |

---

## Como utilizar

1. Clone o repositório ou baixe o código fonte.
2. Abra o terminal ou prompt de comando e navegue até a pasta raiz.
3. Utilize o comando abaixo para restaurar as dependências do projeto:

    ```bash
   dotnet restore
   ```
4. Para executar o projeto compilando em tempo real

   ```bash
   dotnet run --project ControleDeBar.WebApp
   ```

## Requisitos

- .NET 10.0 SDK

## 👩‍💻 Colaboradores

1. Natália Bortoli Vieira - [@nataliavieirab](https://github.com/nataliavieirab)
2. Júlia Hartmann - [@JuliaaHartmann](https://github.com/JuliaaHartmann)
3. Revisado pela [Academia do Programador](https://academiadoprogramador.com.br)