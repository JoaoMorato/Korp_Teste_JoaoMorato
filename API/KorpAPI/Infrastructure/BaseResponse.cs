using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure;
public class BaseResponse
{
    public string? Message { get; set; }
}

public class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }
    public PaginationModel? Pagination { get; set; }
}
