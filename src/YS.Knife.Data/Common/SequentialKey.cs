using System;

namespace YS.Knife.Data
{
    public static class SequentialKey
    {
        private static readonly Random Random = new Random(unchecked((int)DateTime.Now.Ticks));
        private static int _sequenceValue = 0;
        private static long _lastTicks = 0;
        /// <summary>
        /// Create a new string key of length 24.
        /// </summary>
        /// <returns>A new key of length 24.</returns>
        public static string NewString()
        {
            // 15+3+6 
            lock (Random)
            {
                long timestamp = DateTimeOffset.UtcNow.Ticks;
                if (timestamp != _lastTicks)
                {
                    _lastTicks = timestamp;
                    _sequenceValue = 0;
                }
                else
                {
                    _sequenceValue++;
                }
                return $"{timestamp:x15}{(_sequenceValue & 0x00000fff):x3}{Random.Next(0xffffff):x6}";
            }
        }

    }

}
