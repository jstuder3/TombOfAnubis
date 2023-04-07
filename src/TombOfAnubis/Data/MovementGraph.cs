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
        Map map;
        Map node_id_map;
        int MAP_N_COLUMNS;
        int MAP_N_ROWS;
        int graph_n_nodes;
        int graph_n_edges;
        Dictionary<int, int> grid_nr_to_node_id;
        Dictionary<int, int> node_id_to_grid_nr;
        FloydWarshallAllShortestPathAlgorithm<int, Edge<int>> fw_graph;



        public MovementGraph(Map existingInstance)
        {

            if (existingInstance is null)
            {
                throw new ArgumentNullException("MapArgument");
            }
            this.map = existingInstance;
            this.MAP_N_COLUMNS = map.MapDimensions.X;
            this.MAP_N_ROWS = map.MapDimensions.Y;




            Console.WriteLine("start initiating graph");
            
            create_graph_dictionary();
            create_graph();

            Console.WriteLine("graph has " + this.graph_n_nodes + " nodes and " + this.graph_n_edges + " edges.");
            Console.WriteLine("finished creating graph, dajuuum");
        }



        private void create_graph_dictionary()
        {
            //check if map and its dimensions are already set up
            if (this.MAP_N_COLUMNS == 0 || this.MAP_N_ROWS == 0)
            {
                throw new ArgumentNullException("MapInitialization");
            }

            //inititate dictionary
            this.grid_nr_to_node_id = new Dictionary<int, int>();
            this.node_id_to_grid_nr = new Dictionary<int, int>();
            int node_counter = 0;
            Console.WriteLine("draw Map: ");
            
            
            for (int i = 0; i < this.MAP_N_ROWS; i++)
            {
                for (int j = 0; j < this.MAP_N_COLUMNS; j++)
                {
                    if (this.map.CollisionLayer[grid_location_to_grid_nr(i, j)] == 0)
                    {
                        this.grid_nr_to_node_id.Add(grid_location_to_grid_nr(i, j), node_counter);
                        this.node_id_to_grid_nr.Add(node_counter, grid_location_to_grid_nr(i, j));
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
            this.graph_n_nodes = node_counter + 1;
            
        }


        private void create_graph()
        {
            //bidirectinal edges, only need to test from a grid position all edges to right and down

            var graph = new BidirectionalGraph<int, Edge<int>>();
            Console.WriteLine("type of graph: " + graph.GetType());

            
            //first add all nodes to the graph:
            for (int i = 0; i < this.graph_n_nodes; i++)
            {
                graph.AddVertex(i);
            }
            

            //var edge = new Edge<int>(2, 4);
            //graph.AddEdge(Edge<int>(2, 4));


            //Dictionary<int, int> edges = new Dictionary<int, int>();
            int n_edges = 0;
            for (int j = 0; j < this.MAP_N_COLUMNS; j++) 
            {

                for (int i = 0; i < this.MAP_N_ROWS; i++)
                {
                    if (this.map.CollisionLayer[grid_location_to_grid_nr(i,j)] == 1)
                    {
                        continue;
                    }
                    
                    int cur_grid_id = grid_location_to_grid_nr(i, j);
                    int cur_graph_id = grid_nr_to_node_id[cur_grid_id];

                    if (i < this.MAP_N_ROWS-1 && this.map.CollisionLayer[grid_location_to_grid_nr(i + 1, j)] == 0)
                    {
                        //check if edge to below
                        int target_grid_id = grid_location_to_grid_nr(i + 1, j);
                        int target_graph_id = grid_nr_to_node_id[target_grid_id];

                        var edge = new Edge<int>(cur_graph_id, target_graph_id);
                        Console.WriteLine("try to add edge. #nodes: " + this.graph_n_nodes + ", edge: " + edge.Source + "->" + edge.Target);
                        graph.AddEdge(edge);
                        //edges.Add(grid_nr_to_node_id[grid_location_to_nr(i,j)], grid_nr_to_node_id[grid_location_to_nr(i+1,j)]]);
                        n_edges++;

                    }
                    if (j < this.MAP_N_COLUMNS-1 && this.map.CollisionLayer[grid_location_to_grid_nr(i, j+1)] == 0)
                    {
                        // check if edge to right
                        int to_grid_id = grid_location_to_grid_nr(i, j + 1);
                        int to_graph_id = grid_nr_to_node_id[to_grid_id];

                        var edge = new Edge<int>(cur_graph_id, to_graph_id);
                        graph.AddEdge(edge);

                        n_edges++;
                    }
                }
            }
            this.graph_n_edges = n_edges;

            
            Func<Edge<int>, double> weights = edge => 1;
            //var fw = new FloydWarshallAllShortestPathAlgorithm<int, Edge<int>>(graph, weights);
            fw_graph = new FloydWarshallAllShortestPathAlgorithm<int, Edge<int>>(graph, weights);
            fw_graph.Compute();

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

        /// <summary>
        /// Method <c>grid_location_to_grid_nr</c> maps the grid poition to its grid number.
        /// Pair (i,j) represents (row_index,column_index), eg. standart matrix notation
        /// </summary>
        public int grid_location_to_grid_nr(int i, int j)
        {
            
            //Map is stored columnwise
            return this.MAP_N_COLUMNS * i + j;
          
        }

        /// <summary>
        /// Method <c>grid_nr_to_grid_location</c> maps the grid number to the grid location.
        /// Pair/Tuple (i,j) represents (row_index,column_index), eg. standart matrix notation
        /// </summary>
        public Point grid_nr_to_grid_location(int nr)
        {
            
            int j = nr % this.MAP_N_COLUMNS;
            int i = (nr - j) / this.MAP_N_ROWS;
            return new Point(j, i);
        }


        /// <summary>
        /// Method <c>world_position_to_node_id</c> maps the world poition (top left of a sprite) to its location (node id) on the graph.
        /// The node id can be used in all graphs for shortest path or any other operations
        /// If the mapping fails, the return value is -1 thus invalid.
        /// </summary>
        public int world_position_to_node_id(Vector2 position)
        {
            //first get mid position of sprite
            var map = Session.GetInstance().Map;
            Vector2 centerPos = new Vector2(position.X / map.TileSize.X, position.Y / map.TileSize.Y);

            Point tilePosition = Session.GetInstance().Map.PositionToTileCoordinate(position);
            int gridNumber = grid_location_to_grid_nr(tilePosition.Y, tilePosition.X);
            Console.WriteLine("tilePos to gridNumber: " + tilePosition + " -> " + gridNumber);
            int nodeID = -1;
            if (grid_nr_to_node_id.ContainsKey(gridNumber))
            {
                nodeID = grid_nr_to_node_id[gridNumber];
            } else
            {
                Console.WriteLine("graph: node key not found: key: " + gridNumber);
            }
            Console.WriteLine("world posiition otpleft & middle, TilePosition, GridNumber, NodeID: " + position + " -> " + centerPos + " -> " + tilePosition + " -> " + nodeID);
            return nodeID;
        }

        public Vector2 node_id_to_world_position(int nodeID)
        {
            if (this.node_id_to_grid_nr.ContainsKey(nodeID))
            {
                int tileNumber = this.node_id_to_grid_nr[nodeID];
                Point tilePosition = grid_nr_to_grid_location(tileNumber);

                var map = Session.GetInstance().Map;
                Vector2 topLeft = new Vector2(tilePosition.X * map.TileSize.X, tilePosition.Y * map.TileSize.Y);
                Vector2 centerPos = topLeft +  map.TileSize / 2;
                return topLeft;

            }
            return new Vector2(-1, -1);
        }

        public Vector2 node_id_to_position(int node_id)
        {
            if(this.node_id_to_grid_nr.ContainsKey(node_id))
            {
                int grid_nr = this.node_id_to_grid_nr[node_id];
                //Tuple<int,int> grid_position_tuple = grid_nr_to_grid_location(grid_nr);
                Point gridPosition = grid_nr_to_grid_location(grid_nr);
                Vector2 grid_position = new Vector2(gridPosition.Y, gridPosition.X);
                Point tile_position = new Point((int)grid_position.X, (int)grid_position.Y);

                //Session.GetInstance().Map.TileCoordinateToPosition(tile_position);
                var map = Session.GetInstance().Map;
                Vector2 centerPos = new Vector2(tile_position.X * map.TileSize.X, tile_position.Y * map.TileSize.Y);
                centerPos += map.TileSize / 2;



                //Vector2 world_position = new Vector2(Convert.ToSingle(grid_position_tuple.Item1) * Convert.ToSingle(63.125), Convert.ToSingle(grid_position_tuple.Item2) * Convert.ToSingle(63.125));

                return new Vector2(tile_position.X, tile_position.Y);

            }
            //node id does  not exist, return error
            return new Vector2(-1,-1);
        }

        
        public bool check_path_exists(int source, int target)
        {
            return fw_graph.TryGetPath(source, target, out IEnumerable<Edge<int>> path);
        }

        public IEnumerable<Edge<int>> get_path(int source, int target)
        {
            fw_graph.TryGetPath(source, target, out IEnumerable<Edge<int>> path);
            return path;
        }
        public Vector2 get_target_to_walk_to(int source, int target)
        {
            //assumes path exists
            fw_graph.TryGetPath(source, target, out IEnumerable<Edge<int>> path);

            Edge<int> to = path.First();
            if(to.Source != source)
            {
                throw new ArgumentException();
            }

            int target_grid_nr = node_id_to_grid_nr[to.Target];
            //Tuple<int,int> direction_position_tp = grid_nr_to_grid_location(target_grid_nr);
            //Vector2 ret = new Vector2(float(direction_position_tp.Item2) * 63.125, float(direction_position_tp.Item1)* 63.125);
            //Vector2 ret = new Vector2(Convert.ToSingle(direction_position_tp.Item1) * Convert.ToSingle(63.125), Convert.ToSingle(direction_position_tp.Item2)*Convert.ToSingle(63.125));
            return new Vector2(1,1);
        }

        public int get_distance(int source, int target)
        {
            if(fw_graph.TryGetPath(source, target, out IEnumerable<Edge<int>> path)) 
            {
                return path.Count();
            } else
            {
                return -1;
            }

        }
    }
}
