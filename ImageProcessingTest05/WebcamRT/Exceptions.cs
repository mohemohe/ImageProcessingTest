using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebcamRT
{
    [Serializable]
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException() : base() { }
        public DeviceNotFoundException(string message) : base(message) { }
        public DeviceNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected DeviceNotFoundException(SerializationInfo info, StreamingContext context) { }
    }

    [Serializable]
    public class DeviceNotInitializedException : Exception
    {
        public DeviceNotInitializedException() : base() { }
        public DeviceNotInitializedException(string message) : base(message) { }
        public DeviceNotInitializedException(string message, Exception inner) : base(message, inner) { }
        protected DeviceNotInitializedException(SerializationInfo info, StreamingContext context) { }
    }
}
