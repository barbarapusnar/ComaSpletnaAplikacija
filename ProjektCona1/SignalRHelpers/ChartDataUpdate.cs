using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using ProjektCona1.Models;
using System.Web.Hosting;
using System.Data.Entity;

namespace ProjektCona1
{
    public class ChartDataUpdate
    {
        // Singleton instance     
        private readonly static Lazy<ChartDataUpdate> _instance = new Lazy<ChartDataUpdate>(() => new ChartDataUpdate());
        // Send Data every 5 seconds     
        readonly int _updateInterval = 120000;
        //Timer Class     
        private Timer _timer;
        private volatile bool _sendingChartData = false;
        private readonly object _chartUpateLock = new object();
        LineChart lineChart = new LineChart();
        LineChart1 lineChart1 = new LineChart1();

        private ChartDataUpdate()
        {

        }

        public static ChartDataUpdate Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        // Calling this method starts the Timer     
        public void GetChartData()
        {
            _timer = new Timer(ChartTimerCallBack, null, _updateInterval, _updateInterval);

        }
        private void ChartTimerCallBack(object state)
        {
            if (_sendingChartData)
            {
                return;
            }
            lock (_chartUpateLock)
            {
                if (!_sendingChartData)
                {
                    _sendingChartData = true;
                    SendChartData();
                    _sendingChartData = false;
                }
            }
        }

        private void SendChartData()
        {
            lineChart.SetLineChartData();
            lineChart1.SetLineChartData1();
            GetAllClients().All.UpdateChart(lineChart,lineChart1);

        }

        private static dynamic GetAllClients()
        {
            return GlobalHost.ConnectionManager.GetHubContext<ChartHub>().Clients;
        }

       
    }
    
    public class LineChart
    {
        [JsonProperty("lineChartData")]
        private decimal[,] lineChartData;
        [JsonProperty("labelsData")]
        private string[] labelsData;
        //[JsonProperty("colorString")]
        //private string colorString;

        public void SetLineChartData()
        {
            ConaPrevzemEntities cp = new ConaPrevzemEntities();
            lineChartData = new decimal[6, 20];
            var p3 = (from a in cp.Podatki
                      where a.IdPostaje >= 1 && a.IdPostaje<=6
                      orderby a.Cas descending
                      select a.Cas).FirstOrDefault();
            DateTime zadnji = (DateTime)p3;
            DateTime prvi = zadnji.AddMinutes(-38);
            //var p2 = p3.OrderBy(a => a.Value).ToList();
            var pomoč = new DateTime[20];    
            labelsData = new string[20];
            for (int k = 0; k < 20; k++)
            {
                labelsData[k] = ((DateTime)prvi).TimeOfDay.ToString().Substring(0,5);
                pomoč[k] = prvi;
                prvi = prvi.AddMinutes(2);
            }
            for (int k = 0; k < 6; k++)
            {
                var p1 = (from a in cp.Podatki
                          where a.IdPostaje==k+1 
                          orderby a.Cas descending
                          select a).Take(20);
                p1 = p1.OrderBy(a => a.Cas.Value);
              
                int števec = 0;
               
                foreach (var c in p1)
                {
                    double t = (c.Cas.Value.TimeOfDay - pomoč[števec].TimeOfDay).TotalMinutes;
                    if (Math.Abs(t) <= 2)
                        lineChartData[k, števec] = (decimal)c.Padavine;
                    else
                        lineChartData[k, števec] = 0;
                    števec++;
                    if (števec >= 20) break;
                }

            }                 
        }
        
    }
    public class LineChart1
    {
        [JsonProperty("lineChartData")]
        private decimal[,] lineChartData;
        [JsonProperty("labelsData")]
        private string[] labelsData;
        //[JsonProperty("colorString")]
        //private string colorString;

        
        public void SetLineChartData1()
        {
            ConaPrevzemEntities cp = new ConaPrevzemEntities();
            lineChartData = new decimal[6, 20];
            var p3 = (from a in cp.Podatki
                      where a.IdPostaje >= 7 && a.IdPostaje <= 12
                      orderby a.Cas descending
                      select a.Cas).FirstOrDefault();
            DateTime zadnji = (DateTime)p3;
            DateTime prvi = zadnji.AddMinutes(-38);
            //var p2 = p3.OrderBy(a => a.Value).ToList();
            var pomoč = new DateTime[20];
            labelsData = new string[20];
            for (int k = 0; k < 20; k++)
            {
                labelsData[k] = ((DateTime)prvi).TimeOfDay.ToString().Substring(0, 5);
                pomoč[k] = prvi;
                prvi = prvi.AddMinutes(2);
            }
           
            for (int k = 0; k < 6; k++)
            {
                var p1 = (from a in cp.Podatki
                          where a.IdPostaje == k + 7
                          orderby a.Cas descending
                          select a).Take(20);
                p1 = p1.OrderBy(a => a.Cas.Value);
                int števec = 0;

                foreach (var c in p1)
                {
                    double t = (c.Cas.Value.TimeOfDay - pomoč[števec].TimeOfDay).TotalMinutes;
                    if (Math.Abs(t) <= 2)
                        lineChartData[k, števec] = (decimal)c.Padavine;
                    else
                        lineChartData[k, števec] = 0;
                    števec++;
                    if (števec >= 20) break;
                }

            }
        }
    }
}