using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace ServerCore
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        public static int ChunkSize { get; set; } = 4096 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if(CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            if(CurrentBuffer.Value.FreeSize > reserveSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }
        public static ArraySegment<byte> Close(int _usedSize)
        {
            return CurrentBuffer.Value.Close(_usedSize);
        }
    }
    public class SendBuffer
    {
        byte[] buffer;
        int usedSize = 0;

        public SendBuffer(int chunkSize)
        {
            buffer = new byte[chunkSize];

        }

        public int FreeSize { get { return buffer.Length - usedSize; } }

        public  ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize) return null;

            return new ArraySegment<byte>(buffer, usedSize, reserveSize);
        }
        public ArraySegment<byte> Close(int _usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, usedSize, _usedSize);
            usedSize += _usedSize;
            return segment;
        }

    }
}
