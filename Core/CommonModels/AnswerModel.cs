namespace Core.CommonModels
{
    using System.Collections.Generic;
    using System.Linq;

    using Core.Constants;
    using Core.Tools;

    /// <summary>
    ///     Class AnswerModel
    /// </summary>
    public class AnswerModel
    {
        /// <summary>
        ///     Gets or sets all words inside.
        /// </summary>
        /// <value>All words inside.</value>
        public List<string> AllWordsInside { get; set; }

        /// <summary>
        ///     Gets or sets the alternatives.
        /// </summary>
        /// <value>The alternatives.</value>
        public List<AnswerModel> Alternatives { get; set; }

        /// <summary>
        ///     Gets or sets the single answer.
        /// </summary>
        /// <value>The single answer.</value>
        public string SingleAnswer { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public AnswerType Type { get; set; }
    }

    /// <summary>
    ///     Class AnswerModelHelper
    /// </summary>
    public static class AnswerModelHelper
    {
        /// <summary>
        ///     Splits the answer.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>IEnumerable{AnswerModel}.</returns>
        public static List<AnswerModel> SplitAnswer(string inputString)
        {
            var answersCollection = inputString.SplitStringByDelimiter();
            if (!answersCollection.AnyValues())
            {
                return new List<AnswerModel>();
            }

            var result = new List<AnswerModel>();
            foreach (var answer in answersCollection)
            {
                var alternativesCollection = answer.SplitStringByDelimiter(CommonConstants.CommaDelimiter);
                if (alternativesCollection.Count != 1)
                {
                    FillAlternatives(alternativesCollection, ref result);
                    continue;
                }

                FillSingleAnswerOrAllWords(answer, ref result);
            }

            return result;
        }

        private static void FillAlternatives(List<string> alternatives, ref List<AnswerModel> result)
        {
            var alternativeResult = new List<AnswerModel>();
            foreach (var alternative in alternatives)
            {
                FillSingleAnswerOrAllWords(alternative, ref alternativeResult);
            }

            var model = new AnswerModel { Type = AnswerType.Alternatives, Alternatives = alternativeResult };
            result.Add(model);
        }

        private static void FillSingleAnswerOrAllWords(string answer, ref List<AnswerModel> result)
        {
            var allWordsCollection = answer.SplitStringByDelimiter(CommonConstants.Delimiter).Select(w=>w.ToLower()).ToList();
            if (allWordsCollection.Count == 1)
            {
                result.Add(
                    new AnswerModel { SingleAnswer = allWordsCollection.First(), Type = AnswerType.SingleAnswer });
            }
            else
            {
                var model = new AnswerModel { Type = AnswerType.AllWordsInside, AllWordsInside = allWordsCollection };
                result.Add(model);
            }
        }
    }

    /// <summary>
    ///     Enum AnswerType
    /// </summary>
    public enum AnswerType
    {
        /// <summary>
        ///     The single answer
        /// </summary>
        SingleAnswer = 0,

        /// <summary>
        ///     The alternatives
        /// </summary>
        Alternatives = 1,

        /// <summary>
        ///     All words inside
        /// </summary>
        AllWordsInside = 2
    }
}