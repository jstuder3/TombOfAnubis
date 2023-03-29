using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Collections.Generic;

using QuikGraph;

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



        public MovementGraph(Map existingInstance)
        {

            if (existingInstance is null)
            {
                throw new ArgumentNullException("MapArgument");
            }
            this.map = existingInstance;
            this.MAP_N_COLUMNS = map.MapDimensions.Y;
            this.MAP_N_ROWS = map.MapDimensions.X;


            Console.WriteLine("map name: " + map.Name);
            Console.WriteLine("map dimensions: width: " + map.MapDimensions.X + ", height: " + map.MapDimensions.Y);




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
            int node_counter = 0;
            for (int i = 0; i < this.MAP_N_ROWS; i++)
            {
                for (int j = 0; j < this.MAP_N_COLUMNS; j++)
                {
                    if (this.map.CollisionLayer[grid_location_to_nr(i, j)] == 0)
                    {
                        this.grid_nr_to_node_id.Add(grid_location_to_nr(i, j), node_counter);
                        Console.Write(0 + ",");
                        //Console.Write("("+i+","+j+")->(" + grid_location_to_nr(i, j) + "," + node_counter + "), ");
                        node_counter++;
                    } else
                    {
                        Console.Write(1 + ",");
                    }


                }
                Console.WriteLine();
            }
            Console.WriteLine("---------------------");
            this.graph_n_nodes = node_counter + 1;
            
        }


        private void create_graph()
        {
            //bidirectinal edges, only need to test from a grid position all edges to right and down

            var graph = new BidirectionalGraph<int, Edge<int>>();

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
                    if (this.map.CollisionLayer[grid_location_to_nr(i,j)] == 1)
                    {
                        continue;
                    }
                    int cur_grid_id = grid_location_to_nr(i, j);
                    int cur_graph_id = grid_nr_to_node_id[cur_grid_id];

                    if (i < this.MAP_N_ROWS-1 && this.map.CollisionLayer[grid_location_to_nr(i + 1, j)] == 0)
                    {
                        //check if edge to below
                        int to_grid_id = grid_location_to_nr(i + 1, j);
                        int to_graph_id = grid_nr_to_node_id[to_grid_id];
                        //edges.Add(cur_graph_id, to_graph_id);
                        var edge = new Edge<int>(cur_graph_id, to_graph_id);
                        graph.AddEdge(edge);
                        //edges.Add(grid_nr_to_node_id[grid_location_to_nr(i,j)], grid_nr_to_node_id[grid_location_to_nr(i+1,j)]]);
                        n_edges++;

                    }
                    if (j < this.MAP_N_COLUMNS-1 && this.map.CollisionLayer[grid_location_to_nr(i, j+1)] == 0)
                    {
                        // check if edge to right
                        int to_grid_id = grid_location_to_nr(i, j + 1);
                        int to_graph_id = grid_nr_to_node_id[to_grid_id];

                        var edge = new Edge<int>(cur_graph_id, to_graph_id);
                        graph.AddEdge(edge);

                        n_edges++;
                    }
                }
            }
            this.graph_n_edges = n_edges;
        }
        private int grid_location_to_nr(int i, int j)
        {
            /// <summary>
            /// Method <c>grid_location_to_nr</c> maps the grid poition to its grid number.
            /// </summary>
            //return this.MAP_N_COLUMNS * i + j;
            return this.MAP_N_ROWS * j + i;
        }

        public int position_to_node_id(Vector2 position)
        {
            //Console.WriteLine("player initial position: " + position.X + ", " + position.Y);
            int x = (int) Math.Floor(position.X/63.125);
            int y = (int) Math.Floor(position.Y/63.125);
            //Console.WriteLine("rounded x, y: " + x + ", " + y);
            int grid_nr = grid_location_to_nr(x, y);

            int node_id = 5;
            if(!grid_nr_to_node_id.ContainsKey(grid_nr))
            {
                Console.WriteLine("graph: node key not found: key: "+ grid_nr);
                node_id = -1;
                
            } else
            {
                node_id = grid_nr_to_node_id[grid_nr];
            }
            //int node_id = 5;
            //Console.WriteLine("grid nr: " + grid_nr + ", graph node nr: ", node_id);

            return node_id;
        }
    }
}
