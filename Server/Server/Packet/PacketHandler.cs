using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PacketHandler
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;
        Console.WriteLine($"PlayerId : {p.playerId}, PlayerName : {p.name}");

        foreach (C_PlayerInfoReq.Skile skile in p.skiles)
        {
            Console.WriteLine($"ID : {skile.id}, Level : {skile.level}, duration : {skile.duration}");
        }
    }
}
