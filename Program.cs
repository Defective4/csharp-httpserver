using System.Reflection;

class Program
{
    public static void Main(String[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: <host> <port> <path>");
            return;
        }

        String host = args[0];
        int port;

        try
        {
            port = Convert.ToInt32(args[1]);
        }
        catch
        {
            Console.Error.WriteLine(args[1] + " is not a valid port");
            return;
        }

        String path = args[2];
        if (!Directory.Exists(path))
        {
            Console.Error.WriteLine(args[2] + " is not a valid directory");
            return;
        }

        new HttpServer(host, port, path).Start();
    }
}