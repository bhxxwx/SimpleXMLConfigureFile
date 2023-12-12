# How to use
This library can help you easily convert a class to XML file (just property of this class), or read data from XML file.
Here are some functions:

```cs
/// <summary>
/// Read or create default configure file.
/// If read file not exists, create one by new T object, and this file version will be the param TargetVersion.
/// If read file exists, compare the file version and the param TargetVersion.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="FilePath">File Path</param>
/// <param name="TargetVersion">Target version</param>
/// <returns>Configure class</returns>
/// <exception cref="Exception"></exception>
public static T ReadConfigFile<T>(string FilePath, string TargetVersion = "1.0");

/// <summary>
/// Save configure file, will replace old file.
/// </summary>
/// <param name="ConfigObj"></param>
/// <param name="FilePath"></param>
/// <param name="FileVersion"></param>
public static void SaveConfigFile(object ConfigObj, string FilePath, string FileVersion);

/// <summary>
/// File version
/// </summary>
public static string? FileVersion { get; set; }
```


# Test bench
### CS
```cs
namespace Test.NET
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
            //read from file, is file not exists, create one by new T object
            var tans = SimpleXMLConfigureFileHelper.NET.SimpleXMLConfig.ReadConfigFile<TestClass>(@"./Settings.xml");
            tans.TestString = "123456";
            //write class to XML file, replace file if exists
            SimpleXMLConfigureFileHelper.NET.SimpleXMLConfig.SaveConfigFile(tans, @"./Settings.xml", "2.0");
            Console.ReadLine();
        }
    }
}
```
## Created XML configure file
```XML
<?xml version="1.0" encoding="utf-8"?>
<root>
  <ProgramSettings ProgramVersion="2.0" />
  <Config vint="1" vint64="2" vint32="3" vint16="0" vbyte="255" vb="False" vdouble="0" vfloat="2.1" TestString="123456">
    <Sub1 S1Value="0" />
    <Sub2 S2Value="0">
      <ints>
        <String>sd1</String>
        <String>sd2</String>
      </ints>
    </Sub2>
    <Ints2>
      <Int32>12</Int32>
      <Int32>56</Int32>
      <Int32>54</Int32>
    </Ints2>
    <sub2s />
    <sub3s>
      <Sub2 S2Value="1">
        <ints>
          <String>sd1</String>
          <String>sd2</String>
        </ints>
      </Sub2>
      <Sub2 S2Value="2">
        <ints>
          <String>sd1</String>
          <String>sd2</String>
        </ints>
      </Sub2>
    </sub3s>
  </Config>
</root>
```
