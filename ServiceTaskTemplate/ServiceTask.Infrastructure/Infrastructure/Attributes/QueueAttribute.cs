using System;

namespace $ext_projectname$.Infrastructure.Infrastructure.Attributes
{
    public class QueueAttribute:Attribute
    {
        public string QueueName { get; set; }

        public QueueAttribute(string queueName)
        {
            QueueName = queueName;
        }

    }
}
