using System;

namespace InstantMessenger.Core.Base
{
    public interface IBDO
    {
        long OID { get; set; }
        DateTime Created { get; set; }
    }
}
