using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.Aplicacao.Modulos.ModuloGarcom;
using ControleDeBar.Aplicacao.Modulos.ModuloMesa;
using ControleDeBar.Aplicacao.Modulos.ModuloProduto;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.WebApp.Compartilhado.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControleDeBar.WebApp.Modulos.ModuloConta;

public class ContaController(
    ServicoConta servicoConta,
    ServicoMesa servicoMesa,
    ServicoGarcom servicoGarcom,
    ServicoProduto servicoProduto,
    IMapper mapeador
) : Controller
{
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarContasDto> dtos = servicoConta.SelecionarTodos();
        List<ListarContasViewModel> listarVms = mapeador.Map<List<ListarContasViewModel>>(dtos);

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarContaViewModel cadastrarVm = new CadastrarContaViewModel(
            string.Empty,
            Guid.Empty,
            Guid.Empty
        )
        {
            Mesas = ObterMesasSelectList(),
            Garcons = ObterGarconsSelectList()
        };

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarContaViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(PreencherListas(cadastrarVm));

        CadastrarContaDto dto = mapeador.Map<CadastrarContaDto>(cadastrarVm);

        Result resultado = servicoConta.Cadastrar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);
            return View(PreencherListas(cadastrarVm));
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(Guid id)
    {
        Result<DetalhesContaDto> resultado = servicoConta.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);
            return RedirectToAction(nameof(Listar));
        }

        if (resultado.Value.Situacao == SituacaoConta.Fechada)
        {
            TempData["MensagemErro"] = "Não é possível editar uma conta fechada.";
            return RedirectToAction(nameof(Listar));
        }

        EditarContaViewModel editarVm = mapeador.Map<EditarContaViewModel>(resultado.Value);

        return View(PreencherListas(editarVm));
    }

    [HttpPost]
    public ActionResult Editar(EditarContaViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(PreencherListas(editarVm));

        EditarContaDto dto = mapeador.Map<EditarContaDto>(editarVm);

        Result resultado = servicoConta.Editar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);
            return View(PreencherListas(editarVm));
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(Guid id)
    {
        Result<DetalhesContaDto> resultado = servicoConta.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);
            return RedirectToAction(nameof(Listar));
        }

        ExcluirContaViewModel excluirVm = mapeador.Map<ExcluirContaViewModel>(resultado.Value);

        return View(excluirVm);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirContaViewModel excluirVm)
    {
        Result resultado = servicoConta.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Gerenciar(Guid id)
    {
        Result<DetalhesContaDto> resultado = servicoConta.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);
            return RedirectToAction(nameof(Listar));
        }

        GerenciarContaViewModel gerenciarVm = mapeador.Map<GerenciarContaViewModel>(resultado.Value);

        return View(gerenciarVm);
    }

    [HttpPost]
    public ActionResult Fechar(Guid id)
    {
        Result resultado = servicoConta.Fechar(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);
            return RedirectToAction(nameof(Gerenciar), new { id });
        }

        TempData["MensagemSucesso"] = "Conta fechada com sucesso.";

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult AdicionarPedidos(Guid id)
    {
        Result<DetalhesContaDto> resultado = servicoConta.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);
            return RedirectToAction(nameof(Listar));
        }

        AdicionarPedidosContaViewModel pedidosVm = CriarAdicionarPedidosViewModel(resultado.Value);

        return View(pedidosVm);
    }

    [HttpPost]
    public ActionResult AdicionarPedido(AdicionarPedidosContaViewModel pedidosVm)
    {
        Result<DetalhesContaDto> resultadoConsulta = servicoConta.SelecionarPorId(pedidosVm.ContaId);

        if (resultadoConsulta.IsFailed)
        {
            TempData.AddErrorMessage(resultadoConsulta);
            return RedirectToAction(nameof(Listar));
        }

        AdicionarPedidosContaViewModel viewModelAtualizada = CriarAdicionarPedidosViewModel(
            resultadoConsulta.Value,
            pedidosVm.ProdutoId,
            pedidosVm.Quantidade
        );

        if (!ModelState.IsValid)
            return View(nameof(AdicionarPedidos), viewModelAtualizada);

        AdicionarPedidoContaDto dto = new AdicionarPedidoContaDto(
            pedidosVm.ContaId,
            pedidosVm.ProdutoId,
            pedidosVm.Quantidade
        );

        Result resultado = servicoConta.AdicionarPedido(dto);

        if (resultado.IsFailed)
        {
            if (resultadoConsulta.Value.Situacao == SituacaoConta.Fechada)
            {
                TempData.AddErrorMessage(resultado);
                return RedirectToAction(nameof(Gerenciar), new { id = pedidosVm.ContaId });
            }

            ModelState.AddModelError(resultado);
            return View(nameof(AdicionarPedidos), viewModelAtualizada);
        }

        return RedirectToAction(nameof(AdicionarPedidos), new { id = pedidosVm.ContaId });
    }

    [HttpPost]
    public ActionResult AlterarQuantidadeItemPedido(Guid contaId, Guid itemPedidoId, int quantidade)
    {
        Result resultado = servicoConta.AlterarQuantidadeItemPedido(
            new AlterarQuantidadeItemPedidoDto(contaId, itemPedidoId, quantidade)
        );

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);
            return RedirecionarAposFalhaEmItem(contaId);
        }

        return RedirectToAction(nameof(AdicionarPedidos), new { id = contaId });
    }

    [HttpPost]
    public ActionResult RemoverItemPedido(Guid contaId, Guid itemPedidoId)
    {
        Result resultado = servicoConta.RemoverItemPedido(
            new RemoverItemPedidoDto(contaId, itemPedidoId)
        );

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);
            return RedirecionarAposFalhaEmItem(contaId);
        }

        return RedirectToAction(nameof(AdicionarPedidos), new { id = contaId });
    }

    private ActionResult RedirecionarAposFalhaEmItem(Guid contaId)
    {
        Result<DetalhesContaDto> resultadoConsulta = servicoConta.SelecionarPorId(contaId);

        if (resultadoConsulta.IsFailed)
            return RedirectToAction(nameof(Listar));

        if (resultadoConsulta.Value.Situacao == SituacaoConta.Fechada)
            return RedirectToAction(nameof(Gerenciar), new { id = contaId });

        return RedirectToAction(nameof(AdicionarPedidos), new { id = contaId });
    }

    private AdicionarPedidosContaViewModel CriarAdicionarPedidosViewModel(
        DetalhesContaDto detalhes,
        Guid? produtoId = null,
        int? quantidade = null
    )
    {
        return new AdicionarPedidosContaViewModel
        {
            ContaId = detalhes.Id,
            NomeCliente = detalhes.NomeCliente,
            IdentificacaoMesa = detalhes.IdentificacaoMesa,
            NomeGarcom = detalhes.NomeGarcom,
            Situacao = detalhes.Situacao,
            ValorTotal = detalhes.ValorTotal,
            ProdutoId = produtoId ?? Guid.Empty,
            Quantidade = quantidade ?? 1,
            Pedidos = mapeador.Map<List<PedidoContaViewModel>>(detalhes.Pedidos),
            Produtos = ObterProdutosSelectList()
        };
    }

    private CadastrarContaViewModel PreencherListas(CadastrarContaViewModel cadastrarVm)
    {
        return cadastrarVm with
        {
            Mesas = ObterMesasSelectList(cadastrarVm.MesaId),
            Garcons = ObterGarconsSelectList(cadastrarVm.GarcomId)
        };
    }

    private EditarContaViewModel PreencherListas(EditarContaViewModel editarVm)
    {
        return editarVm with
        {
            Mesas = ObterMesasSelectList(editarVm.MesaId),
            Garcons = ObterGarconsSelectList(editarVm.GarcomId)
        };
    }

    private List<SelectListItem> ObterMesasSelectList(Guid? mesaSelecionada = null)
    {
        return servicoMesa
            .SelecionarTodos()
            .Select(m => new SelectListItem(
                m.Identificacao,
                m.Id.ToString(),
                mesaSelecionada.HasValue && m.Id == mesaSelecionada.Value
            ))
            .ToList();
    }

    private List<SelectListItem> ObterGarconsSelectList(Guid? garcomSelecionado = null)
    {
        return servicoGarcom
            .SelecionarTodos()
            .Select(g => new SelectListItem(
                g.Nome,
                g.Id.ToString(),
                garcomSelecionado.HasValue && g.Id == garcomSelecionado.Value
            ))
            .ToList();
    }

    private List<SelectListItem> ObterProdutosSelectList()
    {
        return servicoProduto
            .SelecionarTodos()
            .Select(p => new SelectListItem(
                $"{p.Nome} - R$ {p.Valor:F2}",
                p.Id.ToString()
            ))
            .ToList();
    }
}
