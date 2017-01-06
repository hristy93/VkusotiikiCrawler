using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    class StaticRandom
    {
        private static int _seed;

        private static ThreadLocal<Random> threadLocal = new ThreadLocal<Random>
            (() => new Random(Interlocked.Increment(ref _seed)));

        static StaticRandom()
        {
            _seed = Environment.TickCount;
        }

        public static Random Instance { get { return threadLocal.Value; } }
    }
}
