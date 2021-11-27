namespace $ext_projectname$.Domain.Models.Messages
{
    public class FailMessage:IFailMessage
    {
        public string Task { get; set; }
        public string Uid { get; set; }
        public int? ProvisionJobId { get; set; }
        public int RetryCount { get; set; }
        public string Reason { get; set; }

        public FailMessage()
        {
            
        }
        public FailMessage(IMessage message)
        {
            InitializeProperties(message);
        }
        public FailMessage(IMessage message, string reason)
        {
            InitializeProperties(message);
            Reason = reason;
        }
        private void InitializeProperties(IMessage message)
        {
            Task = message.Task;
            Uid = message.Uid;
            ProvisionJobId = message.ProvisionJobId;
            RetryCount = message.RetryCount;
        }
    }
}
