//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// Network - Various classes used by both server and client network
// Created: January 19 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 19 2016
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
    #region Enums
    [System.Flags]
    public enum NetworkConnectionStatus
    {
        Unconnected, //When unconnected
        ConnectionFailed, //Connection was attempted but failed for some reason
        Connected, //Is connected
        ClientDisconnect, //Was connected, but client ended connection
        ServerDisconnect, //Was connected, but server ended connection
        UnknownError //Unknown problem, like a timeout or network failure
    }

    public enum NetMessageType : byte
    {
        RESERVED = 0,

        CONNECTIONSTART = 1,
        CONNECTIONESTABLISHED = 2,
        DISCONNECT = 3,
        PING = 4,
        PONG = 5
    }
    #endregion

    #region Delegates
    public delegate void OnTcpClientConnect(NetGameClient newConnection);
    public delegate void OnTcpClientDataReceived(NetMessage message);
    public delegate void OnTcpClientDataTransmit();
    public delegate void OnTcpClientDisconnect(IPEndPoint remoteEP, NetworkConnectionStatus connectionStatus);

    public delegate void OnTcpServerClientConnect();
    public delegate void OnTcpServerDataReceived();
    public delegate void OnTcpServerDataTransmit();
    public delegate void OnTcpServerClientDisconnect();
    #endregion

    #region Utility
    //Inspired greatly by Lidgren's UDP Library
    //https://github.com/lidgren/lidgren-network-gen3/blob/master/Lidgren.Network/NetUtility.cs
    public static class NetUtility
    {
        /// <summary>
        /// Resolve IP address callback
        /// </summary>
        /// <param name="ipAddr"></param>
        public delegate void ResolveAddressCallback(IPAddress ipAddr);

        public static void ResolveAsync(string ipOrHost, ResolveAddressCallback ipAddrCallback)
        {
            if(string.IsNullOrEmpty(ipOrHost))
            {
                Debug.LogException(new System.ArgumentException("String cannot be empty", "ipOrHost"));
            }

            ipOrHost = ipOrHost.Trim();

            IPAddress ipAddr = null;

            //IP
            if (IPAddress.TryParse(ipOrHost, out ipAddr))
            {
                if (ipAddr.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddrCallback(ipAddr);
                    return;
                }

                Debug.LogException(new System.Exception(string.Format("Cannot handle '{0}'. Use IPv4 only.", ipAddr.AddressFamily.ToString())));
            }

            //Hostname
            IPHostEntry entry;
            try
            {
                Dns.BeginGetHostEntry(ipOrHost, delegate(System.IAsyncResult result)
                {
                    try
                    {
                        entry = Dns.EndGetHostEntry(result);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.HostNotFound)
                        {
                            //LogWrite(string.Format(CultureInfo.InvariantCulture, "Failed to resolve host '{0}'.", ipOrHost));
                            ipAddrCallback(null);
                            return;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (entry == null)
                    {
                        ipAddrCallback(null);
                        return;
                    }

                    // check each entry for a valid IP address
                    foreach (var ipCurrent in entry.AddressList)
                    {
                        if (ipCurrent.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrCallback(ipCurrent);
                            return;
                        }
                    }

                    ipAddrCallback(null);
                }, null);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.HostNotFound)
                {
                    //LogWrite(string.Format(CultureInfo.InvariantCulture, "Failed to resolve host '{0}'.", ipOrHost));
                    ipAddrCallback(null);
                }
                else
                {
                    throw;
                }
            }
        }
    }
    #endregion
}