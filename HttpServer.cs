using System.Net;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Web;

class HttpServer(String host, int port, String root)
{
    private readonly Socket server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static readonly Dictionary<String, String> MIMES = new() {
        {"html", "text/html"},
        {"css", "text/css"},
        {"js", "application/javascript"},
        {"png", "image/png"},
        {"ttf", "font/ttf"}
    };

    public void Start()
    {
        server.Bind(new IPEndPoint(IPAddress.Parse(host), port));
        server.Listen();
        while (server.IsBound)
        {
            using Socket client = server.Accept();
            using NetworkStream str = new(client);
            using StreamWriter writer = new(str);
            String? req = new StreamReader(str).ReadLine();
            if (req == null) continue;
            if (req.StartsWith("GET /") && req.EndsWith(" HTTP/1.1"))
            {
                String resource = req[(req.IndexOf('/') + 1)..];
                resource = HttpUtility.UrlDecode(resource[..resource.IndexOf(' ')]);
                if (resource == "")
                    resource = "index.html";
                String path = root + "/" + resource;
                String msg = "404 Not Found";
                String? contentType = null;
                long contentLength = 0;
                Stream contentStream = new MemoryStream();
                if (File.Exists(path))
                {
                    contentStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    msg = "200 OK";
                    contentLength = new FileInfo(path).Length;
                    int index = path.LastIndexOf('.');
                    if (index > -1)
                    {
                        String ext = path[(index + 1)..];
                        if (MIMES.TryGetValue(ext, out string? value))
                            contentType = value;
                    }
                }

                writer.WriteLine("HTTP/1.1 " + msg);
                writer.WriteLine("Server: HttpServer");
                writer.WriteLine("Content-Length: " + contentLength);
                if (contentType != null)
                    writer.WriteLine("Content-Type: " + contentType);
                writer.WriteLine();
                writer.Flush();

                contentStream.CopyTo(str);
                str.Flush();
            }
        }
    }
}