using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjektCona1.Models;
using X.PagedList;

namespace ProjektCona1.Controllers
{
    public class PodatkiController : Controller
    {
       // private pc1Context db = new pc1Context();
        private ConaPrevzemEntities db = new ConaPrevzemEntities();
        // GET: Podatki
        public ActionResult Index(int? page)
        {
            var dataTable = (from x in db.Podatki
                        orderby x.Id descending
                        select x).Take(12000);
            
            var dataGraph = (from x in dataTable
                        orderby x.Id descending
                        select x).Take(108);
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePageOfData = dataTable.ToPagedList(pageNumber, 60); // will only contain 25 products max because of the pageSize

            ViewBag.OnePageOfData = onePageOfData;

            var tpovp = from t in dataGraph
                        group t by new
                        {
                            t.Cas
                        } into g
                        select new
                        {
                            PovpTemp = g.Average(p => p.Temp),
                            g.Key.Cas
                        };

            var vlpovp = from t in dataGraph
                        group t by new
                        {
                            t.Cas
                        } into g
                        select new
                        {
                            PovpVlg = g.Average(p => p.Vlaga),
                            g.Key.Cas
                        };

            ViewData["TempAvg"] = tpovp;
            ViewData["VlagaAvg"] = vlpovp;

            return View(dataTable);
        }

        public ActionResult Postaja(int? id)
        {
            int stevilka = id ?? 1;
            // to prikazuje v tabeli
            //je smiselno??
            var data = (from x in db.Podatki
                        where x.IdPostaje == stevilka
                        orderby x.Cas descending
                        select x).Take(100);
            DateTime danes = DateTime.Now;
            var dataDan = (from x in db.Podatki
                           where x.IdPostaje == stevilka && DbFunctions.DiffDays(x.Cas, danes) <=1
                           group x by new { Datum= DbFunctions.TruncateTime(x.Cas), Cas= x.Cas.Value.Hour } into z
                           orderby z.Key descending //,z.Key.Cas
                           select new ZaTabele()
                           {
                               datum=z.Key.Datum,
                               Cas = z.Key.Cas,
                               tempMax = z.Max(y => y.Temp),
                               tempMin = z.Min(y => y.Temp),
                               padSUM = z.Sum(y => y.Padavine),
                               vlagaAVG = z.Average(y => y.Vlaga)
                           }).Take(25);
            dataDan = dataDan.OrderBy(a => new { a.datum, a.Cas });
            
            var dataTeden = (from x in db.Podatki
                             where x.IdPostaje == stevilka && DbFunctions.DiffDays(x.Cas, danes) < 7
                             orderby x.Cas descending
                             group x by DbFunctions.TruncateTime(x.Cas) into g
                             select new 
                             {
                                 datum = g.Key,
                                 Cas = 0,
                                 tempMax = g.Max(z => z.Temp),
                                 tempMin = g.Min(z => z.Temp),
                                 padSUM = g.Sum(z => z.Padavine),
                                 vlagaAVG = g.Average(z=>z.Vlaga)
                             }).Take(7);

            var dataMesec = (from x in db.Podatki
                             where x.IdPostaje == stevilka && DbFunctions.DiffDays(x.Cas, danes) < 30
                             orderby x.Id descending
                             group x by DbFunctions.TruncateTime(x.Cas) into g
                             select new 
                             {
                                 datum = g.Key,
                                 Cas = 0,
                                 tempMax = g.Max(z => z.Temp),
                                 tempMin = g.Min(z => z.Temp),
                                 padSUM = g.Sum(z=>z.Padavine),
                                 vlagaAVG = g.Average(z => z.Vlaga)
                             }).Take(30);

            ViewData["Dan"] = dataDan;
            ViewData["Teden"] = dataTeden;
            ViewData["Mesec"] = dataMesec;
            ViewData["id"] = stevilka;

            return View(dataDan);
        }
       

        public ActionResult PostajaLive (int? id)
        {
            int stevilka = id ?? 1;
            //Dodano 25.3.2019
            db.Configuration.ProxyCreationEnabled = false;
            var data = (from x in db.Podatki
                        where x.IdPostaje == stevilka
                        orderby x.Id descending
                        select x).Take(10);
           
            var model = from x in data
                        orderby x.Id
                        select x;

            ViewData["id"] = stevilka;

            return View(model);
        }

        public ActionResult _PostajaLive(int? id)
        {
            int stevilka = id ?? 1;

            var data = (from x in db.Podatki
                        where x.IdPostaje == stevilka
                        orderby x.Id descending
                        select x).Take(10);

            var model = from x in data
                        orderby x.Id
                        select x;

            return View("_podatki", model);
        }
        public ActionResult PodatkiZaTabele(int? id, string gumb)
        {
            int stevilka = id ?? 1;
            if (String.IsNullOrEmpty(gumb))
                gumb = "dan";
            IEnumerable<ZaTabele> data;

            // to prikazuje v tabeli
            //je smiselno??
            DateTime danes = DateTime.Now;
            if (gumb == "dan")
            {

                var dataDan = (from x in db.Podatki
                               where x.IdPostaje == stevilka && DbFunctions.DiffDays(x.Cas, danes) <= 1
                               group x by new { Datum = DbFunctions.TruncateTime(x.Cas), Cas = x.Cas.Value.Hour } into z
                               orderby z.Key descending //,z.Key.Cas
                               select new ZaTabele()
                               {
                                   datum = z.Key.Datum,
                                   Cas = z.Key.Cas,
                                   tempMax = z.Max(y => y.Temp),
                                   tempMin = z.Min(y => y.Temp),
                                   padSUM = z.Sum(y => y.Padavine),
                                   vlagaAVG = z.Average(y => y.Vlaga)
                               }).Take(25);
                dataDan = dataDan.OrderBy( a =>new{ a.datum, a.Cas });
                data = dataDan;
                //foreach (var x in dataDan)
                //{
                //    data = data.Concat(new[] { (ZaTabele)x });
                //}
                //ViewData["gumb"] = "dan";
            }
            else
            {
                if (gumb == "teden")
                {
                    var dataTeden = (from x in db.Podatki
                                     where x.IdPostaje == stevilka && DbFunctions.DiffDays(x.Cas, danes) < 7
                                     orderby x.Cas descending
                                     group x by DbFunctions.TruncateTime(x.Cas) into g
                                     select new ZaTabele()
                                     {
                                         datum = g.Key,
                                         Cas = 0,
                                         tempMax = g.Max(z => z.Temp),
                                         tempMin = g.Min(z => z.Temp),
                                         padSUM = g.Sum(z => z.Padavine),
                                         vlagaAVG = g.Average(z => z.Vlaga)
                                     }).Take(7);
                    dataTeden = dataTeden.OrderBy(a => a.datum);
                    data = dataTeden;
                    //foreach (var x in dataTeden)
                    //{
                    //    data = data.Concat(new[] { (ZaTabele)x });
                    //}
                    ViewData["gumb"] = "teden";
                }
                else
                {
                    var dataMesec = (from x in db.Podatki
                                     where x.IdPostaje == stevilka && DbFunctions.DiffDays(x.Cas, danes) < 30
                                     orderby x.Id descending
                                     group x by DbFunctions.TruncateTime(x.Cas) into g
                                     select new ZaTabele()
                                     {
                                         datum = g.Key,
                                         Cas = 0,
                                         tempMax = g.Max(z => z.Temp),
                                         tempMin = g.Min(z => z.Temp),
                                         padSUM = g.Sum(z => z.Padavine),
                                         vlagaAVG = g.Average(z => z.Vlaga)
                                     }).Take(30);

                    dataMesec = dataMesec.OrderBy(a => a.datum);
                    data = dataMesec;
                    //foreach (var x in dataMesec)
                    //{

                    //   data= data.Concat(new[] { (ZaTabele)x });
                    //}
                    ViewData["gumb"] = "mesec";
                }
            }
            ViewData["id"] = stevilka;
            return PartialView(data);
        }
           
        }
    }

