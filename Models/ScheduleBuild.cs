using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace MVC_project.Models
{
    public class ScheduleBuild
    {

        public static int HallNumber { get; set; }
        public static string Movie { get; set; }
        public static System.DateTime Date { get; set; }
        public static List<Time> freeTimes { get; set; }
        public static int Id { get; set; }
    }
}