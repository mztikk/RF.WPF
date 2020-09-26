using System;

namespace RF.WPF
{
    public static class FuncCache
    {
        public static Func<bool> True = () => true;
        public static Func<bool> False = () => false;
    }
}
