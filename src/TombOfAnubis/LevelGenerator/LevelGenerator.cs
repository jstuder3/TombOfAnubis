using Microsoft.Xna.Framework;
using QuikGraph;
using QuikGraph.Algorithms;
using System;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class LevelGenerator
    {

        // Needs to be dividable by 3
        public Point LevelDimensions { get; set; }

        private int[,] level;
        //private LevelBlock[,] levelBlockGrid;
        private List<LevelBlock> levelPieces;
        //private LevelPiece fillerPiece;
        private Random rand;
        private int numPlayers;

        // Graph to complete the paths between the building blocks
        private UndirectedBidirectionalGraph<Point, Edge<Point>> Graph;
        private Dictionary<Edge<Point>, double> EdgeCost;

        public LevelGenerator(Point levelDimensions, List<LevelBlock> levelPieces, int numPlayers)
        {
            this.numPlayers = numPlayers;
            LevelDimensions = levelDimensions;
            level = new int[levelDimensions.X, levelDimensions.Y];
            Populate(level, LevelBlock.InvalidValue);
            //levelBlockGrid = new LevelBlock[levelDimensions.X / 3, levelDimensions.Y / 3];

            this.levelPieces = levelPieces;
            //this.levelPieces.Add(LevelPiece.Empty);
            //this.levelPieces.Add(new LevelPiece(new int[,] { {1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } }, 2));
            this.levelPieces.Add(new LevelBlock(new int[,] { 
                { 1, 0, 1 ,1}, 
                { 1, 0, 1, 1}, 
                { 0, 0, 0, 1},
                { 1, 1, 0, 1}}, 2));
            this.levelPieces.Add(new LevelBlock(new int[,] {
                { 1, 0, 1 ,1},
                { 1, 88, 0, 0},
                { 0, 0, 0, 1},
                { 1, 0, 1, 1}}, 1, 1, 1));
            //this.levelPieces.Add(LevelBlock.TwoByOne(1));
            //this.levelPieces.Add(LevelBlock.TwoByTwo(1));
            //this.levelPieces.Add(LevelBlock.Altar());
            //this.levelPieces.Add(LevelBlock.Artefakt(numPlayers));
            //this.levelPieces.Add(LevelBlock.SpawnLocation(numPlayers));

            //fillerBlock = new LevelBlock();
            //fillerBlock.Dimensions = new Point(LevelBlock.BlockSize.X,LevelBlock.BlockSize.Y);
            //fillerBlock.Values = new int[LevelBlock.BlockSize.X, LevelBlock.BlockSize.Y];
            //fillerBlock.Name = "f";
            //Populate(fillerBlock.Values, 2);

            //fillerBlock.Priority = 1;
            rand = new Random();

        }
        public int[,] GenerateLevel()
        {
            int numAttempts = 0;
            int maxAttempts = 50;
            for(; numAttempts < maxAttempts; numAttempts++)
            {
                numAttempts++;
                CreateLevel();
                if (ValidateLevel()) break;
                foreach (LevelBlock piece in levelPieces)
                {
                    piece.Reset();
                }
            }
            Console.WriteLine("Num Attempts: " + numAttempts);

            //PrintBlockGrid();
            //MapBlockGridToLevel();
            //CreatGraphFromLevel();
            PrintLevel();

            return level;
        }

        private void CreateLevel()
        {
            CreateBorder();
            for (int x = 0; x < LevelDimensions.X; x++)
            {
                for(int y = 0; y < LevelDimensions.Y; y++)
                {
                    Point coord = new Point(x, y);

                    List<LevelBlock> candidates = GetCandidates(coord);
                    if (candidates.Count == 0)
                    {
                        if(CanPlace(LevelBlock.Empty, coord))
                         {
                            Place(LevelBlock.Empty, coord);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        LevelBlock winner = PickCandidateBasedOnPriority(candidates);
                        Place(winner, coord);
                        CreateBorder(coord, winner);
                        UpdateLevelPieces(winner);
                    }
                }
                    
            }
        }
        private void CreateBorder()
        {
            for(int i = 0; i < LevelDimensions.X; i += LevelBlock.Road.Dimensions.X)
            {
                for(int j =0; j < LevelDimensions.Y; j += LevelBlock.Road.Dimensions.Y)
                {
                    if(i == 0 || i == LevelDimensions.X - LevelBlock.Road.Dimensions.X || j == 0 || j == LevelDimensions.Y - LevelBlock.Road.Dimensions.Y)
                    {
                        if(CanPlace(LevelBlock.Road, new Point(j, i)))
                        {
                            Place(LevelBlock.Road, new Point(j, i));
                        }
                    }
                }
            }
        }

        private List<LevelBlock> GetCandidates(Point coord)
        {
            List<LevelBlock> candidates = new List<LevelBlock>();
            foreach (LevelBlock piece in levelPieces) 
            {
                if(CanPlace(piece, coord))
                {
                    candidates.Add(piece);
                }
            }
            return candidates;
        }

        private bool CanPlace(LevelBlock piece, Point coord)
        {
            for (int i = 0; i < piece.Dimensions.X; i++)
            {
                for (int j = 0; j < piece.Dimensions.Y; j++)
                {
                    Point dst = coord + new Point(i, j);
                    if (!ValidCoords(dst) || level[dst.X, dst.Y] != LevelBlock.InvalidValue)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void UpdateLevelPieces(LevelBlock winner)
        {
            foreach (LevelBlock piece in levelPieces)
            {
                piece.Update(winner.Equals(piece));
            }
        }
        private void Place(LevelBlock levelPiece, Point coord)
        {
            for (int i = 0; i < levelPiece.Dimensions.X; i++)
            {
                for(int j = 0; j < levelPiece.Dimensions.Y; j++)
                {
                    Point dest = coord + new Point(i, j);
                    if (ValidCoords(dest))
                    {
                        level[dest.X, dest.Y] = levelPiece.GetValue(new Point(i, j));
                    }
                }
            }
        }
        private void CreateBorder(Point coord, LevelBlock piece)
        {
            int w = piece.Dimensions.X;
            int h = piece.Dimensions.Y;
            int x = coord.X;
            int y = coord.Y;
            int emptyW = LevelBlock.Road.Dimensions.X;
            int emptyH = LevelBlock.Road.Dimensions.Y;
            for (int i = x - emptyW; i < x + w + emptyW; i++)
            {
                for (int j = y - emptyH; j < y + h + emptyH; j++)
                {
                    if (CanPlace(LevelBlock.Road, new Point(i, j)))
                    {
                        Place(LevelBlock.Road, new Point(i, j));
                    }
                }
            }
        }

        //private void CreatGraphFromLevel()
        //{
        //    for (int i = 0; i < levelBlockGrid.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < levelBlockGrid.GetLength(1); j++)
        //        {
        //        }
        //    }
        //}

        //private void MapBlockGridToLevel()
        //{
        //    for (int i = 0; i < levelBlockGrid.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < levelBlockGrid.GetLength(1); j++)
        //        {
        //            LevelBlock block = levelBlockGrid[i, j];
        //            if (block.Written) { continue; }
        //            Point levelCoordinate = new Point(i * LevelBlock.BlockSize.X, j * LevelBlock.BlockSize.Y);

        //            for (int k = 0; k < block.Dimensions.X; k++)
        //            {
        //                for (int l = 0; l < block.Dimensions.Y; l++)
        //                {
        //                    Point p = new Point(levelCoordinate.X + k, levelCoordinate.Y + l);
        //                    level[p.X, p.Y] = block.Values[k, l];
        //                }
        //            }
        //            block.Written = true;
        //        }
        //    }
        //}

        //private void GenerateBlockGrid()
        //{
        //    levelBlockGrid = new LevelBlock[LevelDimensions.X / 3, LevelDimensions.Y / 3];
        //    // Outermost border are only filler blocks
        //    for (int i = 0; i < levelBlockGrid.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < levelBlockGrid.GetLength(1); j++)
        //        {
        //            if (i == 0 || j == 0 || i == levelBlockGrid.GetLength(0) - 1 || j == levelBlockGrid.GetLength(1) - 1)
        //            {
        //                levelBlockGrid[i, j] = fillerBlock.Clone();
        //            }
        //        }
        //    }

        //    // Fill the inside grid with LevelBuildingBlocks
        //    for (int i = 0; i < levelBlockGrid.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < levelBlockGrid.GetLength(1); j++)
        //        {
        //            if(levelBlockGrid[i, j] == null)
        //            {
        //                List<LevelBlock> blockCandidates = GetBlockCandidates(new Point(i, j));
        //                if (blockCandidates.Count > 0)
        //                {
        //                    LevelBlock winner = PickCandidateBasedOnPriority(blockCandidates);
        //                    Place(new Point(i, j), winner);
        //                    foreach(LevelBlock block in levelPieces)
        //                    {
        //                        block.Update();
        //                    }
        //                }
        //                else
        //                {
        //                    levelBlockGrid[i, j] = fillerBlock.Clone();
        //                }
        //            }
        //        }
        //    }
        //}

        //private List<LevelBlock> GetBlockCandidates(Point blockGridCoord)
        //{
        //    List<LevelBlock> res = new List<LevelBlock>();
        //    foreach(LevelBlock block in levelPieces)
        //    {
        //        if(CanPlace(blockGridCoord, block))
        //        {
        //            res.Add(block);
        //        }
        //    }
        //    return res;
        //}

        private void Populate(int[,] arr, int value)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    arr[i, j] = value;
        }

        //private bool CanPlace(Point blockGridCoord, LevelBlock block)
        //{
        //    for(int i = 0; i < block.BlockGridDimensions.X; i++)
        //    {
        //        for(int j = 0; j < block.BlockGridDimensions.Y; j++)
        //        {
        //            Point destBlockGoord = blockGridCoord + new Point(i, j);
        //            if(!ValidBlockGridLocation(destBlockGoord) || levelBlockGrid[destBlockGoord.X, destBlockGoord.Y] != null){
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}
        //private void Place(Point blockGridCoord, LevelBlock block)
        //{
        //    LevelBlock placedBlock = block.Clone();
        //    // Place block
        //    for (int i = 0; i < block.BlockGridDimensions.X; i++)
        //    {
        //        for (int j = 0; j < block.BlockGridDimensions.Y; j++)
        //        {
        //            Point destBlockGoord = blockGridCoord + new Point(i, j);
        //            levelBlockGrid[destBlockGoord.X, destBlockGoord.Y] = placedBlock;
        //        }
        //    }

        //    // Place a border of fillerblocks around the placed block
        //    Point borderStart = new Point(blockGridCoord.X - 1, blockGridCoord.Y - 1);
        //    for (int i = 0; i < block.BlockGridDimensions.X + 2; i++)
        //    {
        //        for (int j = 0; j < block.BlockGridDimensions.Y + 2; j++)
        //        {
        //            Point destBlockGoord = borderStart + new Point(i, j);
        //            if(ValidBlockGridLocation(destBlockGoord) && levelBlockGrid[destBlockGoord.X, destBlockGoord.Y] == null)
        //            {
        //                levelBlockGrid[destBlockGoord.X, destBlockGoord.Y] = fillerBlock.Clone();
        //            }
        //        }
        //    }

        //}
        private bool ValidCoords(Point levelCoord)
        {
            return levelCoord.X >= 0 && levelCoord.X < LevelDimensions.X && levelCoord.Y >= 0 && levelCoord.Y < LevelDimensions.Y;
        }
        private bool ValidateLevel()
        {
            foreach (LevelBlock levelPiece in levelPieces)
            {
                if (!levelPiece.Valid())
                {
                    return false;
                }
            }
            return true;
        }

        private LevelBlock PickCandidateBasedOnPriority(List<LevelBlock> blocks)
        {
            int sum = 0;
            foreach (LevelBlock block in blocks)
            {
                sum += block.Priority;
            }
            int selection = rand.Next(sum);
            sum = 0;
            foreach (LevelBlock block in blocks)
            {
                sum += block.Priority;
                if (selection < sum)
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

        //public void PrintBlockGrid()
        //{
        //    Console.WriteLine("----------------BlockGrid----------------");
        //    for (int i = 0; i < levelBlockGrid.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < levelBlockGrid.GetLength(1); j++)
        //        {
        //            Console.Write(levelBlockGrid[i, j]?.Name + ",");
        //        }
        //        Console.WriteLine();
        //    }
        //    Console.WriteLine("----------------BlockGrid----------------");

        //}
    }
}
