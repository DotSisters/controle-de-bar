using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.WebApp.Compartilhado.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
namespace ControleDeBar.WebApp.Modulos.ModuloMesa;

public class MesaController(ServicoMesa servicoMesa, IMapper mapeador) : Controller
{
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarMesasDto> dtos = servicoMesa.SelecionarTodos();
        List<ListarMesasViewModel> listarVms = mapeador.Map<List<ListarMesasViewModel>>(dtos);

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarMesaViewModel cadastrarVm = new CadastrarMesaViewModel(
            string.Empty,
            0,
            StatusMesa.Livre
        );

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarMesaViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        CadastrarMesaDto dto = mapeador.Map<CadastrarMesaDto>(cadastrarVm);

        Result resultado = servicoMesa.Cadastrar(dto);


        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(cadastrarVm);
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(Guid id)
    {
        Result<DetalhesMesaDto> resultado = servicoMesa.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        EditarMesaViewModel editarVm = mapeador.Map<EditarMesaViewModel>(resultado.Value);

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarMesaViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        EditarMesaDto dto = mapeador.Map<EditarMesaDto>(editarVm);

        Result resultado = servicoMesa.Editar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(editarVm);
        }

        return RedirectToAction(nameof(Listar));
    }


    [HttpGet]
    public ActionResult Excluir(Guid id)
    {
        Result<DetalhesMesaDto> resultado = servicoMesa.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        ExcluirMesaViewModel excluirVm = mapeador.Map<ExcluirMesaViewModel>(resultado.Value);

        return View(excluirVm);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirMesaViewModel excluirVm)
    {
        Result resultado = servicoMesa.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(Listar));
    }
}