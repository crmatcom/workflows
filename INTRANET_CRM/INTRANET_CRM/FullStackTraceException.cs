using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM
{
    /// <summary>
    /// Provides a method for handling any exception types that "eat" the stacktrace in it's implementation of ToString()
    /// </summary>
    [Serializable]
    public class FullStackTraceException : Exception
    {
        private readonly Exception _exception;

        private FullStackTraceException(Exception ex)
        {
            _exception = ex;
        }

        public static Exception Create(Exception exception)
        {
            return CreateInternal((dynamic)exception);
        }

        private static Exception CreateInternal(Exception exception)
        {
            return exception;
        }

        public static FullStackTraceException Create<TDetail>(FaultException<TDetail> exception)
        {
            return new FullStackTraceException(exception);
        }

        private static FullStackTraceException CreateInternal<TDetail>(FaultException<TDetail> exception)
        {
            return new FullStackTraceException(exception);
        }

        public override String ToString()
        {
            String s = _exception.ToString();

            if (_exception.InnerException != null)
            {
                s = String.Format("{0} ---> {1}{2}   --- End of inner exception stack trace ---{2}", s, _exception.InnerException, Environment.NewLine);
            }

            string stackTrace = _exception.StackTrace;
            if (stackTrace != null)
            {
                s += Environment.NewLine + stackTrace;
            }

            return s;
        }
    }
}
