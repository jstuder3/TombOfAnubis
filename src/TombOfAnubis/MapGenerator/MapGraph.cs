using Microsoft.Xna.Framework;
using QuikGraph.Algorithms;
using QuikGraph;
using System; using System.Diagnostics;
using System.Collections.Generic;


namespace TombOfAnubis
{
    public class MapGraph
    {
        private static readonly int EdgeCostFloorFloor = 10;
        private static readonly int EdgeCostRoad = 0;
        private static readonly int EdgeCostFloorEmpty = 1;
        private static readonly int EdgeCostEmptyEmpty = 1;


        private HashSet<Point> floors;
        private HashSet<Point> walls;
        private HashSet<Point> emptys;
        private List<Dictionary<Point, Edge<Point>>> doors;

        // Graph to complete the paths between the building blocks
        private UndirectedBidirectionalGraph<Point, Edge<Point>> Graph;

        private Dictionary<Edge<Point>, double> EdgeCost;
        private Map map;

        public MapGraph(Map map)
        {
            this.map = map;
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
            for (int y = 0; y < map.MapDimensions.Y; y++)
            {
                for (int x = 0; x < map.MapDimensions.X; x++)
                {
                    if (map.GetCollisionLayerValue(new Point(x, y)) == MapBlock.FloorValue)
                    {
                        floors.Add(new Point(x, y));
                    }
                    else if (map.GetCollisionLayerValue(new Point(x, y)) == MapBlock.WallValue)
                    {
                        walls.Add(new Point(x, y));
                    }
                    else if(map.GetCollisionLayerValue(new Point(x, y)) == MapBlock.EmptyValue)
                    {
                        emptys.Add(new Point(x, y));
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
                    if (!map.ValidTileCoordinates(neighbour)) { continue; }
                    if (map.GetCollisionLayerValue(neighbour) == MapBlock.FloorValue)
                    {
                        Edge<Point> edge = new Edge<Point>(floor, neighbour);
                        EdgeCost.Add(edge, EdgeCostFloorFloor);
                        directedGraph.AddEdge(edge);
                    }
                    else if (map.GetCollisionLayerValue(neighbour) == MapBlock.EmptyValue)
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
                    if (!map.ValidTileCoordinates(neighbour)) { continue; }
                    if (map.GetCollisionLayerValue(neighbour) == MapBlock.EmptyValue)
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
                        if (map.GetCollisionLayerValue(v) == MapBlock.EmptyValue)
                        {
                            map.SetCollisionLayerValue(v, MapBlock.FloorValue);
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
                map.SetCollisionLayerValue(empty, MapBlock.WallValue);
            }
        }


        private  IEnumerable<Edge<Point>> FindPath(Point from, Point to)
        {
            Func<Edge<Point>, double> edgeCost = AlgorithmExtensions.GetIndexer(EdgeCost);
            TryFunc<Point, IEnumerable<Edge<Point>>> tryGetPath = Graph.ShortestPathsDijkstra(edgeCost, from);

            IEnumerable<Edge<Point>> path;
            tryGetPath(to, out path);
            return path;

        }
    }
}
