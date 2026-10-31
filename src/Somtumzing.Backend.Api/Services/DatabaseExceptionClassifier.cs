using System.Data.Common;

namespace Somtumzing.Backend.Api.Services;

internal static class DatabaseExceptionClassifier
{
    public static bool ContainsDbException(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is DbException)
            {
                return true;
            }
        }

        return false;
    }
}
