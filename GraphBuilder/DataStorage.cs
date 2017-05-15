using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace GraphBuilder
{

    [XmlRoot("Dictionary")]// для какова класса пишем сереализацыю/десереализацыю
    public class XmlSerializableDictionary<TKey, TValue>//зоздаем класс-наследник Dictionary с данным интерфейсом
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()//зарезервированный метод. должен возвращать null 
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)//при десериализации передаем сюда XmlReader
        {                                               // как его передовать и тд. не ясно =(

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }


    [Serializable]
    public class DataStorage
    {
        public DataStorage()
        {
        }
        List<UIElement> drawAreaChildrens = new List<UIElement>();

        XmlSerializableDictionary<Border, List<Line>> bindingVertexWithLinesStart = new XmlSerializableDictionary<Border, List<Line>>();
        XmlSerializableDictionary<Border, List<Line>> bindingVertexWithLinesEnd = new XmlSerializableDictionary<Border, List<Line>>();

        List<XmlSerializableVertexBag> bindingVertexWithAlgoVertex = new List<XmlSerializableVertexBag>();
        List<XmlSerializableEdgeBag> bindingBridgeWithAlgoEdge = new List<XmlSerializableEdgeBag>();
        List<Vertex> vertexList = new List<Vertex>();
        List<Edge> edgeList = new List<Edge>();
        List<Line> bridgesList = new List<Line>();

        [XmlIgnore]
        public XmlSerializableDictionary<Border, List<Line>> BindingVertexWithLinesStart
        {
            get
            {
                return bindingVertexWithLinesStart;
            }

            set
            {
                bindingVertexWithLinesStart = value;
            }
        }
        [XmlIgnore]
        public XmlSerializableDictionary<Border, List<Line>> BindingVertexWithLinesEnd
        {
            get
            {
                return bindingVertexWithLinesEnd;
            }

            set
            {
                bindingVertexWithLinesEnd = value;
            }
        }

        public List<XmlSerializableVertexBag> BindingVertexWithAlgoVertex
        {
            get
            {
                return bindingVertexWithAlgoVertex;
            }

            set
            {
                bindingVertexWithAlgoVertex = value;
            }
        }

        public List<XmlSerializableEdgeBag> BindingBridgeWithAlgoEdge
        {
            get
            {
                return bindingBridgeWithAlgoEdge;
            }

            set
            {
                bindingBridgeWithAlgoEdge = value;
            }
        }

        public List<Vertex> VertexList
        {
            get
            {
                return vertexList;
            }

            set
            {
                vertexList = value;
            }
        }
        public List<Edge> EdgeList
        {
            get
            {
                return edgeList;
            }

            set
            {
                edgeList = value;
            }
        }
        [XmlIgnore]
        public List<Line> BridgesList
        {
            get
            {
                return bridgesList;
            }

            set
            {
                bridgesList = value;
            }
        }
        [XmlIgnore]
        
        public List<UIElement> DrawAreaChildrens
        {
            get
            {
                return drawAreaChildrens;
            }

            set
            {
                drawAreaChildrens = value;
            }
        }
    }
}