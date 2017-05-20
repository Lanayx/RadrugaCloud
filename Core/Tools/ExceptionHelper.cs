namespace Core.Tools
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    ///     Class ExceptionHelper
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        ///     To the string extended.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>System.String.</returns>
        public static string ToStringExtended(this Exception exception)
        {
            return ProcessException(exception);
        }

        /// <summary>
        ///     Traces the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="nestLevel">The nest level.</param>
        public static void Trace(this Exception exception, int nestLevel = 0)
        {
            var message = string.Format(
                "Method: '{0}'.\nException:\n{1}",
                GetCurrentMethodName(nestLevel),
                exception.ToStringExtended());
            System.Diagnostics.Trace.TraceError(message);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethodName(int nestLevel = 0)
        {
            var st = new StackTrace();
            if (nestLevel == 0)
            {
                var sf = st.GetFrame(nestLevel + 2);
                return sf.GetMethod().Name;
            }

            var sb = new StringBuilder();
            for (var i = nestLevel; i >= 0; i--)
            {
                var sf = st.GetFrame(i + 2);
                sb.Append(sf.GetMethod().Name);
                if (i != 0)
                {
                    sb.Append(".");
                }
            }

            return sb.ToString();
        }

        private static string ProcessException(Exception ex, bool isInner = false)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            if (!isInner)
            {
                sb.Append("\n__________________Start of detailed information for exception__________________\n\n");
            }

            sb.Append(
                string.Format(
                    "{0}: {1}; Message: {2}\n",
                    isInner ? "Inner exception" : "Exception",
                    ex.GetType(),
                    ex.Message));
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                sb.Append("==================================StackTrace===================================\n");
                sb.Append(ex.StackTrace + "\n");
                sb.Append("===============================================================================\n\n");
            }
            else
            {
                sb.Append("\n");
            }

            if (ex.InnerException != null)
            {
                sb.Append(ProcessException(ex.InnerException, true));
            }

            if (!isInner)
            {
                sb.Append("___________________End of detailed information for exception___________________");
            }

            return sb.ToString();
        }
    }
}