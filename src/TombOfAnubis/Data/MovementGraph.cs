using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Collections.Generic;

using QuikGraph;
using static Microsoft.Xna.Framework.Graphics.SpriteFont;
using QuikGraph.Algorithms.ShortestPath;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TombOfAnubis
{
    public class MovementGraph
    {


        public readonly Map map;
        private Dictionary<Point, int> nodeIds;
        private Dictionary<int, Point> tileCoordinates;

        private FloydWarshallAllShortestPathAlgorithm<int, Edge<int>> fwGraph;
        private int nGraphNodes;
        private int nGraphEdges;

        private int nNodesPerTile = 9;

        public MovementGraph(Map map)
        {

            if (map is null)
            {
                throw new ArgumentNullException("MapArgument");
            }
            this.map = map;
            //Console.WriteLine("start initiating graph");

            //try to get map info


            
            CreateNodeTileCoordinateMapping();
            CreateGraph();

            //Console.WriteLine("graph has " + nGraphNodes + " nodes and " + nGraphEdges + " edges.");
            //Console.WriteLine("finished creating graph, dajuuum");
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
                        //Console.Write(0 + ",");
                        /*
                        for (int k = 0; k < nNodesPerTile; k++)
                        {
                            
                            nodeIds.Add(tileCoordinate, node_counter);
                            tileCoordinates.Add(node_counter, tileCoordinate);
                            node_counter++;
                        }
                        */
                        nodeIds.Add(tileCoordinate, node_counter);
                        tileCoordinates.Add(node_counter, tileCoordinate);
                        node_counter++;
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
                        Console.WriteLine("from node " + source + " to " + target + " path: ");
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
            Vector2 offset = new Vector2(200, 200);
            if (map.ValidTileCoordinates(tileCoordinates)){
                return map.TileCoordinateToPosition(tileCoordinates) + offset;
            }
            else
            {
                return new Vector2(-1, -1);
            }
        }

        public Vector2 ToCenterTilePosition(int nodeID)
        {
            Point tileCoordinates = ToTileCoordinate(nodeID);
            if (map.ValidTileCoordinates(tileCoordinates))
            {
                return map.TileCoordinateToTileCenterPosition(tileCoordinates);
            }
            else
            {
                return new Vector2(-1, -1);
            }
        }
        
        public bool PathExists(int source, int target)
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
            return ToPosition(to.Target);
        }

        public Vector2 getNthTargetToWalkTo(int source, int target, int n)
        {
            //assumes path exists
            fwGraph.TryGetPath(source, target, out IEnumerable<Edge<int>> path);

            int counter = 1;

            foreach (Edge<int> edge in path)
            {
                if (counter == n)
                {
                    return ToPosition(edge.Target);
                }
                counter++;
            }

            Console.WriteLine("Error: MovementGraph 34");
            return new Vector2(0, 0);



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

        public bool isTileNeighbor(Point main, Point potential_neighbor)
        {
            //check if both itles are valid then check if tiles are close (dist equal to one or sqrt(2))
            if(map.ValidTileCoordinates(main) && map.ValidTileCoordinates(potential_neighbor) 
                && (Math.Pow(main.X - potential_neighbor.X,2) + Math.Pow(main.Y-potential_neighbor.Y,2)) < 1.6)
            {
                return true;
            }
            return false;
        }
    }

    public class MovementGraph2
    {
        public readonly Map map;
        private int binSize;
        private int WorldSizeHeight;
        private int WorldSizeWidth;
        private int nBinsPerColumn;
        private int nBinsPerRow;

        public MovementGraph2(Map map, World world)
        {

            if (map is null)
            {
                throw new ArgumentNullException("MapArgument");
            }
            this.map = map;

            //compute fieldBinSize:
            Character player = world.GetChildrenOfType<Character>().First();
            Vector2 topLeftPosition = player.TopLeftCornerPosition();
            Vector2 centrePosition = player.CenterPosition();
            Vector2 halfDiag = centrePosition - topLeftPosition;
            float halfDist = halfDiag.Length();
            //one sidelength is: 2^0.5*halfDist
            float characterSideLength = (float)1.14142 * halfDist;

            int nBinsPerCharacterSideLength = 4;
            //round binsize up to next multiple of characterSieLength
            int sideLength = (int)characterSideLength + (nBinsPerCharacterSideLength - (int)characterSideLength % nBinsPerCharacterSideLength);
            this.binSize = sideLength / nBinsPerCharacterSideLength;
            Console.WriteLine("binSize: " + this.binSize);

            //needs to be variable
            int WorldSizeHeight = 10000;
            int WorldSizeWidth = 8000;

            //round up to be multiple of binSize
            this.WorldSizeHeight += this.binSize - (WorldSizeHeight % this.binSize);
            this.WorldSizeWidth += this.binSize - (WorldSizeWidth % this.binSize);
            Console.WriteLine("WorldSize: " + this.WorldSizeWidth + ", " + this.WorldSizeHeight);

            this.nBinsPerColumn = WorldSizeHeight / this.binSize;
            this.nBinsPerRow = WorldSizeWidth / this.binSize;
            Console.WriteLine("nBins: " + this.nBinsPerRow + ", " + this.nBinsPerColumn);


            //createGraph();


        }

        Point ToIndex(int binNr)
        {
            return new Point(binNr % this.nBinsPerRow, (int)binNr / this.nBinsPerRow);
        }

        Point ToIndex(Vector2 position)
        {
            int rowIndex = (int)position.X / this.binSize;
            int colIndex = (int)position.Y / this.binSize;
            return new Point(rowIndex, colIndex);
        }

        int ToBinNr(Point index)
        {
            return index.Y * this.nBinsPerRow + index.X;
        }

        int ToBinNr(Vector2 position)
        {
            return ToBinNr(ToIndex(position));
        }

        Vector2 ToPosition(Point index)
        {
            int offset = (int)this.binSize / 2;
            return new Vector2(index.X * this.binSize + offset, index.Y * this.binSize + offset);
        }

        Vector2 ToPosition(int binNr)
        {
            return ToPosition(ToIndex(binNr));
        }

        private void CreateGraph()
        {
            //1. make boolean matrix for all bins, mark all wall(&other stuff)bins as inactive
            //default is false, thus setting all to true if a wall is in that bin, then use all bins who are still false as graph nodes
            bool[,] arr = new bool[this.nBinsPerColumn, this.nBinsPerRow];

            //iterate over all map objects
        }
    }
}
