namespace GrabChromiumLinks.Links;

public static class ExceptionExtensions
{
    public static string GetLegacyExceptionDigest(this Exception exception)
    {
        List<string> ret = new List<string>();
        // while (ex != null)
        foreach (var ex in AsPlainExceptionList(exception))
        {
            ret.Add("[" + ex.GetType().Name + "] " + ex.Message);
        }

        return string.Join(" → ", ret);
    }

    private static IEnumerable<Exception> AsPlainExceptionList(Exception ex)
    {
        while (ex != null)
        {
            if (ex is AggregateException ae)
            {
                foreach (var subException in ae.Flatten().InnerExceptions)
                {
                    yield return subException;
                }
                yield break;
            }

            yield return ex;
            ex = ex.InnerException;
        }
    }


}