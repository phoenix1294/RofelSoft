using RofelSoft.Services.Market.Enums;
using System.Text;

namespace RofelSoft.Services.Market
{
    class HttpResponse
    {
        public string Content { get; set; }
        public HttpStatus Status { get; }

        public HttpResponse(string content, HttpStatus status)
        {
            Status = status;
            Content = content;
        }

        public byte[] Serialize()
        {
            switch (Status)
            {
                case HttpStatus.OK:
                    {
                        var content = Encoding.UTF8.GetBytes(Content);
                        var headers = Encoding.UTF8.GetBytes($"HTTP/1.1 {(int)Status} OK\n" +
                        "Server: RofelSoft.inc WebMarket\n" +
                        "Content-Type: text/html\n" +
                        "Connection: keep-alive\n" +
                        $"Content-Length: {content.Length}\n\n");
                        var res = new byte[content.Length + headers.Length];
                        headers.CopyTo(res, 0);
                        content.CopyTo(res, headers.Length);
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
