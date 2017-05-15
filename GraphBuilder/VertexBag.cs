using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GraphBuilder
{

    [XmlRoot("VertexBag")]
    [Serializable]
    public class XmlSerializableVertexBag//зоздаем класс-наследник Dictionary с данным интерфейсом
    : VertexBag, IXmlSerializable
    {
        public XmlSerializableVertexBag() : base() { }
        public XmlSerializableVertexBag(Border graphic, Vertex data) : base(graphic, data) { }
        public System.Xml.Schema.XmlSchema GetSchema()//зарезервированный метод. должен возвращать null 
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)//при десериализации передаем сюда XmlReader
        {                                               // как его передовать и тд. не ясно =(

            XmlSerializer keySerializer = new XmlSerializer(typeof(Border));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(Vertex));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                Border key = (Border)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                Vertex value = (Vertex)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.DataVertex = value;
                this.GraphicVertex = key;
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(string));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(Vertex));
            writer.WriteStartElement("item");
            writer.WriteStartElement("key");
            keySerializer.Serialize(writer, GraphicVertex.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("value");
            valueSerializer.Serialize(writer, DataVertex);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }

    [Serializable]
    public class VertexBag
    {
        //  Структура для удобной привязки графики к данным
        public VertexBag(Border graphic, Vertex data)
        {
            GraphicVertex = graphic;
            DataVertex = data;
        }
        public VertexBag()
        {
            GraphicVertex = new Border();
            DataVertex = new Vertex();
        }

        private static int VertexIterator = 0;
        private static string alphabet = "ABCDEFGHIJKLMNOPRSTUVWXYZ";
        public Border GraphicVertex { get; set; }
        public Vertex DataVertex { get; set; }

        public Vertex Vertex
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public static XmlSerializableVertexBag GenereteVertex()
        {

            if (VertexIterator == alphabet.Length)
                throw new ArgumentOutOfRangeException("alphabet.Length", "The Maximum number of vertexes is reached");

            // Генерируем графическое представление для вершины
            Border b = new Border();
            b.Width = 40;
            b.Height = 40;
            b.CornerRadius = new CornerRadius(50);
            b.Background = Brushes.BlueViolet;
            b.BorderBrush = Brushes.Black;
            b.BorderThickness = new Thickness(1);
            Label l = new Label();
            l.Width = 22;
            l.Height = 22;
            l.Margin = new Thickness(8);
            l.Content = alphabet[VertexIterator];
            l.Foreground = Brushes.White;
            b.Child = l;
            Canvas.SetZIndex(b,300);
            VertexIterator++;
            // Генерирум данные вершины для алгоритма
            Vertex v = new Vertex(99999, false, l.Content.ToString());

            // Создаем пакет с графическим представлением вершины и данными для алгоритма
            XmlSerializableVertexBag Bag = new XmlSerializableVertexBag(b, v);
            return Bag;
        }
        public static XmlSerializableVertexBag GenereteVertex(string Name)
        {
            // Генерируем графическое представление для вершины
            Border b = new Border();
            b.Width = 40;
            b.Height = 40;
            b.CornerRadius = new CornerRadius(50);
            b.Background = Brushes.BlueViolet;
            b.BorderBrush = Brushes.Black;
            b.BorderThickness = new Thickness(1);
            Label l = new Label();
            l.Width = 22;
            l.Height = 22;
            l.Margin = new Thickness(8);
            l.Content = Name;
            l.Foreground = Brushes.White;
            b.Child = l;
            Canvas.SetZIndex(b, VertexIterator + 300);
            // Генерирум данные вершины для алгоритма
            Vertex v = new Vertex(99999, false, l.Content.ToString());

            // Создаем пакет с графическим представлением вершины и данными для алгоритма
            XmlSerializableVertexBag Bag = new XmlSerializableVertexBag(b, v);
            return Bag;
        }
    }
}
