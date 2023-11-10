using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;
        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> array);
        public abstract void Initialise(Packet packet);   
    }
}
