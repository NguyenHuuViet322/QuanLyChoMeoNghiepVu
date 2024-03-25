using System;

namespace ESD.Utility.CustomClass
{
    public class LogicException : Exception
    {
        public LogicException(string message)
            : base(message) { }
        public LogicException(params string[] messages)
            : base(string.Join("\n", messages)) { }
    }
}
