using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class PBAgent : PooledClassObject
    {
        public virtual ProtoBuf.IExtensible GetMsg() { return null; }
    }
}
