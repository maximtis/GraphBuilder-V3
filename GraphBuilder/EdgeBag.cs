using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace GraphBuilder
{

    [XmlRoot("EdgeBag")]

    public class XmlSerializableEdgeBag//зоздаем класс-наследник с данным интерфейсом
        : EdgeBag, IXmlSerializable
    {
        public XmlSerializableEdgeBag() : base() { }
        public XmlSerializableEdgeBag(Line graphic,Edge data) : base(graphic, data) { }
        public System.Xml.Schema.XmlSchema GetSchema()//зарезервированный метод. должен возвращать null 
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)//при десериализации передаем сюда XmlReader
        {                                               // как его передовать и тд. не ясно =(

            XmlSerializer keySerializer = new XmlSerializer(typeof(Line));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(Edge));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                Line key = (Line)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                Edge value = (Edge)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.DataEdge = value;
                this.GraphicEdge = key;
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(Border));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(Vertex));
            writer.WriteStartElement("item");
            writer.WriteStartElement("key");
            keySerializer.Serialize(writer, GraphicEdge);
            writer.WriteEndElement();
            writer.WriteStartElement("value");
            valueSerializer.Serialize(writer,DataEdge);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }


    [Serializable]
    public class EdgeBag
    {
        public EdgeBag(Line graphic, Edge data)
        {
            GraphicEdge = graphic;
            DataEdge = data;
        }
        public EdgeBag()
        {
            GraphicEdge = new Line();
            DataEdge = new Edge();
     }
        private static int BridgeZindexIterator = 1;
        public Line GraphicEdge { get; set; }
        public Edge DataEdge { get; set; }

        public Edge Edge
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public static XmlSerializableEdgeBag GenerateBridge(Point start, Point end)
        {
            Line l = new Line();
            Canvas.SetZIndex(l, BridgeZindexIterator);
            l.StrokeThickness = 3;
            l.Stroke = Brushes.Red;
            l.X1 = start.X;
            l.Y1 = start.Y;
            l.X2 = end.X;
            l.Y2 = end.Y;
            BridgeZindexIterator++;

            Edge e = new Edge(new Vertex(), new Vertex(), 0);
            return new XmlSerializableEdgeBag(l, e);
        }
    }
}
