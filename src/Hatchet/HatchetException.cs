using System;

namespace Hatchet
{
    public class HatchetException : Exception
    {
        public HatchetException(string message) : base(message) { }
    }
}