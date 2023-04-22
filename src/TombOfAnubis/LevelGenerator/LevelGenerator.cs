using Microsoft.Xna.Framework;
using QuikGraph;
using QuikGraph.Algorithms;
using System;
using System.Collections.Generic;
using static System.Reflection.Metadata.BlobBuilder;

namespace TombOfAnubis
{
    public class LevelGenerator
    {

        // Needs to be dividable by 3
        public Point LevelDimensions { get; set; }

        private int[,] level;
        private List<Point> positionsToFill;
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
            positionsToFill = new List<Point>();

            for (int i = 0; i < LevelDimensions.X; i++)
            {
                for(int j = 0;  j < LevelDimensions.Y; j++)
                {
                    positionsToFill.Add(new Point(i, j));
                }
            }

            this.levelPieces = levelPieces;

            this.levelPieces.Add(new LevelBlock(new int[,] { 
                { 1, 0, 1 ,1}, 
                { 1, 0, 1, 1}, 
                { 0, 0, 0, 1},
                { 1, 1, 0, 1}}, 2000, "zero"));
            this.levelPieces.Add(new LevelBlock(new int[,] {
                { 1, 1, 1 ,1},
                { 1, 1, 1, 1},
                { 1, 1, 1, 1},
                { 1, 1, 1, 1}}, 2, "one"));
            this.levelPieces.Add(new LevelBlock(new int[,] {
                { 4, 4, 4},
                { 4, 4, 4},
                { 4, 4, 4}}, 2, 4, int.MaxValue));
            this.levelPieces.Add(new LevelBlock(new int[,] {
                { 1, 0, 1 ,1},
                { 1, 88, 0, 0},
                { 0, 0, 0, 1},
                { 1, 0, 1, 1}}, 1, 1, 1));

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
                for (int i = 0; i < LevelDimensions.X; i++)
                {
                    for (int j = 0; j < LevelDimensions.Y; j++)
                    {
                        positionsToFill.Add(new Point(i, j));
                    }
                }
            }
            Console.WriteLine("Num Attempts: " + numAttempts);

            //CreatGraphFromLevel();
            PrintLevel();

            return level;
        }

        private void CreateLevel()
        {
            CreateBorder();
            int roundsWithoutPlacement = 0;
            int maxRoundsWithoutPlacement = 2 * LevelDimensions.X * LevelDimensions.Y;
            int placedBlocks = 0;
            while (positionsToFill.Count != 0)
            {
                Point coord = SelectRandomPosition();
                List<LevelBlock> candidates = GetCandidates(coord);
                if (candidates.Count == 0)
                {
                    if (roundsWithoutPlacement >= maxRoundsWithoutPlacement && CanPlace(LevelBlock.Empty, coord))
                    {
                        //PrintLevel();
                        Place(LevelBlock.Empty, coord);
                    }
                    else
                    {
                        roundsWithoutPlacement++;
                        continue;
                    }
                }
                else
                {
                    LevelBlock winner;
                    candidates.Sort(CompareCandidates);
                    if (!candidates[0].Valid())
                    {
                        winner = candidates[0];
                    }
                    else
                    {
                        winner = PickCandidateBasedOnPriority(candidates);
                    }
                    Place(winner, coord);
                    placedBlocks++;
                    CreateBorder(coord, winner);
                    UpdateLevelPieces(winner);
                }

            }
        }
        private void CreateBorder()
        {
            for(int i = 0; i < LevelDimensions.X; i += LevelBlock.Empty.Dimensions.X)
            {
                for(int j =0; j < LevelDimensions.Y; j += LevelBlock.Empty.Dimensions.Y)
                {
                    if(i == 0 || i == LevelDimensions.X - LevelBlock.Empty.Dimensions.X || j == 0 || j == LevelDimensions.Y - LevelBlock.Empty.Dimensions.Y)
                    {
                        if(CanPlace(LevelBlock.Empty, new Point(j, i)))
                        {
                            Place(LevelBlock.Empty, new Point(j, i));
                        }
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
            int emptyW = LevelBlock.Empty.Dimensions.X;
            int emptyH = LevelBlock.Empty.Dimensions.Y;
            for (int i = x - emptyW; i < x + w + emptyW; i++)
            {
                for (int j = y - emptyH; j < y + h + emptyH; j++)
                {
                    if (CanPlace(LevelBlock.Empty, new Point(i, j)))
                    {
                        Place(LevelBlock.Empty, new Point(i, j));
                    }
                }
            }
        }

        private List<LevelBlock> GetCandidates(Point coord)
        {
            List<LevelBlock> candidates = new List<LevelBlock>();
            foreach (LevelBlock piece in levelPieces) 
            {
                if(CanPlace(piece, coord) && piece.Occurences < piece.MaxOccurences)
                {
                    candidates.Add(piece);
                }
            }
            return candidates;
        }

        private Point SelectRandomPosition()
        {
            return positionsToFill[rand.Next(positionsToFill.Count)];
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
                        positionsToFill.Remove(dest);
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

        private void Populate(int[,] arr, int value)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    arr[i, j] = value;
        }
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

        /// <summary>
        /// Invalid blocks have highest priority
        /// </summary>
        private int CompareCandidates(LevelBlock b1, LevelBlock b2)
        {
            if (!b1.Valid() && b2.Valid())
            {
                return -1;
            }
            else if (b1.Valid() && !b2.Valid())
            {
                return 1;
            }
            else
            {
                return 0;
            }

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
    }
}
