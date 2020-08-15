using YS.Knife.Hosting;

namespace StaticFileServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            KnifeWebHost.Start(args);
        }
    }
}
