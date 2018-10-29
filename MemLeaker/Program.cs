namespace MemLeaker
{
    /// <summary>
    /// This program reads one Active Directory group and gets all users from it.
    /// It is used to demonstrate how to use the Visual Studio tools to find managed and native memory leaks in your code.
    /// </summary>
    class Program
    {
        public static string AdDomain = "bndev.local";
        public static string AdGroupName = "Domain Users";

        static void Main(string[] args)
        {
            //var leak1 = new AdLeak1();
            //while (true)
            //{
            //    leak1.Execute();
            //}

            while (true)
            {
                var leak2 = new AdLeak2();
                leak2.Execute();
            }

            //var leak3 = new AdLeak3();
            //while (true)
            //{
            //    leak3.Execute();
            //}

            var noleak = new AdNoLeak();
            while (true)
            {
                noleak.Execute();
            }
        }
    }
}
