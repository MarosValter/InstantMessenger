﻿using System;

namespace InstantMessenger.Common.TransportObject
{
    public class SerializationException : Exception
    {
        public SerializationException(string message, Type t)
            : base(string.Format(message, t.FullName))
        { }

        public SerializationException(string message)
            : base(message)
        { }
    }
}
