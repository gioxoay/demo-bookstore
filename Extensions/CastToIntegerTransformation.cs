using HtmlAgilityPack;
using OpenScraping.Transformations;

namespace BookStore.Extensions
{
    /// <summary>
    /// Class to cast selected XPath value to <see cref="int"/>.
    /// </summary>
    public class CastToIntegerTransformation : ITransformationFromHtml, ITransformationFromObject
    {
        public object Transform(Dictionary<string, object> settings, HtmlNodeNavigator nodeNavigator, List<HtmlAgilityPack.HtmlNode> logicalParents)
        {
            var text = nodeNavigator?.Value ?? nodeNavigator?.CurrentNode?.InnerText;

            if (text != null)
            {
                int intVal;

                if (int.TryParse(text, out intVal))
                {
                    return intVal;
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
            if (int.TryParse(input.ToString(), out int number))
            {
                return number;
            }

            throw new FormatException($"Input parameter {input} is not a valid integer!");
        }
    }
}