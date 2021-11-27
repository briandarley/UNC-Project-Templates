using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Pocos.Response
{
    public class FailedQueueResponse:IErrorResponse
    {
        public string Message { get; set; }
        
    }
}
