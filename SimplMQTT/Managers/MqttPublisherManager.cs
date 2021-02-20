﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crestron.SimplSharp;
#if USE_LOGGER
using Crestron.SimplSharp.CrestronLogger;
#endif

using SimplMQTT.Client.Events;
using SimplMQTT.Client.Messages;


namespace SimplMQTT.Client.Managers
{
    public class MqttPublisherManager
    {
        private MqttSessionManager sessionManager;
        public event EventHandler<PacketToSendEventArgs> PacketToSend;
        public MqttPublisherManager(MqttSessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        public void Publish(MqttMsgPublish publish)
        {
            sessionManager.AddInflightMessage(publish);
            //Add to queue of outgoing messages
            switch (publish.QosLevel)
            {
                case MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE:
                    {
                        #if USE_LOGGER
                            CrestronLogger.WriteToLog("MQTTPUBLISHERMANAGER - RouteOnQoS - Routing qos0 message", 5);
                        #endif
                        ManageQoS0(publish);
                        break;
                    }
                case MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE:
                    {
                        #if USE_LOGGER
                            CrestronLogger.WriteToLog("MQTTPUBLISHERMANAGER - RouteOnQoS - Routing qos1 message", 5);
                        #endif
                        ManageQoS1(publish);
                        break;
                    }
                case MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE:
                    {
                        #if USE_LOGGER
                            CrestronLogger.WriteToLog("MQTTPUBLISHERMANAGER - RouteOnQoS - Routing qos2 message", 5);
                        #endif
                        ManageQoS2(publish);
                        break;
                    }
                default:
                    break;
            }
        }

        private void ManageQoS2(MqttMsgPublish publish)
        {
            throw new NotImplementedException();
        }

        private void ManageQoS1(MqttMsgPublish publish)
        {
            //TODO : Send , add to inflight messages with waitforpuback start timer , if ack is not provided in time boom
            throw new NotImplementedException();
        }

        private void ManageQoS0(MqttMsgPublish publish)
        {
           OnPacketToSend(publish);
           sessionManager.RemoveInflightMessage(publish.MessageId);
        }

        private void OnPacketToSend(MqttMsgBase packet)
        {
            if (PacketToSend != null)
                PacketToSend(this, new PacketToSendEventArgs(packet));
        }


        internal void ManagePubAck(MqttMsgPuback pubAck)
        {
            sessionManager.RemoveInflightMessage(pubAck.MessageId);
        }
    }
}