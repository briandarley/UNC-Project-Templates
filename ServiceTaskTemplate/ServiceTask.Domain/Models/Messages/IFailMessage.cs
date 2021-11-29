namespace $ext_projectname$.Domain.Models.Messages
{
    public interface IFailMessage: IMessage
    {
        string Reason { get; set; }
    }
}
