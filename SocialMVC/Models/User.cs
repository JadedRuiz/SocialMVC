using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialMVC.Models
{
    public class User
    {
        public int id_usuario { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public int sexo { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime fecha_nacimiento { get; set; }
        [Display(Name = "Edad")]
        public int edad { get {
                DateTime now = DateTime.Today;
                int edad = now.Year - fecha_nacimiento.Year;
                if(fecha_nacimiento.Month > now.Month)
                {
                    --edad;
                }
                return edad;
            } }
        public string email { get; set; }
        public string contraseña { get; set; }
        [Phone]
        public string telefono { get; set; }
        public string path_perfil { get; set; }
        public string path_fondo { get; set; }
        public string descripcion { get; set; }



    }
}