using $ext_projectname$.Infrastructure.Interfaces.Tasks;

namespace $ext_projectname$.Infrastructure.Pocos.ProcessHelperService
{
    public class TaskOrder
    {
        
        public int Order { get; set; }
        public string TaskName { get; set; }
        
        public IProcessTask ProcessTask { get; set; }
        

    }
}
