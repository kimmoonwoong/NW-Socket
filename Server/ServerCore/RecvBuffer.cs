using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> buffer;
        int readPos;
        int writePos;
        public RecvBuffer(int bufsize)
        {
            buffer = new ArraySegment<byte>(new byte[bufsize], 0, bufsize);
        }

        public int DataSize { get { return writePos - readPos; } }
        public int FreeSize { get { return buffer.Count - writePos; } }

        public ArraySegment<byte> DataSegment { get { return new ArraySegment<byte>(buffer.Array, buffer.Offset + readPos, DataSize); } }

        public ArraySegment<byte> RecvSegment { get { return new ArraySegment<byte>(buffer.Array, buffer.Offset + writePos, FreeSize); } }
    
        public void Clean()
        {
            int dataSize = DataSize;

            if(dataSize == 0)
            {
                readPos = writePos = 0;
            }
            else
            {
                Array.Copy(buffer.Array, buffer.Offset + readPos, buffer.Array, buffer.Offset, dataSize);
                readPos = 0;
                writePos = dataSize;
            }
        }

        public bool OnRead(int numBytes)
        {
            if (numBytes > DataSize)
                return false;

            readPos += numBytes;
            return true;
        }

        public bool OnWrite(int numBytes)
        {
            if (numBytes > FreeSize)
                return false;

            writePos += numBytes;
            return true;
        }

    }
}
