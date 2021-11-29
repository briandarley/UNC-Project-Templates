using System.Collections.Generic;

namespace $ext_projectname$.Domain.Models.WebServices.MessageQueue
{
    public class MessageModel: QueueModel
    {
        public string Body { get; set; }
        public int? RetryCount { get; set; }
        public Dictionary<string, object> QueueSettings { get; set; }
    }
}
