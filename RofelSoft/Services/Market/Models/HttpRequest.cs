using System.Text;

namespace RofelSoft.Services.Market.Models
{
    class HttpRequest
    {
        public string Type { get; }
        public string Path { get; }
        public bool Status { get; }
        public string[] Headers { get; }

        public HttpRequest(byte[] request, int len)
        {
            var str = Encoding.UTF8.GetString(request, 0, len).Trim();
            Headers = str.Split('\n');
            var head = Headers[0].Split(' ');
            try
            {
                Type = head[0];
                Path = head[1];
                Status = true;
            }
            catch
            {
                Status = false;
            }
        }

        public string ExtractHeader(string key)
        {
            foreach (string header in Headers)
            {
                if (header.Contains(key + ":"))
                {
                    var idx = header.IndexOf(":");
                    return header.Substring(idx + 2, header.Length - idx - 2).Replace("\r", "");
                }
            }
            return "";
        }
    }
}
