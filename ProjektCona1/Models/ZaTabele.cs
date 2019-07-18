using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjektCona1.Models
{
    public class ZaTabele
    {
        [DisplayFormat(DataFormatString = "{0:d}")]

        public DateTime? datum { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.#;-#.#;}")]

        public int Cas { get; set; }
        public decimal? tempMax { get; set; }
        public decimal? tempMin { get; set; }
        public decimal? padSUM { get; set; }
        public decimal? vlagaAVG { get; set; }
        
    }
}