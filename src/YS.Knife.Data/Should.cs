namespace YS.Knife
{
    public static class Should
    {
        public static void BeTrue(bool condition, int errorCode, string message, object args = null)
        {
            if (!condition)
            {
                throw KnifeException.FromTemplate(errorCode, message, args);
            }
        }
        public static void BeFalse(bool condition, int errorCode, string message, object args = null)
            => BeTrue(!condition, errorCode, message, args);
        public static void BeNull(object instance, int errorCode, string message, object args = null)
            => BeTrue(instance == null, errorCode, message, args);
        public static void BeNotNull(object instance, int errorCode, string message, object args = null)
            => BeTrue(instance != null, errorCode, message, args);

    }
}
