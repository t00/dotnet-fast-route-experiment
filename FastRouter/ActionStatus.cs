namespace FastRouter
{
    public class ActionStatus
    {
        public string Text { get; }

        public int Length { get; }

        public int Index { get; set; }

        internal ActionStatus(string text)
        {
            Text = text;
            Length = text.Length;
        }
    }
}
