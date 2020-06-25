using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Development
{
    public static class DevHelper
    {

        public static TimeSpan Measure(Action a)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            a.Invoke();

            watch.Stop();
            return watch.Elapsed;
        }
    }
}
