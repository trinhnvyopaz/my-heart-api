using HtmlAgilityPack;

namespace MyHeart.Functions.Utils
{
    public static class TextHelper
    {
        public static string ExtractTextFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var extractedText = htmlDocument.DocumentNode.InnerText;
            return extractedText;
        }
    }
}
