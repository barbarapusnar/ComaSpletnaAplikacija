using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ProjektCona1
{
    public class ChartHub:Hub
    {
        // Create the instance of ChartDataUpdate     
        private readonly ChartDataUpdate _ChartInstance;
        public ChartHub() : this(ChartDataUpdate.Instance) { }

        public ChartHub(ChartDataUpdate ChartInstance)
        {
            _ChartInstance = ChartInstance;
        }
       
        public void InitChartData()
        {
            //Show Chart initially when InitChartData called first time     
            LineChart lineChart = new LineChart();
            LineChart1 lineChart1 = new LineChart1();
            lineChart.SetLineChartData();
            lineChart1.SetLineChartData1();
            Clients.All.UpdateChart(lineChart,lineChart1);


            //Call GetChartData to send Chart data every 5 seconds     
            _ChartInstance.GetChartData();

        }

    }
}