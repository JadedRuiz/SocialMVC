using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialMVC.Models
{
    public class RespuestaModel
    {
        public Boolean ok { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }
}