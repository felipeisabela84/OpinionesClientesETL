using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Persisitence
{
    public class ServiceResult
    {
            public bool IsSuccess { get; private set; }
            public string Message { get; private set; } = string.Empty;
            public object? Data { get; private set; }

            public static ServiceResult Success(string message = "Operación exitosa.", object? data = null)
                => new() { IsSuccess = true, Message = message, Data = data };

            public static ServiceResult Failure(string message)
                => new() { IsSuccess = false, Message = message };
    }
}
