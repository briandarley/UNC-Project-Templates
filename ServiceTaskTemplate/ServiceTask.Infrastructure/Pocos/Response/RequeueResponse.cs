using $ext_projectname$.Domain.Models.Messages;
using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Pocos.Response
{
    public class RequeueResponse:IErrorResponse
    {
        
        public BaseMessage Body { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string Message { get; set; }
    }
}
