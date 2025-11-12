using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using NotaFiscalAPI.Models;
using NotaFiscalAPI.Services;

namespace NotaFiscalAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NotaFiscalController : ControllerBase
{
    private readonly ILogger<NotaFiscalController> log;
    private readonly INotaFiscalService notaFiscalService;

    public NotaFiscalController(ILogger<NotaFiscalController> log, INotaFiscalService notaFiscalService)
    {
        this.log = log;
        this.notaFiscalService = notaFiscalService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotas([FromQuery] int id = 0, [FromQuery] PaginationModel? pagination = default)
    {
        BaseResponse<List<NotaFiscalModel>> br = new BaseResponse<List<NotaFiscalModel>>();
        try
        {
            notaFiscalService.Pagination = pagination ?? new PaginationModel();
            br.Data = await notaFiscalService.GetNotas(id);
            br.Pagination = notaFiscalService.Pagination;
            return Ok(br);
        }
        catch (BaseException ex)
        {
            br.Message = ex.Message;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Erro ao obter notas fiscais.");
            br.Message = "Ocorreu um erro inesperado durante o processamento.";
        }
        return StatusCode(422, br);
    }

    [HttpPost]
    public async Task<IActionResult> AddNotaFiscal([FromBody] NotaFiscalModel notaFiscal)
    {
        BaseResponse<NotaFiscalModel> br = new BaseResponse<NotaFiscalModel>();
        try
        {
            br.Data = await notaFiscalService.AddNotaFiscal(notaFiscal);
            return Ok(br);
        }
        catch (BaseException ex)
        {
            br.Message = ex.Message;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Erro ao adicionar nota fiscal.");
            br.Message = "Ocorreu um erro inesperado durante o processamento.";
        }
        return StatusCode(422, br);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateNotaFiscal([FromBody] NotaFiscalModel notaFiscal)
    {
        BaseResponse<NotaFiscalModel> br = new BaseResponse<NotaFiscalModel>();
        try
        {
            br.Data = await notaFiscalService.UpdateNotaFiscal(notaFiscal);
            return Ok(br);
        }
        catch (BaseException ex)
        {
            br.Message = ex.Message;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Erro ao atualizar nota fiscal.");
            br.Message = "Ocorreu um erro inesperado durante o processamento.";
        }
        return StatusCode(422, br);
    }

    [HttpPost("Imprimir/{id}")]
    public async Task<IActionResult> InprimirNotaFiscal(int id)
    {
        BaseResponse br = new BaseResponse();
        try
        {
            await notaFiscalService.InprimirNotaFiscal(id);
            return Ok(br);
        }
        catch (BaseException ex)
        {
            br.Message = ex.Message;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Erro ao imprimir nota fiscal.");
            br.Message = "Ocorreu um erro inesperado durante o processamento.";
        }
        return StatusCode(422, br);
    }
}
