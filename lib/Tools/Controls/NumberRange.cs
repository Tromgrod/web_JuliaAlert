namespace LIB.Tools.Controls
{
    public class NumbersRange
    {
        public NumbersRange()
        {
            From = 0;
            To = 0;
            PostFix = "";
        }
        public int From { get; set; }

        public int To { get; set; }

        public string PostFix { get; set; }

        public override string ToString() => this.From.ToString();
    }
}