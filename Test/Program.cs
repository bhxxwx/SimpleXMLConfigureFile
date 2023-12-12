using System;
using System.Collections.Generic;


namespace Test
{
    public class Sub1
    {
        public int S1Value { get; set; }
    }

    public class Sub2
    {
        public int S2Value { get; set; }
        public List<string> ints { get; set; } = new List<string>() { "sd1", "sd2" };
    }
    public class TestClass
    {

        public int vint { get; set; } = 1;
        public Int64 vint64 { get; set; } = 2;
        public Int32 vint32 { get; set; } = 3;
        public Int16 vint16 { get; set; }
        public byte vbyte { get; set; } = 255;
        public bool vb { get; set; } = false;

        public double vdouble { get; set; }
        public float vfloat { get; set; } = 2.1F;
        public string TestString { get; set; }
        public Sub1 Sub1 { get; set; }
        public Sub2 Sub2 { get; set; }

        public List<int> Ints2 { get; set; } = new List<int>() { 12, 56, 54 };
        public List<Sub2> sub2s { get; set; }
        public List<Sub2> sub3s { get; set; } = new List<Sub2>() { new Sub2() { S2Value = 1 }, new Sub2() { S2Value = 2 } };
    }
    internal class Program
    {
        static void Main(string[] args)
        {

            var tans = SimpleXMLConfigureFileHelper.SimpleXMLConfig.ReadConfigFile<TestClass>(@"./Settings.xml");
            tans.TestString = "123456";
            SimpleXMLConfigureFileHelper.SimpleXMLConfig.SaveConfigFile(tans, @"./Settings.xml", "2.0");

            Console.ReadLine();
        }
    }
}
