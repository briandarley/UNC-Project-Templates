using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Pocos.QueueService
{
    public class MessageReceivedResponse: IResponse
    {
        public bool AcknowledgeMessage { get; set; }
    }
}
