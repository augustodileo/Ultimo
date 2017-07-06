using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ultimo.Models
{
    public class Materia
    {
        public int MateriaId { get; set; }
        public string MateriaName { get; set; }
        public int UniversidadId { get; set; }
        public int FacultadId { get; set; }

    }
}