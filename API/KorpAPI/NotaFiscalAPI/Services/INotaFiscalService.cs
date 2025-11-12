using Infrastructure.Models;
using NotaFiscalAPI.Models;

namespace NotaFiscalAPI.Services;

public interface INotaFiscalService
{
    public PaginationModel Pagination { get; set; }

    Task<List<NotaFiscalModel>> GetNotas(int id);
    Task InprimirNotaFiscal(int id);
    Task<NotaFiscalModel> UpdateNotaFiscal(NotaFiscalModel notaFiscal);
    Task<NotaFiscalModel> AddNotaFiscal(NotaFiscalModel notaFiscal);
}