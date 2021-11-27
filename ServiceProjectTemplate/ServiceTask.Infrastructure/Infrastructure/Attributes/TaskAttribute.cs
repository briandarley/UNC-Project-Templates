using System;

namespace $ext_projectname$.Infrastructure.Infrastructure.Attributes
{
    public class TaskAttribute:Attribute
    {
        public string Task { get; set; }

        public TaskAttribute(string task)
        {
            Task = task;
        }
    }
}
