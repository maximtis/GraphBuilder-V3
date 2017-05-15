
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// Реализация алгоритма Дейкстры. Содержит матрицу смежности в виде массивов вершин и ребер
/// </summary>
namespace GraphBuilder
{
    class DekstraAlgorim
    {

        public List<Vertex> vertexList { get; set; }
        public List<Edge> edgeList { get; set; }
        public Vertex beginVertex { get; private set; }

        internal DekstraException DekstraException
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public DekstraAlgorim(List<Vertex> vertexesOfgrath, List<Edge> edgeListOfgrath)
        { 
            vertexList = vertexesOfgrath;
            foreach (Vertex x in vertexList)
                x.IsChecked = false;
            edgeList = edgeListOfgrath;
        }
        /// <summary>
        /// Запуск алгоритма расчета
        /// </summary>
        /// <param name="beginvetex"></param>
        public void AlgoritmRun(Vertex beginvetex)
        {
            if (vertexList.Count() == 0 || edgeList.Count() == 0)
                throw new DekstraException("Массив вершин или ребер не задан!");
            else
            {
                beginVertex = beginvetex;
                OneStep(beginvetex);
                foreach (Vertex point in vertexList)
                {
                    Vertex anotherP = NearestUncheckedVertex();
                    if (anotherP != null)
                    {
                        OneStep(anotherP);
                    }
                    else
                    {
                        break;
                    }

                }
            }

        }
        /// <summary>
        /// Метод, делающий один шаг алгоритма. Принимает на вход вершину
        /// </summary>
        /// <param name="beginpoint"></param>
        public void OneStep(Vertex beginpoint)
        {
            foreach (Vertex nextp in Neighbors(beginpoint))
            {
                if (nextp.IsChecked == false)//не отмечена
                {
                    float newmetka = beginpoint.PathLength + SelectEdge(nextp, beginpoint).Weight;
                    if (nextp.PathLength > newmetka)
                    {
                        nextp.PathLength = newmetka;
                        nextp.PrevVertex = beginpoint;
                    }
                }
            }
            beginpoint.IsChecked = true;//вычеркиваем
        }
        /// <summary>
        /// Поиск соседей для вершины. Для неориентированного графа ищутся все соседи.
        /// </summary>
        /// <param name="currpoint"></param>
        private IEnumerable<Vertex> Neighbors(Vertex currpoint)
        {
            IEnumerable<Vertex> firstVertex = from ff in edgeList
                                              where ff.FirstPoint == currpoint
                                              select ff.SecondPoint;

            IEnumerable<Vertex> secondVertex = from sp in edgeList
                                               where sp.SecondPoint == currpoint
                                               select sp.FirstPoint;

            IEnumerable<Vertex> totalVertex = firstVertex.Concat(secondVertex);
            return totalVertex;
        }
        /// <summary>
        /// Получаем ребро, соединяющее 2 входные точки
        /// </summary>
        private Edge SelectEdge(Vertex sour, Vertex dest)
        {//ищем ребро по 2 точкам
            IEnumerable<Edge> edge = from e in edgeList
                                     where (e.FirstPoint == sour & e.SecondPoint == dest) ||
                                           (e.FirstPoint == dest & e.SecondPoint == sour)
                                     select e;

            if (edge.Count() > 1 || edge.Count() == 0)
                throw new DekstraException("Не найдено ребро между соседями!");
            else
                return edge.First();
        }
        /// <summary>
        /// Получаем очередную неотмеченную вершину, "ближайшую" к заданной.
        /// </summary>
        /// <returns></returns>
        private Vertex NearestUncheckedVertex()
        {
            IEnumerable<Vertex> UncheckedVertex = from Vertex in vertexList
                                                  where Vertex.IsChecked == false
                                                  select Vertex;
            if (UncheckedVertex.Count() == 0)
                return null;
            else
            {
                float minPathLength = UncheckedVertex.First().PathLength;
                Vertex minPathLengthVertex = UncheckedVertex.First();
                foreach (Vertex p in UncheckedVertex)
                {
                    if (p.PathLength < minPathLength)
                    {
                        minPathLength = p.PathLength;
                        minPathLengthVertex = p;
                    }
                }
                return minPathLengthVertex;
            }
        }
        public List<Vertex> MinPath(Vertex end)
        {
            List<Vertex> listOfpoints = new List<Vertex>();
            Vertex tempp = new Vertex();
            tempp = end;
            while (tempp != beginVertex)
            {
                listOfpoints.Add(tempp);
                tempp = tempp.PrevVertex;
            }

            return listOfpoints;
        }
    }

    /// <summary>
    /// Класс, реализующий ребро
    /// </summary>

    [Serializable]
    public class Edge
    {
        public Vertex FirstPoint { get; set; }
        public Vertex SecondPoint { get; set; }
        public float Weight { get; set; }

        public Edge()
        {
            FirstPoint = new Vertex();
            SecondPoint = new Vertex();
            Weight = 9999;
        }
        public Edge(Vertex first, Vertex second, float valueOfWeight)
        {
            FirstPoint = first;
            SecondPoint = second;
            Weight = valueOfWeight;
        }

    }
    /// <summary>
    /// Класс, реализующий вершину графа
    /// </summary>
    /// 

    [Serializable]
    public class Vertex
    {
        public float PathLength { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public Vertex PrevVertex { get; set; }
        public Vertex Clone()
        {
            return new Vertex(PathLength, IsChecked, Name);
        }
        public Vertex(float value, bool ischecked)
        {
            PathLength = value;
            IsChecked = ischecked;
            PrevVertex = new Vertex();
        }
        public Vertex(float value, bool ischecked, string name)
        {
            PathLength = value;
            IsChecked = ischecked;
            Name = name;
            PrevVertex = new Vertex();
        }
        public Vertex()
        {
        }
    }
    // <summary>
    /// для печати графа
    /// </summary>
    static class PrintGrath
    {
        internal static DekstraAlgorim DekstraAlgorim
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public static List<string> PrintAllPoints(DekstraAlgorim da)
        {
            List<string> retListOfPoints = new List<string>();
            foreach (Vertex p in da.vertexList)
            {
                retListOfPoints.Add(string.Format("point name={0}, point value={1}, predok={2}", p.Name, p.PathLength, p.PrevVertex.Name ?? "нет предка"));
            }
            return retListOfPoints;
        }
        public static List<string> PrintAllMinPaths(DekstraAlgorim da)
        {
            List<string> retListOfPointsAndPaths = new List<string>();
            foreach (Vertex p in da.vertexList)
                if (p != da.beginVertex)
                {
                    string s = string.Empty;
                    foreach (Vertex p1 in da.MinPath(p))
                        s += string.Format("{0} ", p1.Name);
                    retListOfPointsAndPaths.Add(string.Format("Point ={0}: \n Minimal Path to {1} = {2}", p.Name, da.beginVertex.Name, s));
                }
            return retListOfPointsAndPaths;
        }
    }

    class DekstraException : ApplicationException
    {
        public DekstraException(string message) : base(message)
        {
        }
    }
}