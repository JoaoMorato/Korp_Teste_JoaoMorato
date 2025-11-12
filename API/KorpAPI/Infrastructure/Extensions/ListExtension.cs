using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions;
public static class ListExtension
{
    public static List<T?> ToObject<T>(this IEnumerable<object> objs) where T : class, new()
    {
        return objs.Select(e => e.ToObject<T>()).ToList();
    }
}
