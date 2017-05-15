using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;



namespace GraphBuilder
{
    delegate void StorageChangeEventHandler(StorageChangeEventArgs e);
    class StorageChangeEventArgs : EventArgs
    {
        public StorageChangeEventArgs(DataStorage data)
        {
            Data = data;
        }
        public DataStorage Data;
    }



    class FileController
    {
        public Stack<DataStorage> History { get; set; }

        public event EventHandler Undo;
        public FileController()
        {
            //Constructor
        }
        public FileController(DataStorage ds)
        {
            History = new Stack<DataStorage>();
            History.Push(ds);
        }

        public void SaveAs(string Path)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(DataStorage));
            using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate))
                formatter.Serialize(fs, DrawingController.Backup());
        }
        public DataStorage Load(string Path)
        {
            DataStorage data;
            XmlSerializer formatter = new XmlSerializer(typeof(DataStorage));
            using (FileStream fs = new FileStream("Graph.xml", FileMode.OpenOrCreate))
                data = (DataStorage)formatter.Deserialize(fs);
            return data;
        }
        public void UndoCommand()
        {
            if(History.Count>0)
            if (Undo != null)
                Undo(this, new StorageChangeEventArgs(History.Pop()));
        }
        public void DataStorageHistory_Holder(StorageChangeEventArgs e)
        {
            History.Push(e.Data);
        }
        public void ConnectToHistoryJournal(MainWindow window)
        {
            DrawingController.StorageChanged += DataStorageHistory_Holder;
            Undo += window.Undo_Holder;
        }
    }
}
