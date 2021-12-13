using HtmlAgilityPack;
using OpenScraping.Transformations;

namespace BookStore.Extensions
{
    /// <summary>
    /// Class to cast selected XPath value to <see cref="float"/>.
    /// </summary>
    public class CastToFloatTransformation : ITransformationFromHtml, ITransformationFromObject
    {
        public object Transform(Dictionary<string, object> settings, HtmlNodeNavigator nodeNavigator, List<HtmlAgilityPack.HtmlNode> logicalParents)
        {
            var text = nodeNavigator?.Value ?? nodeNavigator?.CurrentNode?.InnerText;

            if (text != null)
            {
                float floatVal;

                if (float.TryParse(text, out floatVal))
                {
                    return floatVal;
                }
            }

            return null;
        }

        /// <summary>
        /// Transforms the input to a valid <see cref="int"/>.
        /// </summary>
        /// <param name="settings"><seealso cref="Config.TransformationConfig.ConfigAttributes"/>.</param>
        /// <param name="input">Parsed XPath value.</param>
        /// <returns><see cref="int"/>.</returns>
        /// <exception cref="FormatException">Occurs when the <paramref name="input" /> parameter
        /// is not a valid integer.</exception>
        public object Transform(Dictionary<string, object> settings, object input)
        {
            if (float.TryParse(input.ToString(), out float number))
            {
                return number;
            }

            throw new FormatException($"Input parameter {input} is not a valid integer!");
        }
    }
}