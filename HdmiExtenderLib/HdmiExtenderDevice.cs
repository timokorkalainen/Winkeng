using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PcapDotNet.Core;
using PcapDotNet.Core.Extensions;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace HdmiExtenderLib
{
    public class HdmiExtenderDevice
    {

        private IPAddress ip;

        private byte[] latestImage = null;

        private int audioListenerIDCounter = 0;
        private ConcurrentDictionary<int, ConcurrentQueue<byte[]>> registeredAudioListeners = new ConcurrentDictionary<int, ConcurrentQueue<byte[]>>();


        public HdmiExtenderDevice(IPAddress _ip)
        {
            this.ip = _ip;
        }

        /// <summary>
        /// Gets a the latest image data, encoded as image/jpeg.
        /// </summary>
        public byte[] LatestImage
        {
            get
            {
                return latestImage;
            }
        }

        public void SetLatestImage(byte[] image)
        {
            latestImage = image;
        }

        public int RegisterAudioListener(ConcurrentQueue<byte[]> audioData)
        {
            int myKey = Interlocked.Increment(ref audioListenerIDCounter);
            registeredAudioListeners.AddOrUpdate(myKey, audioData, (key, existingValue) => { return audioData; });
            return myKey;
        }

        public void UnregisterAudioListener(int registrationId)
        {
            ConcurrentQueue<byte[]> tmp;
            registeredAudioListeners.TryRemove(registrationId, out tmp);
        }

        public void AddAudioPacket(byte[] data)
        {
            foreach (ConcurrentQueue<byte[]> audioQueue in registeredAudioListeners.Values)
            {
                audioQueue.Enqueue(data);
            }
        }
    }
}
