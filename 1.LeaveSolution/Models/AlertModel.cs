using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveSolution.Models
{
    public class AlertModel
    {
        public string AlertType { get; set; }
        public string AlertTitle { get; set; }
        public string AlertMessage { get; set; }

        public AlertModel(string type, string title, string message)
        {
            AlertType = type;
            AlertTitle = title;
            AlertMessage = message;
        }
    }
}
