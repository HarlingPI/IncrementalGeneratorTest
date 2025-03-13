namespace TestProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var testClass = new TestClass { Id = 1, Name = "TestClass" };
            Console.WriteLine(testClass);

            Console.ReadKey();
        }
    }


    /// <summary>
    /// 作者:   Harling
    /// 时间:   2025/3/6 14:07:12
    /// 备注:   此文件通过PIToolKit模板创建
    /// </summary>
    /// <remarks></remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class AutoToStringAttribute : Attribute
    {
    }
    [AutoToString]
    internal partial class TestClass
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public int Id;
        /// <summary>
        /// 对象名称
        /// </summary>
        public string Name;
    }
}
