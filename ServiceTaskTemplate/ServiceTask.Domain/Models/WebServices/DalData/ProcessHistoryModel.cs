using System;

namespace $ext_projectname$.Domain.Models.WebServices.DalData
{
    public class ProcessHistoryModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? Failed { get; set; }

        public string Source { get; set; }

        public string MachineName { get; set; }

        public string Remarks { get; set; }

        public string Arguments { get; set; }

        public string ErrorMessage { get; set; }
    }
}
