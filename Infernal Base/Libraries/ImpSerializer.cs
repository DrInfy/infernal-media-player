using System;
using System.IO;
using System.Xml.Serialization;

namespace Base.Libraries
{
    public static class ImpSerializer
    {
        /// <summary>
        /// attempts to write the file in xml format
        /// </summary>
        /// <param name="filename">path to file to write</param>
        /// <param name="instance">object to trite</param>
        /// <param name="instanceType">type of object to write</param>
        public static void TryWriteFileXml(string filename, object instance, Type instanceType)
        {
            var serializer = new XmlSerializer(instanceType);
            using (var outputStream = new FileStream(filename, FileMode.Create))
                serializer.Serialize(outputStream, instance);
        }


        public static object ReadFile(string filename, Type instanceType)
        {
            var serializer = new XmlSerializer(instanceType);

            using (var outputStream = new FileStream(filename, FileMode.Open))
                return serializer.Deserialize(outputStream);
        }
    }
}
