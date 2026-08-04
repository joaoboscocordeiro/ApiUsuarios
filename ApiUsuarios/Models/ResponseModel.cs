using Microsoft.AspNetCore.Http;

namespace ApiUsuarios.Models
{
    public class ResponseModel<T>
    {
        public T? Dados { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
    }
}
