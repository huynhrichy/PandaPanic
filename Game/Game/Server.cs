using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;

namespace Game
{
    class Server
    {
        private static NetServer s_server;
        int connectedClients = 0;
        List<NetConnection> clients = new List<NetConnection>();

        public Server()
        {
            // set up network
            NetPeerConfiguration config = new NetPeerConfiguration("gamajama");
            config.MaximumConnections = 100;
            config.UseMessageRecycling = true;
            config.Port = 14240;
            config.AcceptIncomingConnections = true;
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.StatusChanged);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.Data);
            s_server = new NetServer(config);
            s_server.RegisterReceivedCallback(new SendOrPostCallback(getData));
            s_server.Start();
        }


        public void getData(object peer)
        {
                NetIncomingMessage im;
                while ((im = s_server.ReadMessage()) != null)
                {
                    // handle incoming message
                    switch (im.MessageType)
                    {

                        case NetIncomingMessageType.DiscoveryRequest:
                            NetOutgoingMessage ome = s_server.CreateMessage();
                            s_server.SendDiscoveryResponse(ome, im.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = im.ReadString();
                            Console.WriteLine(text);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string reason = im.ReadString();
                            Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            UpdateConnectionsList();
                            break;
                        case NetIncomingMessageType.Data:
                            // incoming chat message from a client
                            int msgType = im.ReadInt32();


                            if (msgType == 1)
                            {

                                int index = im.ReadInt32();
                                float x = im.ReadFloat();
                                float y = im.ReadFloat();

                                // broadcast this to all connections, except sender
                                List<NetConnection> all = s_server.Connections; // get copy
                                all.Remove(im.SenderConnection);

                                if (all.Count > 0)
                                {
                                    NetOutgoingMessage om = s_server.CreateMessage();
                                    om.Write(1);
                                    om.Write(index);
                                    om.Write(x);
                                    om.Write(y);
                                    s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                                    s_server.FlushSendQueue();
                                }

                            }
                            else if (msgType == 2)
                            {
                                if (!clients.Contains(im.SenderConnection))
                                {

                                    clients.Add(im.SenderConnection);
                                    NetOutgoingMessage om = s_server.CreateMessage();
                                    om.Write(2);
                                    om.Write(10);
                                    om.Write(connectedClients++);
                                    s_server.SendMessage(om, im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                                    s_server.FlushSendQueue();
                                }


                                if (clients.Count == 2)
                                {
                                    NetOutgoingMessage om = s_server.CreateMessage();
                                    om.Write(3);
                                    s_server.SendMessage(om, s_server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                                    s_server.FlushSendQueue();

                                }

                            }
                            else if (msgType == 4)
                            {
                                int index = im.ReadInt32();
                                int type = im.ReadInt32();
                                NetOutgoingMessage om = s_server.CreateMessage();
                                om.Write(4);
                                om.Write(index);
                                om.Write(type); 
                                
                                List<NetConnection> all = s_server.Connections; // get copy
                                all.Remove(im.SenderConnection);
                                if (all.Count > 0)
                                {
                                    s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                                    s_server.FlushSendQueue();
                                }
                            }

                            break;

                        default:
                            Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                            break;
                    }
                }
            

        }

        private static void UpdateConnectionsList()
        {


            foreach (NetConnection conn in s_server.Connections)
            {
                string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
                Console.WriteLine(str);
            }
        }



    }
}
