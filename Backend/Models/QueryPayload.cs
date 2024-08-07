namespace GenAIApp.Models
{
    public class QueryPayload
    {
        public Input Inputs { get; set; }

        public class Input
        {
            public string Question { get; set; }
            public string Context { get; set; }
        }
    }
}
