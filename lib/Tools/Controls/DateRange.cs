using System;

namespace LIB.Tools.Controls
{
    public class DateRange
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public override string ToString() => this.From.ToString("dd/MM/yyyy");
    }
}