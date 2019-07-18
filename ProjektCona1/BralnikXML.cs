using ProjektCona1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace ProjektCona1
{
    public class BralnikXML
    {
        public static Vreme Branje()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
               new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));

            HttpResponseMessage response = client.GetAsync(new Uri("http://meteo.arso.gov.si/uploads/probase/www/observ/surface/text/sl/observation_NOVA-GOR_latest.xml")).Result;
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    Vreme podatek = new Vreme();
                    var xmlMeteo = response.Content.ReadAsStreamAsync().Result;
                    string xmlString = new StreamReader(xmlMeteo).ReadToEnd();
                    using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                switch (reader.Name)
                                {
                                    case "valid":
                                        podatek.Datum = reader.ReadElementContentAsString();
                                        break;
                                    case "t":
                                        podatek.Temperatura= reader.ReadElementContentAsString();
                                        break;
                                    case "rh":
                                        podatek.Vlaga = reader.ReadElementContentAsString();
                                        break;
                                    case "nn_icon":
                                        podatek.Oblacnost = reader.ReadElementContentAsString();
                                        break;
                                    case "wwsyn_icon":
                                        podatek.Pojavi = reader.ReadElementContentAsString();
                                        break;
                                    case "dd_icon":
                                        podatek.SmerV = reader.ReadElementContentAsString();
                                        break;
                                    case "ff_val_kmh":
                                        podatek.HitrostV= reader.ReadElementContentAsString();
                                        break;
                                    case "ff_icon":
                                        podatek.MocV = reader.ReadElementContentAsString();
                                        break;
                                }
                            }
                        }
                    }
                    return podatek;
                }
                catch { return null; }
            }
            else
            { return null; }
        }
        public static string Alarm()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
               new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));

            HttpResponseMessage response = client.GetAsync(new Uri("http://meteo.arso.gov.si/uploads/probase/www/warning/text/sl/warning_rain_SLOVENIA_SOUTH-WEST_latest_CAP.xml")).Result;
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string stopnja = "";
                    var xmlMeteo = response.Content.ReadAsStreamAsync().Result;
                    string xmlString = new StreamReader(xmlMeteo).ReadToEnd();
                    using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                switch (reader.Name)
                                {
                                    case "headline":
                                        stopnja = reader.ReadElementContentAsString();
                                        break;
                                }
                            }
                        }
                    }
                    return stopnja;
                }
                catch { return null; }
            }
            else
            { return null; }
        
        }
    }
}