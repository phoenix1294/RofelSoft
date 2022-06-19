using RofelSoft.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RofelSoft.Models
{
    class Packet
    {
        public Packet(string text, RequestType requestType)
        {
            RequestType = requestType;
            Text = text;
        }

        public RequestType RequestType { get; set; }

        public string Text { get; set; }

        private readonly Random Random = new Random();

        public byte[] Content
        {
            get
            {
                List<byte> Buffer = new List<byte>();

                int RequestId = Random.Next(0, int.MaxValue);

                byte[] CommandBytes = Encoding.UTF8.GetBytes(Text);

                int ContentLength = 10 + CommandBytes.Length;

                Buffer.AddRange(BitConverter.GetBytes(ContentLength));      // 4
                Buffer.AddRange(BitConverter.GetBytes(RequestId));          // 4
                Buffer.AddRange(BitConverter.GetBytes((int)RequestType)); // 4
                Buffer.AddRange(CommandBytes);                  // N
                Buffer.AddRange(new byte[] { 0x00, 0x00 });     // 2

                return Buffer.ToArray();
            }
            private set { }
        }

    }
}
