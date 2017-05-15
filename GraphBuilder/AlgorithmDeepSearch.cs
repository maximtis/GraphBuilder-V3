using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphBuilder
{
    class DeepSearchAlgorithm
    {
        public List<Edge> ResultListEdges = new List<Edge>();
        List<List<int>> Edge_List = new List<List<int>>();
        public string Result = string.Empty;
        List<Vertex> vertexList { get; set; }
        List<Edge> edgeList { get;  set; }

        public DeepSearchAlgorithm(List<Vertex> vertexesOfgrath, List<Edge> edgeListOfgrath)
        {
            vertexList = vertexesOfgrath;
            edgeList = edgeListOfgrath;
            Create_EdgeList();
            Create_WatchedList();
        }
        public void Create_WatchedList()
        {
            foreach (Vertex x in vertexList)
                x.IsChecked = false;
        }
        public void Create_EdgeList()
        {
            for (int i = 0; i < vertexList.Count; i++)
            {
                Edge_List.Add(new List<int>());
                for (int j = 0; j < vertexList.Count; j++)
                    Edge_List[i].Add(0);
            }

            for (int i = 0; i < vertexList.Count; i++)
                foreach (Edge x in edgeList)
                    if (vertexList[i] == x.FirstPoint)
                        Edge_List[i][vertexList.IndexOf(x.SecondPoint)] = 1;
            if(MainWindow.IsOrientedGraph)
            for (int i = 0; i < vertexList.Count; i++)
                foreach (Edge x in edgeList)
                    if (vertexList[i] == x.SecondPoint)
                        Edge_List[i][vertexList.IndexOf(x.FirstPoint)] = 1;

        }
        public void Show_DeepSearchAlgorithm()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            for (int i = 0; i < Edge_List.Count; i++)
            {
                Console.Write(" \t| ");
                for (int j = 0; j < Edge_List[i].Count; j++)
                {
                    Console.Write(Edge_List[i][j] + " ");
                }
                Console.Write( "|  "+Environment.NewLine);
            }
        }
        void DFS(int index) // Поиск пути к вершине
	    {

        Result += "\t Move Path: "+Environment.NewLine;
        for (int i = 0; i < Edge_List[index].Count; i++)
		{
                vertexList[index].IsChecked = true;

                if (!vertexList[i].IsChecked && Edge_List[index][i] != 0)
			    {
                    Result += "\t From " + (index + 1) + " to " + (i + 1)+Environment.NewLine;
                    ResultListEdges.Add(new Edge(vertexList[index], vertexList[i], 0));
                    DFS(i);
			    }
		    }
        }
        bool isNotCompleate()
        {
            for (int i = 0; i < vertexList.Count; i++)
            {
                if (!vertexList[i].IsChecked)
                    return true;
            }
            return false;
        }

        int IndexNotComplete()
        {
            for(int i=0;i< vertexList.Count; i++)  
            {
                if (!vertexList[i].IsChecked)
                    return i;
            }
            return 0;
        }
        public string Begin_Search()
        {
            int chooseEdge = 0;
            chooseEdge = 1;
            DFS(chooseEdge - 1);

            while (isNotCompleate())
            {
                Result += "Новое остовое дерево: \n\n";
                DFS(IndexNotComplete());
            }
            return Result;
        }
    }
}
