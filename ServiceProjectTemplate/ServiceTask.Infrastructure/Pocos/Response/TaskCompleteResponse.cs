using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Pocos.Response
{
    public class TaskCompleteResponse : IResponse
    {
        /// <summary>
        /// The object of focus
        /// </summary>
        public object Entity { get; set; }
        public string Task { get; set; }
    }
}
