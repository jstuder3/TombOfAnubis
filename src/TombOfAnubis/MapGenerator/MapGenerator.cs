using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static TombOfAnubis.Character;

namespace TombOfAnubis
{
    public class MapGenerator
    {

        public Point MapDimensions { get; set; }

        private int[,] collisionLayer;
        private Map map;
        private List<EntityDescription> entitiyDescriptions;
        private List<Point> positionsToFill;
        private List<MapBlockDescription> mapBlocksDescriptions;
        private Random rand;

        public MapGenerator(Map map)
        {
            this.map = map;
            MapDimensions = map.MapDimensions;
            collisionLayer = new int[MapDimensions.X, MapDimensions.Y];
            Populate(collisionLayer, MapBlock.InvalidValue);
            positionsToFill = new List<Point>();

            for (int i = 0; i < MapDimensions.X; i++)
            {
                for(int j = 0;  j < MapDimensions.Y; j++)
                {
                    positionsToFill.Add(new Point(i, j));
                }
            }

            mapBlocksDescriptions = map.MapBlockDescriptions;
            entitiyDescriptions = new List<EntityDescription>();

            rand = new Random();

        }
        public List<EntityDescription> GenerateMap()
        {
            Console.WriteLine("Level generation started ...");
            int numAttempts = 0;
            int maxAttempts = 50;
            while(numAttempts < maxAttempts)
            {
                numAttempts++;
                Createmap();
                if (ValidateMap())
                {
                    MapGraph mapGraph = new MapGraph(collisionLayer);
                    if (mapGraph.ConnectLevelBlocks())
                    {
                        break;
                    }
                }
                ResetMap();
               
            }
            if(numAttempts < maxAttempts)
            {
                Console.WriteLine("Generated level in " + (numAttempts) + " attempt(s).");
            }
            else
            {
                Console.WriteLine("Level generation failed.");
            }
            PrintLevel();

            map.CollisionLayer = Flatten(collisionLayer);
            map.TranslateCollisionToBaseLayer();

            return entitiyDescriptions;
        }

        private void ResetMap()
        {
            Populate(collisionLayer, MapBlock.InvalidValue);
            entitiyDescriptions.Clear();
            foreach (MapBlockDescription desc in mapBlocksDescriptions)
            {
                desc.Reset();
            }
            for (int i = 0; i < MapDimensions.X; i++)
            {
                for (int j = 0; j < MapDimensions.Y; j++)
                {
                    positionsToFill.Add(new Point(i, j));
                }
            }
        }
        private void Createmap()
        {
            CreateBorder();
            int roundsWithoutPlacement = 0;
            int maxRoundsWithoutPlacement = 2 * MapDimensions.X * MapDimensions.Y;
            int placedBlocks = 0;
            while (positionsToFill.Count != 0)
            {
                Point coord = SelectRandomPosition();
                List<MapBlock> candidates = GetCandidates(coord);
                if (candidates.Count == 0)
                {
                    if (roundsWithoutPlacement >= maxRoundsWithoutPlacement && CanPlace(MapBlock.Empty, coord))
                    {
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
                    if (!candidates[0].Parent.Valid())
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
            for(int i = 0; i < MapDimensions.X; i += MapBlock.Empty.Dimensions.X)
            {
                for(int j =0; j < MapDimensions.Y; j += MapBlock.Empty.Dimensions.Y)
                {
                    if(i == 0 || i == MapDimensions.X - MapBlock.Wall.Dimensions.X || j == 0 || j == MapDimensions.Y - MapBlock.Wall.Dimensions.Y)
                    {
                        if(CanPlace(MapBlock.Wall, new Point(j, i)))
                        {
                            Place(MapBlock.Wall, new Point(j, i));
                        }
                    }
                    if (i == MapBlock.Empty.Dimensions.X || 
                        i == MapDimensions.X - MapBlock.Empty.Dimensions.X - MapBlock.Wall.Dimensions.X  || 
                        j == MapBlock.Empty.Dimensions.Y || 
                        j == MapDimensions.Y - MapBlock.Empty.Dimensions.Y - MapBlock.Wall.Dimensions.Y)
                    {
                        if (CanPlace(MapBlock.Empty, new Point(j, i)))
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
            foreach(MapBlockDescription mapBlockDescription in mapBlocksDescriptions)
            {
                foreach (MapBlock piece in mapBlockDescription.Blocks)
                {
                    if (CanPlace(piece, coord) && mapBlockDescription.Occurences < mapBlockDescription.MaxOccurences)
                    {
                        candidates.Add(piece);
                    }
                }
            }
            
            return candidates;
        }

        private Point SelectRandomPosition()
        {
            return positionsToFill[rand.Next(positionsToFill.Count)];
        }

        private bool CanPlace(MapBlock block, Point coord)
        {
            for (int i = 0; i < block.Dimensions.X; i++)
            {
                for (int j = 0; j < block.Dimensions.Y; j++)
                {
                    Point dst = coord + new Point(i, j);
                    if (!ValidCoords(dst) || collisionLayer[dst.X, dst.Y] != MapBlock.InvalidValue)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void UpdateLevelPieces(MapBlock winner)
        {
            foreach (MapBlockDescription block in mapBlocksDescriptions)
            {
                block.Update(block.Blocks.Contains(winner));
            }
        }
        private void Place(MapBlock block, Point coord)
        {
            for (int i = 0; i < block.Dimensions.X; i++)
            {
                for(int j = 0; j < block.Dimensions.Y; j++)
                {
                    Point dest = coord + new Point(i, j);
                    if (ValidCoords(dest))
                    {
                        collisionLayer[dest.X, dest.Y] = block.GetValue(new Point(i, j));
                        positionsToFill.Remove(dest);
                    }
                }
            }
            if(block.Entities != null)
            {
                foreach (EntityDescription entityDescription in block.Entities)
                {
                    EntityDescription ed = entityDescription.Clone();
                    ed.SpawnTileCoordinate += coord;
                    ed.SpawnTileCoordinate = new Point(ed.SpawnTileCoordinate.Y, ed.SpawnTileCoordinate.X);
                    Type t = Type.GetType(entityDescription.ClassName);
                    if (t == typeof(Artefact))
                    {
                        ed.Type = Enum.GetNames(typeof(CharacterType))[block.Parent.Occurences];
                    }
                    if(t == typeof(Button))
                    {
                        List<EntityDescription> connectedTraps = new List<EntityDescription>();
                        foreach(EntityDescription connectedTrap in entityDescription.ConnectedTrapPositions)
                        {
                            EntityDescription con = connectedTrap.Clone();
                            con.SpawnTileCoordinate += coord;
                            con.SpawnTileCoordinate = new Point(con.SpawnTileCoordinate.Y, con.SpawnTileCoordinate.X);
                            connectedTraps.Add(con);
                        }
                        ed.ConnectedTrapPositions = connectedTraps;
                    }
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
            return levelCoord.X >= 0 && levelCoord.X < MapDimensions.X && levelCoord.Y >= 0 && levelCoord.Y < MapDimensions.Y;
        }
        private bool ValidateMap()
        {
            foreach (MapBlockDescription mapBlockDescription in mapBlocksDescriptions)
            {
                if (!mapBlockDescription.Valid())
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
                sum += block.Parent.Priority;
            }
            int selection = rand.Next(sum);
            sum = 0;
            foreach (MapBlock block in blocks)
            {
                sum += block.Parent.Priority;
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
            if (!b1.Parent.Valid() && b2.Parent.Valid())
            {
                return -1;
            }
            else if (b1.Parent.Valid() && !b2.Parent.Valid())
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
            for (int i = 0; i < collisionLayer.GetLength(0); i++)
            {
                for (int j = 0; j < collisionLayer.GetLength(1); j++)
                {
                    Console.Write(collisionLayer[i, j] + ",");
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
