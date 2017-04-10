using System;

namespace HotKeySystem_example.HotKeySystem
{
    public class HotKeyNotDeclaredException : Exception
    {
        public HotKeyNotDeclaredException()
            :base()
        {
        }

        public HotKeyNotDeclaredException(string message)
            :base(message)
        {
        }

        public HotKeyNotDeclaredException(string message, Exception innerException)
            :base(message, innerException)
        {
        }
    }
}
