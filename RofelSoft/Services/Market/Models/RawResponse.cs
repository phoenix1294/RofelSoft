using RofelSoft.Services.Market.Enums;
using System.Text;

namespace RofelSoft.Services.Market
{
    class RawResponse
    {
        public byte[] Content { get; set; }
        public HttpStatus Status { get; }
        public string CType { get; set; }

        public RawResponse(byte[] content, string ctype, HttpStatus status)
        {
            Content = content;
            Status = status;
            CType = ctype;
        }

        public byte[] Serialize()
        {
            switch (Status)
            {
                case HttpStatus.OK:
                    {
                        var headers = Encoding.UTF8.GetBytes($"HTTP/1.1 {(int)Status} OK\n" +
                        "Server: RofelSoft.inc WebMarket\n" +
                        $"Content-Type: {CType}\n" +
                        "Connection: keep-alive\n" +
                        $"Content-Length: {Content.Length}\n\n");
                        var res = new byte[Content.Length + headers.Length];
                        headers.CopyTo(res, 0);
                        Content.CopyTo(res, headers.Length);
                        return res;
                    }
                default:
                    {
                        return Encoding.UTF8.GetBytes($"HTTP/1.1 404 Not Found\n" +
                        "Server: RofelSoft.inc WebMarket\n" +
                        "Connection: keep-alive\n" +
                        $"Content-Length: {"Unknown".Length}\n\n" +
                        $"Unknown");
                    }

            }

        }
    }
}
