using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tracers;

namespace TRSerializer
{
    public class XmlSerializer : ISerializer
    {
        private void Save(XmlDocument document, XmlElement parent, IEnumerable<IMethodTrace> methods)
        {
            foreach (IMethodTrace method in methods)
            {
                var element = document.CreateElement("method");

                var classAttr = document.CreateAttribute("class");
                var classText = document.CreateTextNode(method.ClassName);
                classAttr.AppendChild(classText);
                element.Attributes.Append(classAttr);

                var nameAttr = document.CreateAttribute("name");
                var nameText = document.CreateTextNode(method.MethodName);
                nameAttr.AppendChild(nameText);
                element.Attributes.Append(nameAttr);

                var deltaTimeAttr = document.CreateAttribute("time");
                var deltaTimeText = document.CreateTextNode(method.DeltaTimeString);
                deltaTimeAttr.AppendChild(deltaTimeText);
                element.Attributes.Append(deltaTimeAttr);

                parent.AppendChild(element);

                Save(document, element, method.Methods);
            }
        }
        public void Save(Stream output, IEnumerable<IThreadTrace> traceResult)
        {
            using var writer = XmlWriter.Create(output);

            var document = new XmlDocument();
            var root = document.DocumentElement;
            if (root == null)
            {
                root = document.CreateElement("TraceResult");
                document.AppendChild(root);
            }


            foreach (IThreadTrace node in traceResult)
            {
                var threadElement = document.CreateElement("thread");

                var nameAttr = document.CreateAttribute("name");
                var nameText = document.CreateTextNode(node.ThreadName);
                nameAttr.AppendChild(nameText);
                threadElement.Attributes.Append(nameAttr);


                var deltaTimeAttr = document.CreateAttribute("time");
                var deltaTimeText = document.CreateTextNode(node.DeltaTimeString);
                deltaTimeAttr.AppendChild(deltaTimeText);
                threadElement.Attributes.Append(deltaTimeAttr);

                root.AppendChild(threadElement);

                Save(document, threadElement, node.Methods);
            }

            document.WriteTo(writer);
        }
    }
}
