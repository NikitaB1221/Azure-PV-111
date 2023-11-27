namespace Azure_PV_111.Models.Home.SpellCheck
{

    public class SpellCheckResponse
    {
        public string _type { get; set; } = null!;
        public List<FlaggedToken> flaggedTokens { get; set; } = null!;
    }
    public class FlaggedToken
    {
        public int offset { get; set; }
        public string token { get; set; } = null!;
        public string type { get; set; } = null!;
        public List<Suggestion> suggestions { get; set; } = null!;
    }

    public class Suggestion
    {
        public string suggestion { get; set; } = null!;
        public double score { get; set; }
    }
}
