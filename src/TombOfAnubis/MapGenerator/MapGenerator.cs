﻿using Microsoft.Xna.Framework;
using QuikGraph;
using System;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class MapGenerator
    {

        public Point LevelDimensions { get; set; }

        private int[,] level;
        private Map map;
        private List<EntityDescription> entitiyDescriptions;
        private List<Point> positionsToFill;
        private List<MapBlock> mapBlocks;
        private Random rand;
        private int numPlayers;

        public MapGenerator(Map map, int numPlayers)
        {
            this.map = map;
            this.numPlayers = numPlayers;
            LevelDimensions = map.MapDimensions;
            level = new int[LevelDimensions.X, LevelDimensions.Y];
            Populate(level, MapBlock.InvalidValue);
            positionsToFill = new List<Point>();

            for (int i = 0; i < LevelDimensions.X; i++)
            {
                for(int j = 0;  j < LevelDimensions.Y; j++)
                {
                    positionsToFill.Add(new Point(i, j));
                }
            }

            mapBlocks = map.MapBlocks;
            entitiyDescriptions = new List<EntityDescription>();

            rand = new Random();

        }
        public List<EntityDescription> GenerateMap()
        {
            int numAttempts = 0;
            int maxAttempts = 50;
            while(numAttempts < maxAttempts)
            {
                numAttempts++;
                CreateLevel();
                if (ValidateLevel())
                {
                    PrintLevel();
                    MapGraph levelGraph = new MapGraph(level);
                    if (levelGraph.ConnectLevelBlocks())
                    {
                        break;
                    }
                }
                ResetLevel();
               
            }
            PrintLevel();

            Console.WriteLine("Num Attempts: " + (numAttempts));
            map.CollisionLayer = Flatten(level);
            map.TranslateCollisionToBaseLayer();

            return entitiyDescriptions;
        }

        private void ResetLevel()
        {
            Populate(level, MapBlock.InvalidValue);
            foreach (MapBlock piece in mapBlocks)
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
        private void CreateLevel()
        {
            CreateBorder();
            int roundsWithoutPlacement = 0;
            int maxRoundsWithoutPlacement = 2 * LevelDimensions.X * LevelDimensions.Y;
            int placedBlocks = 0;
            while (positionsToFill.Count != 0)
            {
                Point coord = SelectRandomPosition();
                List<MapBlock> candidates = GetCandidates(coord);
                if (candidates.Count == 0)
                {
                    if (roundsWithoutPlacement >= maxRoundsWithoutPlacement && CanPlace(MapBlock.Empty, coord))
                    {
                        //PrintLevel();
                        Place(MapBlock.Empty, coord);
                    }
                    else
                    {
                        roundsWithoutPlacement++;
                        continue;
                    }
                }
                else
                {
                    MapBlock winner;
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
            for(int i = 0; i < LevelDimensions.X; i += MapBlock.Empty.Dimensions.X)
            {
                for(int j =0; j < LevelDimensions.Y; j += MapBlock.Empty.Dimensions.Y)
                {
                    if(i == 0 || i == LevelDimensions.X - MapBlock.Empty.Dimensions.X || j == 0 || j == LevelDimensions.Y - MapBlock.Empty.Dimensions.Y)
                    {
                        if(CanPlace(MapBlock.Empty, new Point(j, i)))
                        {
                            Place(MapBlock.Empty, new Point(j, i));
                        }
                    }
                }
            }
        }
        private void CreateBorder(Point coord, MapBlock piece)
        {
            int w = piece.Dimensions.X;
            int h = piece.Dimensions.Y;
            int x = coord.X;
            int y = coord.Y;
            int emptyW = MapBlock.Empty.Dimensions.X;
            int emptyH = MapBlock.Empty.Dimensions.Y;
            for (int i = x - emptyW; i < x + w + emptyW; i++)
            {
                for (int j = y - emptyH; j < y + h + emptyH; j++)
                {
                    if (CanPlace(MapBlock.Empty, new Point(i, j)))
                    {
                        Place(MapBlock.Empty, new Point(i, j));
                    }
                }
            }
        }

        private List<MapBlock> GetCandidates(Point coord)
        {
            List<MapBlock> candidates = new List<MapBlock>();
            foreach (MapBlock piece in mapBlocks) 
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

        private bool CanPlace(MapBlock piece, Point coord)
        {
            for (int i = 0; i < piece.Dimensions.X; i++)
            {
                for (int j = 0; j < piece.Dimensions.Y; j++)
                {
                    Point dst = coord + new Point(i, j);
                    if (!ValidCoords(dst) || level[dst.X, dst.Y] != MapBlock.InvalidValue)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void UpdateLevelPieces(MapBlock winner)
        {
            foreach (MapBlock piece in mapBlocks)
            {
                piece.Update(winner.Equals(piece));
            }
        }
        private void Place(MapBlock levelPiece, Point coord)
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
            if(levelPiece.Entities != null)
            {
                foreach (EntityDescription entityDescription in levelPiece.Entities)
                {
                    EntityDescription ed = entityDescription.Clone();
                    ed.SpawnTileCoordinate += coord;
                    ed.SpawnTileCoordinate = new Point(ed.SpawnTileCoordinate.Y, ed.SpawnTileCoordinate.X);
                    entitiyDescriptions.Add(ed);
                }
            }
        }

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
            foreach (MapBlock levelPiece in mapBlocks)
            {
                if (!levelPiece.Valid())
                {
                    return false;
                }
            }
            return true;
        }
        private MapBlock PickCandidateBasedOnPriority(List<MapBlock> blocks)
        {
            int sum = 0;
            foreach (MapBlock block in blocks)
            {
                sum += block.Priority;
            }
            int selection = rand.Next(sum);
            sum = 0;
            foreach (MapBlock block in blocks)
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
        private int CompareCandidates(MapBlock b1, MapBlock b2)
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

        private static int[] Flatten(int[,] input)
        {
            int size = input.Length;
            int[] result = new int[size];

            int write = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                for (int z = 0; z <= input.GetUpperBound(1); z++)
                {
                    result[write++] = input[i, z];
                }
            }
            return result;
        }
    }
}
