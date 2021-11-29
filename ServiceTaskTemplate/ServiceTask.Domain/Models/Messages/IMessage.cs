namespace $ext_projectname$.Domain.Models.Messages
{
    public interface IMessage
    {
        string Task { get; set; }
        string Uid { get; set; }
        int? ProvisionJobId { get; set; }
        int RetryCount { get; set; }
    }
}