namespace NugetTest.NET
{
    internal class Program
    {

        public class TestClass
        {
            public int i1
            {
                get; set;
            }
            public string ts { get; set; } = "dfdf";
        }

        static void Main(string[] args)
        {
            SimpleXMLConfigureFileHelper.SimpleXMLConfig.ReadConfigFile<TestClass>(@"./setting.xml");
            Console.WriteLine("Hello, World!");
        }
    }
}