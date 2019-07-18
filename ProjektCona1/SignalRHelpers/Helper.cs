using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjektCona1.Models;

namespace ProjektCona1
{
    public class Helper
    {
        public static List<SelectListItem> getYearList()
        {
            var item = new List<SelectListItem> { };
            for (int i = DateTime.Today.Year; i >= 2019; i--)
            {
                item.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            return item;
        }
        public static List<SelectListItem> getPostajaList()
        {
            var item = new List<SelectListItem> { };
            ConaPrevzemEntities ce = new ConaPrevzemEntities();

            for (int i = 1; i <=12; i++)
            {
                var p = (from a in ce.Postaje
                         where a.IdPostaje == i
                         select a.OpisPostaje).FirstOrDefault();
                if (p!=null)
                item.Add(new SelectListItem() { Text = p.ToString(), Value = i.ToString() });
                else
                    item.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            return item;
        }
        public static List<SelectListItem> getMonthList(string year)
        {
            var item = new List<SelectListItem> { };
            item.Add(new SelectListItem() { Text = "Izberite mesec", Value = "Izberite mesec" });
            if (year != "")
            {
                if (year != DateTime.Today.Year.ToString())
                {
                    for (var i = 12; i > 0; i--)
                    {
                        item.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                }
                else
                {
                    for (var i = DateTime.Today.Month; i > 0; i--)
                    {
                        item.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                }
            }

            return item;
        }

        public static List<SelectListItem> getDayList(string year, string month)
        {
            var item = new List<SelectListItem> { };
            item.Add(new SelectListItem() { Text = "Izberite dan", Value = "Izberite dan" });

            if (year != "" && month != "")
            {
                if (!((year == DateTime.Today.Year.ToString()) && (month == DateTime.Today.Month.ToString())))
                {
                    if (month != "Izberite mesec")
                    {
                        for (var i = DateTime.DaysInMonth(Int16.Parse(year), Int16.Parse(month)); i > 0; i--)
                        {
                            item.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                        }
                    }
                }
                else
                {
                    for (var i = DateTime.Today.Day; i > 0; i--)
                    {
                        item.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                }
            }

            return item;
        }
    }
}