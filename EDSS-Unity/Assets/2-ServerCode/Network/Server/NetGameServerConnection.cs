//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// NetGameServerConnection - Class of some higher level game related logic associated with NetServer's lower level tcpclient, as used by the server
// Created: January 23 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 23 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;
using EveryDaySpaceStation.Network;

namespace EveryDaySpaceStation.Network
{
    public class NetGameServerConnection
    {
        public void SendMessage(NetMessageType msgType, byte[] data, bool useCompression)
        {
        }
    }
}