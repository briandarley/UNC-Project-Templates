using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using $ext_projectname$.Domain.Models.Messages;

namespace $ext_projectname$.Infrastructure.Interfaces.WorkService
{
    public interface IWorkService
    {
        Task DoWork(EntityMessage message);
    }
}
