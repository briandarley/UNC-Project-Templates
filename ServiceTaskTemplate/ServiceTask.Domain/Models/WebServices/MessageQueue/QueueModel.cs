using $ext_projectname$.Domain.Types;

namespace $ext_projectname$.Domain.Models.WebServices.MessageQueue
{
    public class QueueModel
    {
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public QueueTypes QueueType { get; set; } = QueueTypes.Classic;
    }
}
