namespace RF.WPF.Extensions
{
    public static class ObjectExtensions
    {
        public static string TypeName(this object obj) => obj.GetType().ToString();
    }
}
