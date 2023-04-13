using Microsoft.Xna.Framework;
using QuikGraph;
using QuikGraph.Algorithms;
using Sdcb.FFmpeg.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace TombOfAnubis
{
    public class LevelGenerator
    {

        // Needs to be dividable by 3
        public Point LevelDimensions { get; set; }

        private int[,] level;
        private LevelBuildingBlock[,] blockGrid;
        private List<LevelBuildingBlock> blocks;
        private LevelBuildingBlock fillerBlock;
        private Random rand;
        private int numPlayers;

        // Graph to complete the paths between the building blocks
        private UndirectedBidirectionalGraph<Point, Edge<Point>> Graph;
        private Dictionary<Edge<Point>, double> EdgeCost;

        public LevelGenerator(Point levelDimensions, List<LevelBuildingBlock> levelBuildingBlocks, int numPlayers)
        {
            this.numPlayers = numPlayers;
            LevelDimensions = levelDimensions;
            level = new int[levelDimensions.X, levelDimensions.Y];
            blockGrid = new LevelBuildingBlock[levelDimensions.X / 3, levelDimensions.Y / 3];

            blocks = levelBuildingBlocks;
            blocks.Add(LevelBuildingBlock.OneByOne(1));
            blocks.Add(LevelBuildingBlock.TwoByOne(1));
            blocks.Add(LevelBuildingBlock.TwoByTwo(1));
            blocks.Add(LevelBuildingBlock.Altar());
            blocks.Add(LevelBuildingBlock.Artefakt(numPlayers));
            blocks.Add(LevelBuildingBlock.SpawnLocation(numPlayers));

            fillerBlock = new LevelBuildingBlock();
            fillerBlock.Dimensions = new Point(LevelBuildingBlock.SmallestBlockSize.X,LevelBuildingBlock.SmallestBlockSize.Y);
            fillerBlock.Values = new int[LevelBuildingBlock.SmallestBlockSize.X, LevelBuildingBlock.SmallestBlockSize.Y];
            fillerBlock.Name = "f";
            Populate(fillerBlock.Values, 2);

            fillerBlock.Priority = 1;
            rand = new Random();

        }
        public int[,] GenerateLevel()
        {
            int numAttempts = 0;
            int maxAttempts = 50;
            for(; numAttempts < maxAttempts; numAttempts++)
            {
                numAttempts++;
                GenerateBlockGrid();
                if (ValidateBlockGrid()) break;
                foreach(LevelBuildingBlock block in blocks)
                {
                    block.Reset();
                }
            }
            Console.WriteLine("Num Attempts: " + numAttempts);

            PrintBlockGrid();
            MapBlockGridToLevel();
            CreatGraphFromLevel();
            PrintLevel();

            return level;
        }

        private void CreatGraphFromLevel()
        {
            for (int i = 0; i < blockGrid.GetLength(0); i++)
            {
                for (int j = 0; j < blockGrid.GetLength(1); j++)
                {
                }
            }
        }

        private void MapBlockGridToLevel()
        {
            for (int i = 0; i < blockGrid.GetLength(0); i++)
            {
                for (int j = 0; j < blockGrid.GetLength(1); j++)
                {
                    LevelBuildingBlock block = blockGrid[i, j];
                    if (block.Written) { continue; }
                    Point levelCoordinate = new Point(i * LevelBuildingBlock.SmallestBlockSize.X, j * LevelBuildingBlock.SmallestBlockSize.Y);

                    for (int k = 0; k < block.Dimensions.X; k++)
                    {
                        for (int l = 0; l < block.Dimensions.Y; l++)
                        {
                            Point p = new Point(levelCoordinate.X + k, levelCoordinate.Y + l);
                            level[p.X, p.Y] = block.Values[k, l];
                        }
                    }
                    block.Written = true;
                }
            }
        }

        private void GenerateBlockGrid()
        {
            blockGrid = new LevelBuildingBlock[LevelDimensions.X / 3, LevelDimensions.Y / 3];
            // Outermost border are only filler blocks
            for (int i = 0; i < blockGrid.GetLength(0); i++)
            {
                for (int j = 0; j < blockGrid.GetLength(1); j++)
                {
                    if (i == 0 || j == 0 || i == blockGrid.GetLength(0) - 1 || j == blockGrid.GetLength(1) - 1)
                    {
                        blockGrid[i, j] = fillerBlock.Clone();
                    }
                }
            }

            // Fill the inside grid with LevelBuildingBlocks
            for (int i = 0; i < blockGrid.GetLength(0); i++)
            {
                for (int j = 0; j < blockGrid.GetLength(1); j++)
                {
                    if(blockGrid[i, j] == null)
                    {
                        List<LevelBuildingBlock> blockCandidates = GetBlockCandidates(new Point(i, j));
                        if (blockCandidates.Count > 0)
                        {
                            LevelBuildingBlock winner = PickCandidateBasedOnPriority(blockCandidates);
                            Place(new Point(i, j), winner);
                            foreach(LevelBuildingBlock block in blocks)
                            {
                                block.Update();
                            }
                        }
                        else
                        {
                            blockGrid[i, j] = fillerBlock.Clone();
                        }
                    }
                }
            }
        }

        private List<LevelBuildingBlock> GetBlockCandidates(Point blockGridCoord)
        {
            List<LevelBuildingBlock> res = new List<LevelBuildingBlock>();
            foreach(LevelBuildingBlock block in blocks)
            {
                if(CanPlace(blockGridCoord, block))
                {
                    res.Add(block);
                }
            }
            return res;
        }

        private void Populate(int[,] arr, int value)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    arr[i, j] = value;
        }

        private bool CanPlace(Point blockGridCoord, LevelBuildingBlock block)
        {
            for(int i = 0; i < block.BlockGridDimensions.X; i++)
            {
                for(int j = 0; j < block.BlockGridDimensions.Y; j++)
                {
                    Point destBlockGoord = blockGridCoord + new Point(i, j);
                    if(!ValidBlockGridLocation(destBlockGoord) || blockGrid[destBlockGoord.X, destBlockGoord.Y] != null){
                        return false;
                    }
                }
            }
            return true;
        }
        private void Place(Point blockGridCoord, LevelBuildingBlock block)
        {
            LevelBuildingBlock placedBlock = block.Clone();
            // Place block
            for (int i = 0; i < block.BlockGridDimensions.X; i++)
            {
                for (int j = 0; j < block.BlockGridDimensions.Y; j++)
                {
                    Point destBlockGoord = blockGridCoord + new Point(i, j);
                    blockGrid[destBlockGoord.X, destBlockGoord.Y] = placedBlock;
                }
            }

            // Place a border of fillerblocks around the placed block
            Point borderStart = new Point(blockGridCoord.X - 1, blockGridCoord.Y - 1);
            for (int i = 0; i < block.BlockGridDimensions.X + 2; i++)
            {
                for (int j = 0; j < block.BlockGridDimensions.Y + 2; j++)
                {
                    Point destBlockGoord = borderStart + new Point(i, j);
                    if(ValidBlockGridLocation(destBlockGoord) && blockGrid[destBlockGoord.X, destBlockGoord.Y] == null)
                    {
                        blockGrid[destBlockGoord.X, destBlockGoord.Y] = fillerBlock.Clone();
                    }
                }
            }

        }
        private bool ValidBlockGridLocation(Point blockGridCoord)
        {
            return blockGridCoord.X >= 0 && blockGridCoord.X < LevelDimensions.X / 3 && blockGridCoord.Y >= 0 && blockGridCoord.Y < LevelDimensions.Y / 3;
        }
        private bool ValidateBlockGrid()
        {
            foreach(LevelBuildingBlock block in blocks)
            {
                if (!block.RequirementSatisfied()){
                    return false;
                }
            }
            return true;
        }

        private LevelBuildingBlock PickCandidateBasedOnPriority(List<LevelBuildingBlock> blocks)
        {
            int sum = 0;
            foreach (LevelBuildingBlock block in blocks)
            {
                sum += block.Priority;
            }
            int selection = rand.Next(sum);
            sum = 0;
            foreach (LevelBuildingBlock block in blocks)
            {
                sum += block.Priority;
                if(selection < sum)
                {
                    return block;
                }
            }
            return null;
        }

        public void FindPath(Point from, Point to)
        {
            Func<Edge<Point>, double> edgeCost = AlgorithmExtensions.GetIndexer(EdgeCost);
            // Positive or negative weights
            TryFunc<Point, System.Collections.Generic.IEnumerable<Edge<Point>>> tryGetPath = Graph.ShortestPathsDijkstra(edgeCost, from);

            IEnumerable<Edge<Point>> path;
            if (tryGetPath(to, out path))
            {
                Console.Write("Path found from {0} to {1}: {0}", from, to);
                foreach (var e in path) { Console.Write(" > {0}", e.Target); }
                Console.WriteLine();
            }
            else { Console.WriteLine("No path found from {0} to {1}."); }
        }

        public void PrintLevel()
        {
            Console.WriteLine("----------------Level----------------");
            for (int i = 0; i < level.GetLength(0); i++)
            {
                for (int j = 0; j < level.GetLength(1); j++)
                {
                    Console.Write(level[i, j] + ",");
                }
                Console.WriteLine();
            }
            Console.WriteLine("----------------Level----------------");

        }

        public void PrintBlockGrid()
        {
            Console.WriteLine("----------------BlockGrid----------------");
            for (int i = 0; i < blockGrid.GetLength(0); i++)
            {
                for (int j = 0; j < blockGrid.GetLength(1); j++)
                {
                    Console.Write(blockGrid[i, j]?.Name + ",");
                }
                Console.WriteLine();
            }
            Console.WriteLine("----------------BlockGrid----------------");

        }
    }
}
