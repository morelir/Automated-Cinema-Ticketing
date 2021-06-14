using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_project.Models
{
    public class PurchaseVM
    {
        public Orders order { get; set; }
        public Payment payment { get; set; }
    }
}