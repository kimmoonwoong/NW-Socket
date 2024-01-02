
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

public enum PacketID
{
    
    C_Chat = 0,

    S_Chat = 1,

}
interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> buffer);
	ArraySegment<byte> Write();
}


class C_Chat : IPacket
{
    public string chat;

    public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }

    /*
        * 모든 Packet에서 쓰는 것, 읽는 것 모두 동일하게 작업하므로 클래스의 메서드화
        */
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);  
            
        count += (ushort)Marshal.SizeOf(count);
        /*
            * 여기서 Array.Copy가 아닌 TryWriteBytes를 쓰는 이유
            * Array.Copy를 사용하면 new를 통해 새로운 객체를 만들어서 복사하는 작업을 함
            * 하지만 TryWriteByte는 span 클래스, 즉 어떤 array에 직접 쓰는 것으로 작업을 한다.
            * 그렇기에 TryWriteByte가 더 효율적이다.
            */
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_Chat);
        count += sizeof(ushort);

        
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
		count += chatLen;
		count += sizeof(ushort);
		            
		

        success &= BitConverter.TryWriteBytes(s, count);


        if (!success) return null;

        return SendBufferHelper.Close(count);

    }
    public void Read(ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(buffer.Array, buffer.Offset, buffer.Count); ;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
		ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += chatLen;
		
    }
}


class S_Chat : IPacket
{
    public int playerId;
	public string chat;

    public ushort Protocol { get { return (ushort)PacketID.S_Chat; } }

    /*
        * 모든 Packet에서 쓰는 것, 읽는 것 모두 동일하게 작업하므로 클래스의 메서드화
        */
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);  
            
        count += (ushort)Marshal.SizeOf(count);
        /*
            * 여기서 Array.Copy가 아닌 TryWriteBytes를 쓰는 이유
            * Array.Copy를 사용하면 new를 통해 새로운 객체를 만들어서 복사하는 작업을 함
            * 하지만 TryWriteByte는 span 클래스, 즉 어떤 array에 직접 쓰는 것으로 작업을 한다.
            * 그렇기에 TryWriteByte가 더 효율적이다.
            */
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_Chat);
        count += sizeof(ushort);

        
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
		count += sizeof(int);
		
		
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
		count += chatLen;
		count += sizeof(ushort);
		            
		

        success &= BitConverter.TryWriteBytes(s, count);


        if (!success) return null;

        return SendBufferHelper.Close(count);

    }
    public void Read(ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(buffer.Array, buffer.Offset, buffer.Count); ;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		
		ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += chatLen;
		
    }
}


