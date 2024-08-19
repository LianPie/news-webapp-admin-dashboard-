using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace newsWebapp.Models
{
    public class NewsViewModel
    {
        public int ID { get; set; }
        [Required]
        public string title { get; set; }

        public string link { get; set; }

        public string image { get; set; }

        public string summary { get; set; }

        [AllowHtml] // Allow HTML content from Summernote
        public string Content { get; set; }
        public string cat { get; set; }
        public string tag { get; set; }

    }
}
