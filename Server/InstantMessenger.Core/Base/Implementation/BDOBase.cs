using System;

namespace InstantMessenger.Core.Base.Implementation
{
    public class BDOBase : IBDO
    {
        public virtual long OID { get; set; }
        public virtual DateTime Created { get; set; }
    }
}
