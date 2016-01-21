//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// NetGameClient - Class of some higher level game related logic associated with NetClient's lower level tcpclient
// Created: January 20 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 20 2016
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
    public class NetGameClient
    {
        public void SendMessage(NetMessageType msgType, byte[] data, bool useCompression)
        {
        }
    }
}