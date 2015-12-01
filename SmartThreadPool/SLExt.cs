#define _SILVERLIGHT
#define _MONO
#if _SILVERLIGHT

using System.Threading;

namespace Amib.Threading
{
    public enum ThreadPriority
    {
        Lowest,
        BelowNormal,
        Normal,
        AboveNormal,
        Highest,
    }
}
#endif
