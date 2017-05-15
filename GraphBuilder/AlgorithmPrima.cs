using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphBuilder
{
    class AlgorithmPrima
    {
        public AlgorithmPrima(List<Vertex> V, List<Edge> E,Vertex S)
        {
            Vertex[] TempVertexes = new Vertex[V.Count];
            Edge[] TempEdges = new Edge[E.Count];

            V.CopyTo(TempVertexes);
            E.CopyTo(TempEdges);
            VertexList = TempVertexes.ToList();
            EdgeList = TempEdges.ToList();
            Start = VertexList[V.IndexOf(S)];
        }

        List<Vertex> VertexList;
        List<Edge> EdgeList;
        Vertex Start;
        public List<Edge> MSTRezult = new List<Edge>();

        public void algorithmByPrim()
        {
            //результирующийсписок
            List<Edge> MST = new List<Edge>();
            //неиспользованные ребра
            //VertexList
            //использованные вершины
            List<Vertex> usedV = new List<Vertex>();
            //неиспользованные вершины
            //EdgeList
            //выбираем начальную вершину

            usedV.Add(VertexList[0]);
            VertexList.Remove(usedV[0]);


            while (VertexList.Count > 0)
            {
                //int minE = -1; //номер наименьшее ребро
                Edge minE = new Edge(new Vertex(), new Vertex(), -10);

                //поиск наименьшего ребра
                for (int i = 0; i < EdgeList.Count; i++)
                {
                    if ((usedV.IndexOf(EdgeList[i].FirstPoint) != -1) && (VertexList.IndexOf(EdgeList[i].SecondPoint) != -1) ||
                        (usedV.IndexOf(EdgeList[i].SecondPoint) != -1) && (VertexList.IndexOf(EdgeList[i].FirstPoint) != -1))
                    {
                        if (minE.Weight >= 0)
                        {
                            if (EdgeList[i].Weight < minE.Weight)
                                minE = EdgeList[i];
                        }
                        else
                            minE = EdgeList[i];
                    }
                }
                //заносим новую вершину в список использованных и удаляем ее из списка неиспользованных
                if (usedV.IndexOf(minE.FirstPoint) != -1)
                {
                    usedV.Add(minE.SecondPoint);
                    VertexList.Remove(minE.SecondPoint);
                }
                else
                {
                    usedV.Add(minE.FirstPoint);
                    VertexList.Remove(minE.FirstPoint);
                }
                //заносим новое ребро в дерево и удаляем его из списка неиспользованных
                MST.Add(minE);
                EdgeList.Remove(minE);
            }
            MSTRezult = MST;
        }
    }
}
