namespace LIB.Tools.Controls
{
    public class DecimalNumberRange
    {
        public DecimalNumberRange()
        {
            From = 0;
            To = 0;
            PostFix = "";
        }
        public decimal From { get; set; }

        public decimal To { get; set; }

        public string PostFix { get; set; }

        public override string ToString() => this.From.ToString("F");
    }
}