using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions;
public static class ObjectExtension
{
    public static T? ToObject<T>(this object obj) where T : class, new()
    {
        if (obj == null)
            return null;
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        }));
    }
}
