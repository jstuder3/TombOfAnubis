using Microsoft.Xna.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Collections;
using QuikGraph;
using System;
using System.Collections.Generic;


namespace TombOfAnubis
{
    public class LevelGraph
    {
        private static readonly int EdgeCostFloorFloor = 10;
        private static readonly int EdgeCostRoad = 0;
        private static readonly int EdgeCostFloorEmpty = 1;
        private static readonly int EdgeCostEmptyEmpty = 1;
        private static readonly int EdgeCostDoor = 1;


        private int[,] level;
        private Point levelDim;

        private HashSet<Point> floors;
        private HashSet<Point> walls;
        private HashSet<Point> emptys;
        // Empty -> Edge(floor, floor)
        private List<Dictionary<Point, Edge<Point>>> doors;

        // Graph to complete the paths between the building blocks
        //private BidirectionalGraph<Point, Edge<Point>> Graph;
        private UndirectedBidirectionalGraph<Point, Edge<Point>> Graph;

        private Dictionary<Edge<Point>, double> EdgeCost;

        public LevelGraph(int[,] level)
        { 
            this.level = level;
            levelDim = new Point(level.GetLength(0), level.GetLength(1));
            floors = new HashSet<Point>();
            walls = new HashSet<Point>();
            emptys = new HashSet<Point>();
            EdgeCost = new Dictionary<Edge<Point>, double>();

        }
        public bool ConnectLevelBlocks()
        {
            FillFloorWallEmptyLists();
            FillGraph();
            if(!ConnectFloors()) return false;
            FillRemainingeEmptiesWithWalls();
            return true;
        }

        private void FillFloorWallEmptyLists()
        {
            for (int i = 0; i < levelDim.X; i++)
            {
                for (int j = 0; j < levelDim.Y; j++)
                {
                    if (level[i, j] == LevelBlock.FloorValue)
                    {
                        floors.Add(new Point(i, j));
                    }
                    else if (level[i, j] == LevelBlock.WallValue)
                    {
                        walls.Add(new Point(i, j));
                    }
                    else if(level[i, j] == LevelBlock.EmptyValue)
                    {
                        emptys.Add(new Point(i, j));
                    }
                }
            }
        }

        private void FillGraph()
        {
            BidirectionalGraph<Point, Edge<Point>> directedGraph = new BidirectionalGraph<Point, Edge<Point>>();

            foreach (Point floor in floors)
            {
                directedGraph.AddVertex(floor);
            }
            foreach (Point empty in emptys)
            {
                directedGraph.AddVertex(empty);
            }
            foreach (Point floor in floors)
            {
                List<Point> neighbours = new List<Point>()
                {
                    floor + new Point(1, 0),
                    floor + new Point(-1, 0),
                    floor + new Point(0, 1),
                    floor + new Point(0, -1)
                };
                foreach (Point neighbour in neighbours)
                {
                    if (!ValidCoord(neighbour)) { continue; }
                    if (level[neighbour.X, neighbour.Y] == LevelBlock.FloorValue)
                    {
                        Edge<Point> edge = new Edge<Point>(floor, neighbour);
                        EdgeCost.Add(edge, EdgeCostFloorFloor);
                        directedGraph.AddEdge(edge);
                    }
                    else if (level[neighbour.X, neighbour.Y] == LevelBlock.EmptyValue)
                    {
                        Edge<Point> edge = new Edge<Point>(floor, neighbour);
                        EdgeCost.Add(edge, EdgeCostFloorEmpty);
                        directedGraph.AddEdge(edge);
                    }
                }
            }
            foreach (Point empty in emptys)
            {
                List<Point> neighbours = new List<Point>()
                {
                    empty + new Point(1, 0),
                    empty + new Point(-1, 0),
                    empty + new Point(0, 1),
                    empty + new Point(0, -1)
                };
                foreach (Point neighbour in neighbours)
                {
                    if (!ValidCoord(neighbour)) { continue; }
                    if (level[neighbour.X, neighbour.Y] == LevelBlock.EmptyValue)
                    {
                        Edge<Point> edge = new Edge<Point>(empty, neighbour);
                        EdgeCost.Add(edge, EdgeCostEmptyEmpty);
                        directedGraph.AddEdge(edge);
                    }
                }
            }
            Graph = new UndirectedBidirectionalGraph<Point, Edge<Point>>(directedGraph);

        }

        private bool ConnectFloors()
        {

            foreach(Point source in floors)
            {
                foreach(Point target in floors)
                {
                    if (source.Equals(target)) { continue; }
                    var path = FindPath(source, target);
                    if(path == null) {
                        return false;
                    }
                    foreach(Edge<Point> edge in path)
                    {
                        Point v = edge.Target;
                        if (level[v.X, v.Y] == LevelBlock.EmptyValue)
                        {
                            level[v.X, v.Y] = LevelBlock.FloorValue;
                            EdgeCost[edge] = EdgeCostRoad;
                            emptys.Remove(v);
                        }
                    }
                }
            }
            return true;
        }

        private void FillRemainingeEmptiesWithWalls()
        {
            foreach (Point empty in emptys)
            {
                level[empty.X, empty.Y] = LevelBlock.WallValue;
            }
        }


        private  IEnumerable<Edge<Point>> FindPath(Point from, Point to)
        {
            Func<Edge<Point>, double> edgeCost = AlgorithmExtensions.GetIndexer(EdgeCost);
            // Positive or negative weights
            TryFunc<Point, System.Collections.Generic.IEnumerable<Edge<Point>>> tryGetPath = Graph.ShortestPathsDijkstra(edgeCost, from);

            IEnumerable<Edge<Point>> path;
            tryGetPath(to, out path);
            return path;
            //{
               
            //    Console.Write("Path found from {0} to {1}: {0}", from, to);
            //    foreach (var e in path) { Console.Write(" > {0}", e.Target); }
            //    Console.WriteLine();
            //}
            //else { Console.WriteLine("No path found from {0} to {1}."); }
        }

        private bool ValidCoord(Point coord)
        {
            return coord.X >= 0 && coord.Y >= 0 &&  coord.X < levelDim.X && coord.Y < levelDim.Y;
        }
    }
}
