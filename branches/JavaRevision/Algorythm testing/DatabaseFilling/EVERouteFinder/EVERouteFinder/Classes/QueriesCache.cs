using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVERouteFinder.Classes
{
    public static class QueriesCache
    {
        public static Dictionary<int, string> systems;
        public static Dictionary<int, List<string>> jumpsInSystem;
    }
}
