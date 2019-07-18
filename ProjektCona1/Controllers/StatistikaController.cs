using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjektCona1.Models;
namespace ProjektCona1.Controllers
{
        public class StatistikaController : Controller
        {
           
            [HttpGet]
            [ActionName("Stat")]
            public ActionResult Stat(int? id)
            {
                int stevilka = id ?? 1;
                string cookie = "";
                if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("Cookie"))
                {
                    cookie = "Yeah - Cookie: " + this.ControllerContext.HttpContext.Request.Cookies["Cookie"].Value;
                }
                ViewData["Cookie"] = cookie;

                try
                {
                    Data model = new Data();
                    model.IdPostaje = stevilka;

                    string month = "";
                    string year = "" + DateTime.Today.Year;

                    System.Text.StringBuilder stringBuilderDates = new System.Text.StringBuilder();
                    System.Text.StringBuilder stringBuilderValues = new System.Text.StringBuilder();

                    List<AllList> list1 = new List<AllList>();
                    List<float> list2 = new List<float>();

                    model.monthListRounded = new List<AllListRounded>();
                    using (var db = new ConaPrevzemEntities())
                    {
                        var databaseMonths = db.Month.Where(a => a.year == DateTime.Today.Year).ToList();

                        for (int i = 1; i <= 12; i++)
                        {
                            var databaseMonth = databaseMonths.Where(a => a.month1 == i).FirstOrDefault();
                            if (databaseMonth == null)
                            {
                                var meritveZnotrajMeseca = db.Podatki
                                                                .Where(a => a.Cas.Value.Year == DateTime.Today.Year && a.Cas.Value.Month == i && a.IdPostaje == stevilka)
                                                                .ToList();

                                for (int j = 1; j <= DateTime.DaysInMonth(DateTime.Today.Year, i); j++)
                                {
                                    float dnevniPodatek = meritveZnotrajMeseca
                                                                .Where(y => y.Cas.Value.Day == j && y.Cas.Value.Month == i && y.IdPostaje == stevilka)
                                                                .Select(x => new
                                                                {
                                                                    Hour1 = x.Cas.Value.Hour,
                                                                    Suma = (float)x.Padavine
                                                                })
                                                                .GroupBy(y => y.Hour1)
                                                                .Select(z => new
                                                                {
                                                                    Average = z.Sum(q => q.Suma),
                                                                    Hour2 = z.FirstOrDefault().Hour1
                                                                })
                                                                .ToList()
                                                                .Sum(x => x.Average);
                                    list2.Add(dnevniPodatek);
                                }

                                var sum = list2.Sum();

                                if (i < DateTime.Today.Month && DateTime.Today.Year <= DateTime.Today.Year)
                                {
                                    Month monthInsert = new Month()
                                    {
                                        year = DateTime.Today.Year,
                                        month1 = i,
                                        mesečnePadavine = (decimal)list2.Sum(),
                                        IdPostaje = stevilka
                                    };
                                    db.Month.Add(monthInsert);
                                    db.SaveChanges();
                                }

                                list2.Clear();

                                stringBuilderDates.Append(i + "x ");
                                stringBuilderValues.Append(sum.ToString("0") + "x ");

                                model.monthListRounded.Add(new AllListRounded
                                {
                                    DatePart = i,
                                    Sum = sum.ToString("0.00"),

                                });
                            }
                            else
                            {
                                stringBuilderDates.Append(databaseMonth.month1 + "x ");
                                stringBuilderValues.Append(((decimal)databaseMonth.mesečnePadavine).ToString("0") + "x ");

                                model.monthListRounded.Add(new AllListRounded
                                {
                                    DatePart = (int)databaseMonth.month1,
                                    Sum = ((decimal)databaseMonth.mesečnePadavine).ToString("0.00")

                                });
                            }
                        }

                        string outputDates = stringBuilderDates.ToString().Replace(',', '.').Replace('x', ',');
                        string outputValues = stringBuilderValues.ToString().Replace(',', '.').Replace('x', ',');

                        if (!String.IsNullOrEmpty(outputDates))
                        {
                            model.getDates = outputDates.Remove(outputDates.Length - 2);
                        }
                        if (!String.IsNullOrEmpty(outputValues))
                        {
                            model.getValues = outputValues.Remove(outputValues.Length - 2);
                        }
                    }
                    List<SelectListItem> postajaList = Helper.getPostajaList();
                    model.selectPostajaList = new SelectList(postajaList, "Value", "Text", (object)"Izberite postajo");
                    List<SelectListItem> yearList = Helper.getYearList();
                    model.selectYearList = new SelectList(yearList, "Value", "Text");

                    List<SelectListItem> monthList = Helper.getMonthList(year);
                    model.selectMonthList = new SelectList(monthList, "Value", "Text", (object)"Izberite mesec");

                    List<SelectListItem> dayList = Helper.getDayList(year, month);
                    model.selectDayList = new SelectList(dayList, "Value", "Text", (object)"Izberite mesec");
                    return View(model);
                }
                catch (Exception)
                {
                    return RedirectToAction("Error");
                }
            }

            [HttpPost]
            [ActionName("Stat")]
            public ActionResult StatistikaPost(Data model)
            {
                ViewData["Message"] = "Welcome to ASP.NET MVC!";
                //int stevilka = model.IdPostaje;
                int stevilka = 1;
                if (model.selectPostaja != null)
                    stevilka = int.Parse(model.selectPostaja.ToString());
                string cookie = "";
                if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("Cookie"))
                {
                    cookie = "Yeah - Cookie: " + this.ControllerContext.HttpContext.Request.Cookies["Cookie"].Value;
                }
                ViewData["Cookie"] = cookie;

                try
                {
                    using (var db = new ConaPrevzemEntities())
                    {
                        string month = "";
                        if (model.selectMonth != null)
                        {
                            month = model.selectMonth.ToString();
                        }
                        string year = "";
                        if (model.selectYear != null)
                        {
                            year = model.selectYear.ToString();
                        }

                        #region Helper - kreiranje seznamov
                        List<SelectListItem> postajaList = Helper.getPostajaList();
                        model.selectPostajaList = new SelectList(postajaList, "Value", "Text", (object)"Izberite postajo");
                        List<SelectListItem> yearList = Helper.getYearList();
                        model.selectYearList = new SelectList(yearList, "Value", "Text", (object)"Izberite leto");

                        List<SelectListItem> monthList = Helper.getMonthList(year);
                        model.selectMonthList = new SelectList(monthList, "Value", "Text", (object)"Izberite mesec");

                        List<SelectListItem> dayList = Helper.getDayList(year, month);
                        model.selectDayList = new SelectList(dayList, "Value", "Text");
                        #endregion
                        #region Inicializacija string builderjev in izbranih dni, mesecev in let
                        System.Text.StringBuilder stringBuilderDates = new System.Text.StringBuilder();
                        System.Text.StringBuilder stringBuilderValues = new System.Text.StringBuilder();
                        System.Text.StringBuilder stringBuilderFillColor = new System.Text.StringBuilder();

                        int intSelectYear, intSelectMonth, intSelectDay;
                        DateTime chosenDate = new DateTime();

                        int.TryParse(model.selectDay, out intSelectDay);
                        int.TryParse(model.selectMonth, out intSelectMonth);
                        int.TryParse(model.selectYear, out intSelectYear);
                        #endregion

                        #region Izpis porabe v določenem dnevu
                        if (int.TryParse(model.selectDay, out intSelectDay))
                        {
                            var queryHour = db.Podatki
                                                .Where(a => a.Cas.Value.Year == intSelectYear)
                                                .Where(a => a.Cas.Value.Month == intSelectMonth)
                                                .Where(a => a.Cas.Value.Day == intSelectDay)
                                                .Where(a => a.IdPostaje == stevilka)
                                                .GroupBy(i => i.Cas.Value.Hour)
                                                .OrderBy(a => a.Key)
                                                .Select(g => new AllList
                                                {
                                                    DatePart = g.Key + 1,
                                                    Sum = (float)g.Sum(a => a.Padavine),
                                                }).ToList();

                            for (int i = 1; i <= 24; i++)
                            {
                                if (queryHour.Count() == i - 1)
                                {
                                    queryHour.Add(new AllList { DatePart = i, Sum = 0.00f });
                                }
                                else if (queryHour[i - 1].DatePart != i)
                                {
                                    queryHour.Insert(i - 1, new AllList { DatePart = i, Sum = 0.00f });
                                }
                            }

                            model.hourList = queryHour;
                            //hourListRounded
                            model.hourListRounded = new List<AllListRounded>();
                            foreach (var item in model.hourList)
                            {
                                model.hourListRounded.Add(new AllListRounded
                                {
                                    DatePart = item.DatePart,
                                    Sum = item.Sum.ToString("0.00"),

                                });
                            }
                            foreach (var item in queryHour)
                            {
                                stringBuilderDates.Append(item.DatePart + "x ");
                                stringBuilderValues.Append(item.Sum.ToString("0") + "x ");
                            }

                            string outputDates = stringBuilderDates.ToString().Replace(',', '.').Replace('x', ',');
                            string outputValues = stringBuilderValues.ToString().Replace(',', '.').Replace('x', ',');

                            if (!String.IsNullOrEmpty(outputDates))
                            {
                                model.getDates = outputDates.Remove(outputDates.Length - 2);
                            }
                            if (!String.IsNullOrEmpty(outputValues))
                            {
                                model.getValues = outputValues.Remove(outputValues.Length - 2);
                            }

                            stringBuilderDates.Clear();
                            stringBuilderValues.Clear();
                        }
                        #endregion
                        #region Izpis porabe v določenem mesecu

                        else if (int.TryParse(model.selectMonth, out intSelectMonth))
                        {
                            List<AllList> list1 = new List<AllList>();
                            var meritveZnotrajMeseca = db.Podatki
                                                            .Where(y => y.Cas.Value.Year == intSelectYear && y.Cas.Value.Month == intSelectMonth)
                                                            .ToList();

                            for (int i = 1; i <= DateTime.DaysInMonth(intSelectYear, intSelectMonth); i++)
                            {
                                float dnevniPodatek = meritveZnotrajMeseca
                                                            .Where(y => y.Cas.Value.Day == i && y.IdPostaje == stevilka)
                                                            .Select(x => new
                                                            {
                                                                Hour1 = x.Cas.Value.Hour,
                                                                Suma = (float)x.Padavine
                                                            })
                                                            .GroupBy(y => y.Hour1)
                                                            .Select(z => new
                                                            {
                                                                Average = z.Sum(q => q.Suma),
                                                                Hour2 = z.FirstOrDefault().Hour1
                                                            })
                                                            .ToList()
                                                            .Sum(x => x.Average);
                                list1.Add(new AllList
                                {
                                    DatePart = i,
                                    Sum = dnevniPodatek

                                });
                            }

                            //dayList
                            model.dayList = list1;
                            int k = 1;
                            foreach (var item in list1)
                            {
                                chosenDate = new DateTime(intSelectYear, intSelectMonth, k);
                                stringBuilderDates.Append(item.DatePart + "x ");
                                stringBuilderValues.Append(item.Sum.ToString("0") + "x ");

                                if (chosenDate.DayOfWeek == DayOfWeek.Saturday || chosenDate.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    stringBuilderFillColor.Append(item.DatePart - 1 + "x ");
                                }
                                k++;
                            }
                            //dayListRounded
                            model.dayListRounded = new List<AllListRounded>();
                            foreach (var item in model.dayList)
                            {
                                model.dayListRounded.Add(new AllListRounded
                                {
                                    DatePart = item.DatePart,
                                    Sum = item.Sum.ToString("0.0"),

                                });
                            }

                            string outputDates = stringBuilderDates.ToString().Replace(',', '.').Replace('x', ',');
                            string outputValues = stringBuilderValues.ToString().Replace(',', '.').Replace('x', ',');
                            string outputFillColor = stringBuilderFillColor.ToString().Replace(',', '.').Replace('x', ',');

                            if (!String.IsNullOrEmpty(outputDates))
                            {
                                model.getDates = outputDates.Remove(outputDates.Length - 2);
                            }
                            if (!String.IsNullOrEmpty(outputValues))
                            {
                                model.getValues = outputValues.Remove(outputValues.Length - 2);
                            }
                            stringBuilderDates.Clear();
                            stringBuilderValues.Clear();
                            stringBuilderFillColor.Clear();
                        }
                        #endregion
                        #region Izpis porabe v določenem letu
                        else if (int.TryParse(model.selectYear, out intSelectYear))
                        {
                            List<float> list2 = new List<float>();

                            model.monthListRounded = new List<AllListRounded>();
                            var databaseMonths = db.Month.Where(a => a.year == intSelectYear).ToList();

                            for (int i = 1; i <= 12; i++)
                            {
                                var databaseMonth = databaseMonths.Where(a => a.month1 == i && a.IdPostaje == stevilka).FirstOrDefault();
                                if (databaseMonth == null)
                                {
                                    var meritveZnotrajMeseca = db.Podatki
                                                                    .Where(a => a.Cas.Value.Year == intSelectYear && a.Cas.Value.Month == i && a.IdPostaje == stevilka)
                                                                    .ToList();

                                    for (int j = 1; j <= DateTime.DaysInMonth(intSelectYear, i); j++)
                                    {
                                        float dnevniPodatek = meritveZnotrajMeseca
                                                                    .Where(y => y.Cas.Value.Day == j && y.Cas.Value.Month == i && y.IdPostaje == stevilka)
                                                                    .Select(x => new
                                                                    {
                                                                        Hour1 = x.Cas.Value.Hour,
                                                                        Suma = (float)x.Padavine
                                                                    })
                                                                    .GroupBy(y => y.Hour1)
                                                                    .Select(z => new
                                                                    {
                                                                        Average = z.Sum(q => q.Suma),
                                                                        Hour2 = z.FirstOrDefault().Hour1
                                                                    })
                                                                    .ToList()
                                                                    .Sum(x => x.Average);
                                        list2.Add(dnevniPodatek);
                                    }

                                    var sum = list2.Sum();

                                    if ((intSelectYear < DateTime.Today.Year) || (i < DateTime.Today.Month && intSelectYear == DateTime.Today.Year))
                                    {
                                        Month monthInsert = new Month()
                                        {
                                            year = intSelectYear,
                                            month1 = i,
                                            mesečnePadavine = (decimal)list2.Sum(),
                                            IdPostaje = stevilka
                                        };
                                        db.Month.Add(monthInsert);
                                        db.SaveChanges();
                                    }

                                    list2.Clear();

                                    stringBuilderDates.Append(i + "x ");
                                    stringBuilderValues.Append(sum.ToString("0") + "x ");

                                    model.monthListRounded.Add(new AllListRounded
                                    {
                                        DatePart = i,
                                        Sum = sum.ToString("0.00"),

                                    });
                                }
                                else
                                {
                                    stringBuilderDates.Append(databaseMonth.month1 + "x ");
                                    stringBuilderValues.Append(((decimal)databaseMonth.mesečnePadavine).ToString("0") + "x ");

                                    model.monthListRounded.Add(new AllListRounded
                                    {
                                        DatePart = (int)databaseMonth.month1,
                                        Sum = ((decimal)databaseMonth.mesečnePadavine).ToString("0.00"),

                                    });
                                }
                            }

                            string outputDates = stringBuilderDates.ToString().Replace(',', '.').Replace('x', ',');
                            string outputValues = stringBuilderValues.ToString().Replace(',', '.').Replace('x', ',');

                            if (!String.IsNullOrEmpty(outputDates))
                            {
                                model.getDates = outputDates.Remove(outputDates.Length - 2);
                            }
                            if (!String.IsNullOrEmpty(outputValues))
                            {
                                model.getValues = outputValues.Remove(outputValues.Length - 2);
                            }
                        }
                        #endregion
                    }
                    return View(model);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error");
                }
            }
        }
    }
