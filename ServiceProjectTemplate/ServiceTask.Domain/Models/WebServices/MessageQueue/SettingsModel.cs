namespace $ext_projectname$.Domain.Models.WebServices.MessageQueue
{
    public class SettingsModel
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int? Port { get; set; }
        public bool? UseSsl { get; set; }
        public string SslVersion { get; set; }
        public bool AllowQueueManagement { get; set; }

        /// <summary>
        /// Quorum/Classic
        /// </summary>
        public string QueueType { get; set; } = "Classic";


    }
}
