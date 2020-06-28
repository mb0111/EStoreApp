using System;
using System.Globalization;

namespace EStore.API.Service.Helpers
{
    public class EStoreAPIException : Exception
    {
        public EStoreAPIException() : base() { }

        public EStoreAPIException(string message) : base(message) { }

        public EStoreAPIException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
