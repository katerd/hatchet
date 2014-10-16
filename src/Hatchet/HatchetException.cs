using System;
using System.Runtime.Serialization;

namespace Hatchet
{
    public class HatchetException : Exception
    {
        public HatchetException() { }
        public HatchetException(string message) : base(message) { }
        public HatchetException(string message, Exception innerException) : base(message, innerException) { }
        protected HatchetException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}