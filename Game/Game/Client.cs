using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;

namespace Game
{
    public class Client
    {
        private static NetClient s_client;
        private int myIndex;
        private Boolean ready = false;
        private Game1 game;

        public Boolean isReady()
        {
            return ready;
        }

        public Client(Game1 theGame)
        {
            game = theGame;

            NetPeerConfiguration config = new NetPeerConfiguration("gamajama");
            config.AutoFlushSendQueue = false;
            config.AcceptIncomingConnections = true;
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.StatusChanged);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.Data);
            s_client = new NetClient(config);
            
           
            s_client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
            s_client.Start();

            s_client.DiscoverLocalPeers(14240);
            NetOutgoingMessage hail = s_client.CreateMessage("This is the hail message");
            //s_client.Connect(theGame.hostIP, 14240, hail);
        }

        public void RequestId()
        {
            NetOutgoingMessage om = s_client.CreateMessage();
            om.Write(2);

            s_client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            s_client.FlushSendQueue();
            ready = false;
        }

        // called by the UI
        public void Send(float x, float y)
        {
            NetOutgoingMessage om = s_client.CreateMessage();
            om.Write(1);
            om.Write(myIndex);
            om.Write(x); // very inefficient to send a full Int32 (4 bytes) but we'll use this for simplicity
            om.Write(y);

            s_client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            Console.WriteLine("Sending '" + x + y + "'");
            s_client.FlushSendQueue();
        }

        // called by the UI
        public void SendKill(int typeOfKill, int index)
        {
            NetOutgoingMessage om = s_client.CreateMessage();
            om.Write(4);
            om.Write(index);
             om.Write(typeOfKill);

            s_client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            s_client.FlushSendQueue();
        }


        public void GotMessage(object peer)
        {
            NetIncomingMessage im;
            while ((im = s_client.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        s_client.Connect(im.SenderEndPoint);
                        ready = true;
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
                        Console.WriteLine(status.ToString());

                        //Console.WriteLine(status.ToString() + ": " + x + " " + y);

                        break;
                    case NetIncomingMessageType.Data:
                        int msgType = im.ReadInt32();

                        if (msgType == 1)
                        {
                            int dex = im.ReadInt32();
                            float x = im.ReadFloat();
                            float y = im.ReadFloat();
                            game.setPlayerPos(dex, x, y);
                        }
                        else if (msgType == 2)
                        {
                            int randSeed = im.ReadInt32();
                            //Game1.randSeed = randSeed;
                            myIndex = im.ReadInt32();
                            game.setMyIndex(myIndex);
                            Console.WriteLine("my player id: " + myIndex);
                        }
                        else if (msgType == 3)
                        {
                            //startgame...
                            ready = true;
                            game.setMsgFreq(0);
                        }
                        else if (msgType == 4)
                        {
                             int dex = im.ReadInt32();
                            int typeOfKill = im.ReadInt32();
                            if (typeOfKill == 1)
                            {
                                game.killAi(dex);
                            }
                            else
                            {
                               game.killPlayer(dex);
                            }
                        }
                        break;

                        //string chat = im.ReadString();
                        //Console.WriteLine(chat);
                        //break;
                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }
        }


    }
}
