namespace $ext_projectname$.Domain.Models.WebServices.Smtp
{
    public class MessageModel
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string AdminEmail { get; set; }
        public string AdminSubject { get; set; }
    }
}
