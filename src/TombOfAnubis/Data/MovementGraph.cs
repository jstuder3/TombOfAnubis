using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using QuickGraph;
using TombOfAnubis;
using System.Security.Cryptography.X509Certificates;

namespace TombOfAnubisData.Graph
{
    public class MovementGraph
    {
        Map map;
        Map node_id_map;
        int MAP_N_COLUMNS;
        int MAP_N_ROWS;
        int graph_n_nodes;
        Dictionary<int, int> grid_nr_to_node_id;



        public MovementGraph(Map existingInstance, int playerNumber)
        {
            
            if(existingInstance is null)
            {
                throw new ArgumentNullException("MapArgument");
            }
            this.map = existingInstance;
            this.MAP_N_COLUMNS = map.MapDimensions.Y;
            this.MAP_N_ROWS = map.MapDimensions.X;


            Console.WriteLine("map name: " + map.Name);
            Console.WriteLine("map dimensions: width: " + map.MapDimensions.X + ", height: "+ map.MapDimensions.Y);
            



            Console.WriteLine("start initiating graph");
            create_graph_dictionary();
            
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
                    if (this.map.CollisionLayer[grid_location_to_nr(i,j)] == 0)
                    {
                        this.grid_nr_to_node_id.Add(grid_location_to_nr(i, j), node_counter);
                        node_counter++;
                    }
                }
            }

            this.graph_n_nodes = node_counter + 1;
            Console.WriteLine("graph has " + this.graph_n_nodes + " nodes.");
        }

        private int grid_location_to_nr(int i, int j)
        {
            /// <summary>
            /// Method <c>grid_location_to_nr</c> maps the grid poition to its grid number.
            /// </summary>
            return this.MAP_N_COLUMNS * i + j;
        }
    }
}
