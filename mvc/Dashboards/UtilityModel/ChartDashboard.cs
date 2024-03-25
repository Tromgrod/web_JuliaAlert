using System;
using System.Collections.Generic;
using JuliaAlert.Models.Objects;
using JuliaAlertweblib.Enums;

namespace JuliaAlert.Dashboards.UtilityModel
{
    public class ChartDataInfo
    {
        public ChartDataInfo(long id, string name, int count, string color)
        {
            Id = id;
            Name = name;
            Count = count;
            Color = color;
        }

        public long Id;
        public string Name;
        public int Count;
        public string Color;
    }

    public struct ChartDashboard
    {
        public static ChartDashboard OrderChart = new ChartDashboard(PieChartTypeLink.Order, OrderState.LoadOrserPerOrderState);
        public static ChartDashboard OrderLocalChart = new ChartDashboard(PieChartTypeLink.OrderLocal, OrderState.LoadLocalOrserPerOrderState);

        private ChartDashboard(PieChartTypeLink PieChartTypeLink, Func<List<ChartDataInfo>> Func)
        {
            this.PieChartTypeLink = PieChartTypeLink;
            this.Func = Func;
        }

        public readonly PieChartTypeLink PieChartTypeLink;

        private readonly Func<List<ChartDataInfo>> Func;

        public bool IsEmpty => Func == null;

        public List<ChartDataInfo> Data => Func?.Invoke();
    }
}