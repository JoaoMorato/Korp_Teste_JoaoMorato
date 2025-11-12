using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using ProdutoAPI.Models;
using ProdutoAPI.Services;
using System.Xml.Linq;

namespace ProdutoAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutoController : ControllerBase
{
    private readonly ILogger<ProdutoController> log;
    private readonly IProdutoService produtoService;

    public ProdutoController(ILogger<ProdutoController> log, IProdutoService produtoService)
    {
        this.log = log;
        this.produtoService = produtoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string name = "", [FromQuery] PaginationModel? pagination = default)
    {
        BaseResponse<List<ProdutoModel>> br = new BaseResponse<List<ProdutoModel>>();
        try
        { 
            produtoService.Pagination = pagination ?? new PaginationModel();
            br.Data = await produtoService.GetProducts(name);
            br.Pagination = produtoService.Pagination;
            return Ok(br);
        }
        catch(BaseException ex)
        {
            br.Message = ex.Message;
        }
        catch(Exception ex)
        {
            log.LogError(ex, "Erro ao obter produtos.");
            br.Message = "Ocorreu um erro inesperado durante o processamento.";
        }
        return StatusCode(422, br);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProdutoModel produto)
    {
        BaseResponse<ProdutoModel> br = new BaseResponse<ProdutoModel>();
        try
        {
            br.Data = await produtoService.AddProduct(produto);
            return Ok(br);
        }
        catch (BaseException ex)
        {
            br.Message = ex.Message;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Erro ao adicionar produto.");
            br.Message = "Ocorreu um erro inesperado durante o processamento.";
        }
        return StatusCode(422, br);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] ProdutoModel produto)
    {
        BaseResponse<ProdutoModel> br = new BaseResponse<ProdutoModel>();
        try
        {
            br.Data = await produtoService.UpdateProduct(produto);
            return Ok(br);
        }
        catch (BaseException ex)
        {
            br.Message = ex.Message;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Erro ao atualizar produto.");
            br.Message = "Ocorreu um erro inesperado durante o processamento.";
        }
        return StatusCode(422, br);
    }

    [HttpPost("Check")]
    public async Task<IActionResult> CheckProducts([FromBody] List<ProdutoNotaModel> produtos)
    {
        BaseResponse br = new BaseResponse();
        try
        {
            await produtoService.CheckProdutos(produtos);
            return Ok(br);
        }
        catch (BaseException ex)
        {
            br.Message = ex.Message;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Erro ao verificar produtos.");
            br.Message = "Ocorreu um erro inesperado durante o processamento.";
        }
        return StatusCode(422, br);
    }
}
