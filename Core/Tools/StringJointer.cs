namespace Core.Tools
{
    using System.Collections.Generic;
    using System.Linq;

    using Core.Constants;

    /// <summary>
    /// Class StringJointer
    /// </summary>
    public static class StringJointer
    {
        /// <summary>
        /// Joins to string semicolon separated.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initialCollection">The initial collection.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>System.String.</returns>
        public static string JoinToString<T>(this IEnumerable<T> initialCollection, string delimiter = CommonConstants.StringsDelimiter)
        {
            var collection = initialCollection.ToIList();

            if (collection == null)
                return null;
            
            if (!collection.Any())
            {
                return string.Empty;
            }

            var type = typeof(T);
            IEnumerable<string> stringList;
            if (type.IsValueType)
            {
                stringList = collection.Select(e => e.ToString()).Where(s => !string.IsNullOrEmpty(s));
            }
            else
            {
                stringList =
                    // ReSharper disable once CompareNonConstrainedGenericWithNull
                    collection.Where(e => e != null)
                        .Select(e => e.ToString())
                        .Where(s => !string.IsNullOrEmpty(s));
            }

            return string.Join(delimiter, stringList);
        }

        /// <summary>
        /// Splits the string by semicolon.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>List{System.String}.</returns>
        public static List<string> SplitStringByDelimiter(this string inputString, string delimiter = CommonConstants.StringsDelimiter)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return null;
            }

            var charDelimiter = delimiter.ToCharArray();

            inputString = inputString.Trim(charDelimiter);
            return inputString.Split(charDelimiter).ToList();
        }
    }
}