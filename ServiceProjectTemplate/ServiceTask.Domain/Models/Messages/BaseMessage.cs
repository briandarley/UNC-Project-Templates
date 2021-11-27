namespace $ext_projectname$.Domain.Models.Messages
{
    public abstract class BaseMessage : IMessage
    {
        public string Task { get; set; }
        public string Uid { get; set; }
        public int? ProvisionJobId { get; set; }
        public int RetryCount { get; set; }
        public string Overload { get; set; }
    }
}
