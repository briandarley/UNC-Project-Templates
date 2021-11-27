namespace $ext_projectname$.Domain.Models.WebServices.MessageQueue
{
    public class RetryMessageModel:MessageModel
    {
        /// <summary>
        /// TTL, In milliseconds
        /// </summary>
        public int TimeToLive { get; set; }

    }
}
