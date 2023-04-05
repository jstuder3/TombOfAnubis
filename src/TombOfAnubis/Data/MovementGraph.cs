using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Collections.Generic;

using QuikGraph;
using static Microsoft.Xna.Framework.Graphics.SpriteFont;
using QuikGraph.Algorithms.ShortestPath;
using System.Linq;

namespace TombOfAnubis
{
    public class MovementGraph
    {


        private readonly Map map;
        private Dictionary<Point, int> nodeIds;
        private Dictionary<int, Point> tileCoordinates;

        private FloydWarshallAllShortestPathAlgorithm<int, Edge<int>> fwGraph;
        private int nGraphNodes;
        private int nGraphEdges;

        public MovementGraph(Map map)
        {

            if (map is null)
            {
                throw new ArgumentNullException("MapArgument");
            }
            this.map = map;
            Console.WriteLine("start initiating graph");
            
            CreateNodeTileCoordinateMapping();
            CreateGraph();

            Console.WriteLine("graph has " + nGraphNodes + " nodes and " + nGraphEdges + " edges.");
            Console.WriteLine("finished creating graph, dajuuum");
        }



        private void CreateNodeTileCoordinateMapping()
        {
            //inititate dictionary
            nodeIds = new Dictionary<Point, int>();
            tileCoordinates = new Dictionary<int, Point>();
            int node_counter = 0;
            //Console.WriteLine("draw Map: ");
            
            
            for (int j = 0; j < map.MapDimensions.Y; j++)
            {
                for (int i = 0; i < map.MapDimensions.X; i++)
                {
                    Point tileCoordinate = new Point(i, j);
                    if (map.GetCollisionLayerValue(tileCoordinate) == 0)
                    {
                        nodeIds.Add(tileCoordinate, node_counter);
                        tileCoordinates.Add(node_counter, tileCoordinate);
                        //graph.AddVertex(node_counter);
                        node_counter++;
                        //Console.Write(0 + ",");
                    } else
                    {
                        //Console.Write(1 + ",");
                    }


                }
                //Console.WriteLine();
            }
            //Console.WriteLine("---------------------");
            nGraphNodes = node_counter + 1;
            
        }


        private void CreateGraph()
        {
            //bidirectinal edges, only need to test from a grid position all edges to right and down

            var graph = new BidirectionalGraph<int, Edge<int>>();
            Console.WriteLine("type of graph: " + graph.GetType());


            //first add all nodes to the graph:
            for (int i = 0; i < nGraphNodes; i++)
            {
                graph.AddVertex(i);
            }


            //var edge = new Edge<int>(2, 4);
            //graph.AddEdge(Edge<int>(2, 4));


            //Dictionary<int, int> edges = new Dictionary<int, int>();
            nGraphEdges = 0;
            for (int j = 0; j < map.MapDimensions.Y; j++)
            {
                for (int i = 0; i < map.MapDimensions.X; i++)
                {
                    if (map.GetCollisionLayerValue(new Point(i, j)) == 1)
                    {
                        continue;
                    }

                    int currNodeId = nodeIds[new Point(i, j)];
                    List<Point> neighbours = new List<Point>() { new Point(i + 1, j), new Point(i - 1, j), new Point(i, j + 1), new Point(i, j - 1) };
                    foreach (Point neighbour in neighbours)
                    {
                        if (map.ValidTileCoordinates(neighbour) && map.GetCollisionLayerValue(neighbour) == 0)
                        {
                            graph.AddEdge(new Edge<int>(currNodeId, nodeIds[neighbour]));
                            nGraphEdges++;
                        }
                    }
                }
            }

            Func<Edge<int>, double> weights = edge => 1;
            //var fw = new FloydWarshallAllShortestPathAlgorithm<int, Edge<int>>(graph, weights);
            fwGraph = new FloydWarshallAllShortestPathAlgorithm<int, Edge<int>>(graph, weights);
            fwGraph.Compute();

            // Get interesting paths
            /*
            foreach (int source in graph.Vertices)
            {
                foreach (int target in graph.Vertices)
                {
                    if (fw_graph.TryGetPath(source, target, out IEnumerable<Edge<int>> path))
                    {
                        Console.WriteLine("for node " + source + " to " + target + " path: ");
                        foreach (Edge<int> edge in path)
                        {
                            Console.WriteLine(edge);
                        }
                    }
                }
            }
            */


        }

        public int ToNodeID(Vector2 position)
        {
            Point tileCoordinates = Session.GetInstance().Map.PositionToTileCoordinate(position);
            if (nodeIds.ContainsKey(tileCoordinates))
            {
                return nodeIds[tileCoordinates];
            }
            return -1;
        }
        public Point ToTileCoordinate(int nodeID)
        {
            if (tileCoordinates.ContainsKey(nodeID))
            {
                return tileCoordinates[nodeID];
            }
            return new Point(-1, -1);
        }

        public Vector2 ToPosition(int nodeID)
        {
            Point tileCoordinates = ToTileCoordinate(nodeID);
            if (map.ValidTileCoordinates(tileCoordinates)){
                return map.TileCoordinateToPosition(tileCoordinates);
            }
            else
            {
                return new Vector2(-1, -1);
            }
            
        }
        
        public bool CheckPathExists(int source, int target)
        {
            return fwGraph.TryGetPath(source, target, out IEnumerable<Edge<int>> path);
        }

        public IEnumerable<Edge<int>> GetPath(int source, int target)
        {
            fwGraph.TryGetPath(source, target, out IEnumerable<Edge<int>> path);
            return path;
        }
        public Vector2 GetTargetToWalkTo(int source, int target)
        {
            //assumes path exists
            fwGraph.TryGetPath(source, target, out IEnumerable<Edge<int>> path);

            Edge<int> to = path.First();
            if(to.Source != source)
            {
                throw new ArgumentException();
            }

            Point targetTileCoordinates = tileCoordinates[to.Target];
            //Tuple<int,int> direction_position_tp = grid_nr_to_grid_location(target_grid_nr);
            //Vector2 ret = new Vector2(float(direction_position_tp.Item2) * 63.125, float(direction_position_tp.Item1)* 63.125);
            //Vector2 ret = new Vector2(Convert.ToSingle(direction_position_tp.Item1) * Convert.ToSingle(63.125), Convert.ToSingle(direction_position_tp.Item2)*Convert.ToSingle(63.125));
            return new Vector2(1,1);
        }

        public int GetDistance(int source, int target)
        {
            if(fwGraph.TryGetPath(source, target, out IEnumerable<Edge<int>> path)) 
            {
                return path.Count();
            } else
            {
                return -1;
            }

        }
    }
}
