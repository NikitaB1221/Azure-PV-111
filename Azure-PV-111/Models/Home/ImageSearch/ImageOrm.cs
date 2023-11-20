namespace Azure_PV_111.Models.Home.ImageSearch
{
    public class ImageSearchResponse
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public QueryContext queryContext { get; set; }
        public int totalEstimatedMatches { get; set; }
        public int nextOffset { get; set; }
        public int currentOffset { get; set; }
        public List<Value> value { get; set; }
    }
    public class Creator
    {
        public string name { get; set; }
    }

    public class InsightsMetadata
    {
        public int pagesIncludingCount { get; set; }
        public int availableSizesCount { get; set; }
        public VideoObject videoObject { get; set; }
    }

    public class Instrumentation
    {
        public string _type { get; set; }
    }

    public class QueryContext
    {
        public string originalQuery { get; set; }
        public string alterationDisplayQuery { get; set; }
        public string alterationOverrideQuery { get; set; }
        public string alterationMethod { get; set; }
        public string alterationType { get; set; }
    }

    public class Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Value
    {
        public string webSearchUrl { get; set; }
        public string name { get; set; }
        public string thumbnailUrl { get; set; }
        public DateTime datePublished { get; set; }
        public bool isFamilyFriendly { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string contentSize { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string hostPageFavIconUrl { get; set; }
        public string hostPageDomainFriendlyName { get; set; }
        public DateTime hostPageDiscoveredDate { get; set; }
        public Thumbnail thumbnail { get; set; }
        public string imageInsightsToken { get; set; }
        public InsightsMetadata insightsMetadata { get; set; }
        public string imageId { get; set; }
        public string accentColor { get; set; }
    }

    public class VideoObject
    {
        public Creator creator { get; set; }
        public string duration { get; set; }
        public string embedHtml { get; set; }
        public bool allowHttpsEmbed { get; set; }
        public string videoId { get; set; }
        public bool allowMobileEmbed { get; set; }
    }
}
