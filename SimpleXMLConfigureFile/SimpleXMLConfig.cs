using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace SimpleXMLConfigureFileHelper
{
    /// <summary>
    /// Simple XML configure class
    /// </summary>
    public class SimpleXMLConfig
    {

        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="xmlDoc">xml文档</param>
        /// <param name="parentNode">父节点</param>
        /// <param name="name">节点名</param>
        /// <param name="value">节点值</param>
        private static XmlNode CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
            return node;
        }

        /// <summary>
        /// Convert calss to XML
        /// </summary>
        /// <param name="obj">class to convert</param>
        /// <param name="type">the type of obj</param>
        /// <param name="xmlDoc">XML document</param>
        /// <param name="Ele">Same level of the obj xml element</param>
        private static void WritePropertys(Object obj, Type type, XmlDocument xmlDoc, XmlElement Ele)
        {
            //get obj's properties
            var porityinfos = type.GetProperties();
            foreach (PropertyInfo item in porityinfos)
            {
                switch (item.PropertyType.Name)
                {
                    case "Byte":
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "Double":
                    case "Single":
                    case "String":
                    case "Boolean":
                        //obj is null, create a new one
                        if (obj == null)
                        {
                            obj = Activator.CreateInstance(type);
                        }
                        //read this obj's property
                        object itvalue = item.GetValue(obj);
                        if (itvalue != null)
                            Ele.SetAttribute(item.Name, itvalue.ToString());
                        else
                            Ele.SetAttribute(item.Name, "");
                        break;
                    case "List`1":
                        XmlElement SubEleList = xmlDoc.CreateElement(item.Name);
                        Ele.AppendChild(SubEleList);
                        string SubEleListName = item.PropertyType.GenericTypeArguments[0].Name;
                        var objEle = item.GetValue(Activator.CreateInstance(type));
                        if (objEle != null)
                        {
                            Type eleType = objEle.GetType();
                            var info = eleType.GetMethod("ToArray");
                            switch (SubEleListName)
                            {
                                case "Byte":
                                    byte[] bytes = (byte[])info.Invoke(objEle, null);
                                    bytes.ToList().ForEach(x => CreateNode(xmlDoc, SubEleList, SubEleListName, x.ToString()));
                                    break;
                                case "Int16":
                                    Int16[] int16s = (Int16[])info.Invoke(objEle, null);
                                    int16s.ToList().ForEach(x => CreateNode(xmlDoc, SubEleList, SubEleListName, x.ToString()));
                                    break;
                                case "Int32":
                                    Int32[] int32s = (Int32[])info.Invoke(objEle, null);
                                    int32s.ToList().ForEach(x => CreateNode(xmlDoc, SubEleList, SubEleListName, x.ToString()));
                                    break;
                                case "Int64":
                                    Int64[] int64s = (Int64[])info.Invoke(objEle, null);
                                    int64s.ToList().ForEach(x => CreateNode(xmlDoc, SubEleList, SubEleListName, x.ToString()));
                                    break;
                                case "Double":
                                    double[] doubles = (double[])info.Invoke(objEle, null);
                                    doubles.ToList().ForEach(x => CreateNode(xmlDoc, SubEleList, SubEleListName, x.ToString()));
                                    break;
                                case "Single":
                                    float[] floats = (float[])info.Invoke(objEle, null);
                                    floats.ToList().ForEach(x => CreateNode(xmlDoc, SubEleList, SubEleListName, x.ToString()));
                                    break;
                                case "String":
                                    string[] strings = (string[])info.Invoke(objEle, null);
                                    strings.ToList().ForEach(x => CreateNode(xmlDoc, SubEleList, SubEleListName, x.ToString()));
                                    break;
                                case "Boolean":
                                    bool[] bools = (bool[])info.Invoke(objEle, null);
                                    bools.ToList().ForEach(x => CreateNode(xmlDoc, SubEleList, SubEleListName, x.ToString()));
                                    break;
                                default:
                                    object[] eleArray = (object[])info.Invoke(objEle, null);
                                    foreach (var item1 in eleArray)
                                    {
                                        Type itemType = item1.GetType();
                                        XmlNode node = CreateNode(xmlDoc, SubEleList, SubEleListName, "");
                                        WritePropertys(item1, itemType, xmlDoc, (XmlElement)node);
                                    }
                                    break;
                            }
                        }
                        break;
                    default:
                        XmlElement SubEle = xmlDoc.CreateElement(item.Name);
                        Ele.AppendChild(SubEle);
                        WritePropertys(item.GetValue(obj), item.PropertyType, xmlDoc, SubEle);
                        break;
                }

            }
        }

        /// <summary>
        /// Read propertys from file
        /// </summary>
        /// <param name="obj">data will write to this obj</param>
        /// <param name="node">data will read from this node</param>
        private static void ReadPropertys(object obj, XmlNode node)
        {
            Type type = obj.GetType();
            var pp = type.GetProperties();

            foreach (XmlAttribute attr in node.Attributes)
            {
                SetValueToProperty(obj, pp.Where(x => x.Name == attr.Name).ToList()[0], attr.Value);
            }

            foreach (XmlNode subNode in node.ChildNodes)
            {
                PropertyInfo classProperty = pp.Where(x => x.Name == subNode.Name).ToList()[0];
                if (classProperty.GetValue(obj) == null)
                {
                    classProperty.SetValue(obj, Activator.CreateInstance(classProperty.PropertyType));
                }

                if (classProperty.PropertyType.Name == "List`1")
                {
                    MethodInfo method = classProperty.PropertyType.GetMethod("Clear");
                    PropertyInfo listProperty = pp.Where(x => x.Name == classProperty.Name).ToList()[0];
                    method.Invoke(listProperty.GetValue(obj), null);
                    foreach (XmlNode listNode in subNode.ChildNodes)
                    {
                        method = classProperty.PropertyType.GetMethod("Add");
                        method.Invoke(listProperty.GetValue(obj), new object[] { ConvertValue(listNode.InnerText, listNode.Name, listNode, classProperty.PropertyType.GetGenericArguments()[0]) });
                    }
                }
                else
                    ReadPropertys(classProperty.GetValue(obj), subNode);
            }

        }

        /// <summary>
        /// Convert data, if convert object is a class, node and objType should't be null
        /// </summary>
        /// <param name="value">origion value</param>
        /// <param name="type">target value type</param>
        /// <param name="node">will convert from this</param>
        /// <param name="objType">what's type should convert from node</param>
        /// <returns></returns>
        private static object ConvertValue(string value, string type, XmlNode node = null, Type objType = null)
        {
            object rvalue = null;
            switch (type)
            {
                case "Byte":
                    byte bytes = Convert.ToByte(value);
                    rvalue = bytes;
                    break;
                case "Int16":
                    Int16 int16s = Convert.ToInt16(value);
                    rvalue = int16s;
                    break;
                case "Int32":
                    Int32 int32s = Convert.ToInt32(value);
                    rvalue = int32s;
                    break;
                case "Int64":
                    Int64 int64s = Convert.ToInt64(value);
                    rvalue = int64s;
                    break;
                case "Double":
                    double doubles = Convert.ToDouble(value);
                    rvalue = doubles;
                    break;
                case "Single":
                    float floats = Convert.ToSingle(value);
                    rvalue = floats;
                    break;
                case "String":
                    string strings = (string)value;
                    rvalue = strings;
                    break;
                case "Boolean":
                    bool bools = Convert.ToBoolean(value);
                    rvalue = bools;
                    break;
                default:
                    object tobj = Activator.CreateInstance(objType);
                    ReadPropertys(tobj, node);
                    rvalue = tobj;
                    break;
            }
            return rvalue;
        }

        /// <summary>
        /// File version
        /// </summary>
        public static string FileVersion { get; set; }

        /// <summary>
        /// Read or create default configure file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FilePath">File Path</param>
        /// <param name="TargetVersion">Target version</param>
        /// <returns>Configure class</returns>
        /// <exception cref="Exception"></exception>
        public static T ReadConfigFile<T>(string FilePath, string TargetVersion = "1.0") where T : new()
        {
            XmlDocument xmlDoc = new XmlDocument();

            T ans = new T();
            if (!File.Exists(FilePath))
            {
                FileVersion = TargetVersion;
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                xmlDoc.AppendChild(node);
                XmlNode root = xmlDoc.CreateElement("root");
                xmlDoc.AppendChild(root);

                XmlElement ProgramSettings = xmlDoc.CreateElement("ProgramSettings");
                ProgramSettings.SetAttribute("ProgramVersion", FileVersion);
                root.AppendChild(ProgramSettings);

                XmlElement ConfigEle = xmlDoc.CreateElement("Config");
                root.AppendChild(ConfigEle);
                WritePropertys(ans, typeof(T), xmlDoc, ConfigEle);
                xmlDoc.Save(FilePath);
                return ans;
            }

            xmlDoc.Load(@".\Settings.xml");
            XmlNode rootNode = xmlDoc.SelectSingleNode("root");
            XmlNodeList firstLevelNodeList = rootNode.ChildNodes;

            foreach (XmlNode node in firstLevelNodeList)
            {
                if (node.LocalName == "ProgramSettings")
                {
                    FileVersion = node.Attributes["ProgramVersion"].Value;
                    if (FileVersion != TargetVersion)
                    {
                        throw new Exception($"Wrong file version, target version:{TargetVersion}, file version:{FileVersion}");
                    }
                }
                if (node.LocalName == "Config")
                {
                    ReadPropertys(ans, node);
                }
            }
            return ans;
        }

        /// <summary>
        /// Save configure file, will replace old file
        /// </summary>
        /// <param name="ConfigObj"></param>
        /// <param name="FilePath"></param>
        /// <param name="FileVersion"></param>
        public static void SaveConfigFile(object ConfigObj, string FilePath, string FileVersion)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            XmlNode root = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(root);

            XmlElement ProgramSettings = xmlDoc.CreateElement("ProgramSettings");
            ProgramSettings.SetAttribute("ProgramVersion", FileVersion);
            root.AppendChild(ProgramSettings);

            XmlElement ConfigEle = xmlDoc.CreateElement("Config");
            root.AppendChild(ConfigEle);
            WritePropertys(ConfigObj, ConfigObj.GetType(), xmlDoc, ConfigEle);
            xmlDoc.Save(FilePath);
            SimpleXMLConfig.FileVersion = FileVersion;
        }

        private static void SetValueToProperty(object obj, PropertyInfo property, object value)
        {
            switch (property.PropertyType.Name)
            {
                case "Byte":
                    byte bytes = Convert.ToByte(value);
                    property.SetValue(obj, bytes);
                    break;
                case "Int16":
                    Int16 int16s = Convert.ToInt16(value);
                    property.SetValue(obj, int16s);
                    break;
                case "Int32":
                    Int32 int32s = Convert.ToInt32(value);
                    property.SetValue(obj, int32s);
                    break;
                case "Int64":
                    Int64 int64s = Convert.ToInt64(value);
                    property.SetValue(obj, int64s);
                    break;
                case "Double":
                    double doubles = Convert.ToDouble(value);
                    property.SetValue(obj, doubles);
                    break;
                case "Single":
                    float floats = Convert.ToSingle(value);
                    property.SetValue(obj, floats);
                    break;
                case "String":
                    string strings = (string)value;
                    property.SetValue(obj, strings);
                    break;
                case "Boolean":
                    bool bools = Convert.ToBoolean(value);
                    property.SetValue(obj, bools);
                    break;
            }
        }

    }
}
