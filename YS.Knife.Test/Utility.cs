using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace YS.Knife.Test
{
    public static class Utility
    {
        public static uint GetAvailableTcpPort(uint start = 1024, uint stop = IPEndPoint.MaxPort)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endPoints = ipGlobalProperties.GetActiveTcpListeners();
            for (uint i = start; i <= stop; i++)
            {
                if (!endPoints.Any(p => p.Port == i))
                {
                    return i;
                }
            }
            throw new ApplicationException("Not able to find a free TCP port.");
        }
        const string fullCode = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

        public static string NewPassword(int length = 32)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder passwordBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                passwordBuilder.Append(fullCode[random.Next(fullCode.Length)]);
            }
            return passwordBuilder.ToString();
        }

    }
}
