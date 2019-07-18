using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjektCona1.Models
{
    public class Data
    {
        public string getValues { get; set; }
        public string getDates { get; set; }
        public string getFillColor { get; set; }
        ////
        //public float lastEntry { get; set; }
        //public DateTime readFrom { get; set; }

        //public float lastHourAverage { get; set; }
        //public float dailyConsumption { get; set; }
        //[StringLength(4)]
        public int IdPostaje { get; set; }
        public string selectPostaja { get; set; }
        public string selectYear { get; set; }
        public string selectMonth { get; set; }
        public string selectDay { get; set; }
        public List<AllList> dayList { get; set; }
        public List<AllList> hourList { get; set; }
        public List<AllListRounded> monthListRounded { get; set; }
        public List<AllListRounded> dayListRounded { get; set; }
        public List<AllListRounded> hourListRounded { get; set; }
        public SelectList selectPostajaList { get; set; }
        public SelectList selectYearList { get; set; }
        public SelectList selectMonthList { get; set; }
        public SelectList selectDayList { get; set; }
    }
    public class AllList
    {
        public int DatePart { get; set; }
        public float Sum { get; set; } //vsota padavin
       
    }
    public class AllListRounded
    {
        public int DatePart { get; set; }
        public string Sum { get; set; } //vsota padavin
        
    }
    public class GraphData
    {
        public int Date { get; set; }
        public float Value { get; set; }
    }
}