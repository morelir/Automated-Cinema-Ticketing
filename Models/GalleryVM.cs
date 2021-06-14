using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_project.Models
{
    public class GalleryVM
    {
        public List<MOVIES> movies { get; set; }
        public List<SCHEDULE> schdules { get; set; }
        public MOVIES movie { get; set; }
        public SCHEDULE schedule { get; set; }

        public USER user;

        public TimeSpan FromTime { get; set; }
    }
}