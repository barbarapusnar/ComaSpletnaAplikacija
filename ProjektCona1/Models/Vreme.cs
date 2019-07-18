using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjektCona1.Models
{
    public class Vreme
    {
        public string Datum { get; set; }
        public string Temperatura { get; set; }
        public string Vlaga { get; set; }
        public string Oblacnost { get; set; }
        public string Pojavi { get; set; }
        public string SmerV { get; set; }
        public string HitrostV { get; set; }
        public string MocV { get; set; }
    }
}