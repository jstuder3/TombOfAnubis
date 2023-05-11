using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;
using static TombOfAnubis.Character;
using System.Net.Mail;

namespace TombOfAnubis
{
    public class MapGenerator
    {
        public static readonly int MinTileDistanceArtefactAltar = 7;
        public Point MapDimensions { get; set; }

        private int[,] collisionLayer;
        private Map map;
        private List<EntityDescription> entitiyDescriptions;
        private List<Point> positionsToFill;
        private List<MapBlockDescription> mapBlocksDescriptions;
        private Random rand;
        private Dictionary<string, int> placedBlockNames;

        private int numTrappedArtefacts = 0;

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
            placedBlockNames = new Dictionary<string, int>();
            rand = new Random();

        }
        public List<EntityDescription> GenerateMap()
        {
            Console.WriteLine("Level generation started ...");
            int numAttempts = 0;
            int maxAttempts = 500;
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
            PrintPlacedBlockInformation();

            map.CollisionLayer = Flatten(collisionLayer);
            map.TranslateCollisionToBaseLayer();

            return entitiyDescriptions;
        }

        private void ResetMap()
        {
            Populate(collisionLayer, MapBlock.InvalidValue);
            entitiyDescriptions.Clear();
            placedBlockNames.Clear();
            numTrappedArtefacts = 0;
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
            PlaceArtefacts();
            CreateBorder();
            CreateEmptyPaddingNearBorder();
            int roundsWithoutPlacement = 0;
            int maxRoundsWithoutPlacement = 4 * MapDimensions.X * MapDimensions.Y;
            int placedBlocks = 0;
            while (positionsToFill.Count != 0)
            {
                Point coord = SelectRandomPosition();
                MapBlock candidate = GetCandidate(coord);
                if (candidate == null)
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
                    Place(candidate, coord);
                    placedBlocks++;
                    CreateBorder(coord, candidate);
                    UpdateLevelPieces(candidate);
                }

            }
        }
        private void CreateBorder()
        {
            for (int i = 0; i < MapDimensions.X; i += MapBlock.Empty.Dimensions.X)
            {
                for (int j = 0; j < MapDimensions.Y; j += MapBlock.Empty.Dimensions.Y)
                {
                    if (i == 0 || i == MapDimensions.X - MapBlock.Wall.Dimensions.X || j == 0 || j == MapDimensions.Y - MapBlock.Wall.Dimensions.Y)
                    {
                        if (CanPlace(MapBlock.Wall, new Point(j, i)))
                        {
                            Place(MapBlock.Wall, new Point(j, i));
                        }
                    }

                }
            }
        }
        private void CreateEmptyPaddingNearBorder()
        {
            for (int i = 0; i < MapDimensions.X; i += MapBlock.Empty.Dimensions.X)
            {
                for (int j = 0; j < MapDimensions.Y; j += MapBlock.Empty.Dimensions.Y)
                {
                    if (i == MapBlock.Empty.Dimensions.X ||
                        i == MapDimensions.X - MapBlock.Empty.Dimensions.X - MapBlock.Wall.Dimensions.X ||
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
        private void PlaceArtefacts()
        {
            List<MapBlock> artefactBlocks = new List<MapBlock>();
            foreach(var mbd in mapBlocksDescriptions)
            {
                foreach (var block in mbd.Blocks)
                {
                    if(MapBlockContainsEntity(block, typeof(Artefact)))
                    {
                        artefactBlocks.Add(block);
                    }
                }
            }
            int nPlaced = 0;
            int nRounds = 0;
            int maxRounds = 500;

            while(nPlaced < Session.GetInstance().NumberOfPlayers && nRounds < maxRounds) 
            {
                nRounds++;
                MapBlock candidate = artefactBlocks[rand.Next(artefactBlocks.Count)];
                List<Point> locations = FindLocationsAtBorder(candidate);
                if(locations.Count != 0)
                {
                    Point dst = locations[rand.Next(locations.Count)];
                    Place(candidate, dst);
                    CreateBorder(dst, candidate);
                    UpdateLevelPieces(candidate);

                    nPlaced++;
                }
            }
        }

        private List<Point> FindLocationsAtBorder(MapBlock block)
        {
            int w = block.Dimensions.X;
            int h = block.Dimensions.Y;
            List<Point> locations = new List<Point>();

            // Left and right border
            for (int x = w; x < MapDimensions.X - w; x++)
            {
                Point candidateLeft = new Point(x, 0);
                Point candidateRight = new Point(x, MapDimensions.Y - h);
                if (CanPlace(block, candidateLeft) && !block.HasLeftDoor())
                {
                    locations.Add(candidateLeft);
                }
                if (CanPlace(block, candidateRight) && !block.HasRightDoor())
                {
                    locations.Add(candidateRight);
                }
            }
            // Top and bottom border
            for (int y = h; y < MapDimensions.Y - h; y++)
            {
                Point candidateTop = new Point(0, y);
                Point candidateBottom = new Point(MapDimensions.X - w, y);
                if (CanPlace(block, candidateTop) && !block.HasTopDoor())
                {
                    locations.Add(candidateTop);
                }
                if (CanPlace(block, candidateBottom) && !block.HasBottomDoor())
                {
                    locations.Add(candidateBottom);
                }
            }

            // Cornder cases
            Point topLeft = new Point(0, 0);
            Point topRight = new Point(MapDimensions.X - w, 0);
            Point bottomLeft = new Point(0, MapDimensions.Y - h);
            Point bottomRight = new Point(MapDimensions.X - w, MapDimensions.Y - h);

            if(CanPlace(block, topLeft) && !block.HasTopDoor() && !block.HasLeftDoor())
            {
                locations.Add(topLeft);
            }
            if (CanPlace(block, topRight) && !block.HasTopDoor() && !block.HasRightDoor())
            {
                locations.Add(topRight);
            }
            if (CanPlace(block, bottomLeft) && !block.HasBottomDoor() && !block.HasLeftDoor())
            {
                locations.Add(bottomLeft);
            }
            if (CanPlace(block, bottomRight) && !block.HasBottomDoor() && !block.HasRightDoor())
            {
                locations.Add(bottomRight);
            }
            return locations;

        }
        private MapBlock GetCandidate(Point coord)
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
            if(candidates.Count == 0)
            {
                return null;
            }
            List<MapBlock> validCandidates = new List<MapBlock>();
            List<MapBlock> invalidCandidates = new List<MapBlock>();
            foreach (var candidate in candidates)
            {
                if (candidate.Parent.Valid())
                {
                    validCandidates.Add(candidate);
                }
                else
                {
                    invalidCandidates.Add(candidate);
                }
            }
            if(invalidCandidates.Count > 0)
            {
                return PickCandidateBasedOnSize(invalidCandidates);
            }
            else if(!HasInvalidBlockDescriptions() && validCandidates.Count > 0)
            {
                return PickCandidateBasedOnPriority(validCandidates);
            }
            else
            {
                return null;
            }

        }

        private Point SelectRandomPosition()
        {
            return positionsToFill[rand.Next(positionsToFill.Count)];
        }

        private bool CanPlace(MapBlock block, Point coord)
        {
            //if(coord.X == 0 && block.HasLeftDoor() || coord.X == MapDimensions.X - block.Dimensions.X)dd
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
            if(block.Name != null && placedBlockNames.ContainsKey(block.Name))
            {
                placedBlockNames[block.Name]++;
            }
            else if(block.Name != null)
            {
                placedBlockNames[block.Name] = 1;
            }
            if(MapBlockContainsEntity(block, typeof(Artefact)) && MapBlockContainsEntity(block, typeof(Trap)))
            {
                numTrappedArtefacts++;
            }
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
        private bool HasInvalidBlockDescriptions()
        {
            foreach(MapBlockDescription mapBlockDescription in mapBlocksDescriptions) 
            {
                if (!mapBlockDescription.Valid())
                {
                    return true;
                }
            }
            return false;
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
            //Console.WriteLine("-------LevelCandidate-------");
            //PrintPlacedBlockInformation();
            bool validMapBlockDescs = ValidateMapBlockDescriptions();
            //if(!validMapBlockDescs) { Console.WriteLine("Map block validation failed."); }
            bool trappedArtefactsAppear = numTrappedArtefacts > 0;
            //if (!trappedArtefactsAppear) { Console.WriteLine("Not enough trapped artefacts: "+numTrappedArtefacts); }

            //bool artefactMinDistance = ValidateArtefactMinDistanceToAltar();

            //bool altarNotInCorner = ValidateAltarNotInCorner();
            //if (!artefactMinDistance) { Console.WriteLine("Min Distance check failed"); }
            //Console.WriteLine("-------EndLevelCandidate-------");

            return validMapBlockDescs && trappedArtefactsAppear;
        }

        private bool ValidateAltarNotInCorner()
        {
            int border = 7;
            float chance = 0.0f;
            EntityDescription altar = null;
            foreach (EntityDescription ed in entitiyDescriptions)
            {
                if (Type.GetType(ed.ClassName) == typeof(Altar))
                {
                    altar = ed;
                }
            }
            if(altar == null ) { return false; }
            Point altarCoord = altar.SpawnTileCoordinate;

            Point up = altarCoord + new Point(0, border);
            Point down = altarCoord + new Point(0, -border);
            Point left = altarCoord + new Point(-border, 0);
            Point right = altarCoord + new Point(border, 0);

            if ((ValidCoords(up) && ValidCoords(down) && ValidCoords(right)&& ValidCoords(left)))
            {
                return true;
            }
            return false;
        }

        private bool ValidateArtefactMinDistanceToAltar()
        {
            EntityDescription altar = null;
            List<EntityDescription> artefacts = new List<EntityDescription>();
            foreach(EntityDescription ed in entitiyDescriptions)
            {
                if(Type.GetType(ed.ClassName) == typeof(Altar))
                {
                    altar = ed;
                }
                if(Type.GetType(ed.ClassName) == typeof(Artefact))
                {
                    artefacts.Add(ed);
                }
            }
            if(altar == null || artefacts.Count == 0) { return false; }

            Point altarCoord = altar.SpawnTileCoordinate;
            foreach(EntityDescription artefact in artefacts)
            {
                Point artefactCoord = artefact.SpawnTileCoordinate;
                Point diff = artefactCoord - altarCoord;
                Vector2 diffv = new Vector2 (diff.X, diff.Y);
                if (diffv.Length() < MinTileDistanceArtefactAltar)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidateMapBlockDescriptions()
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

        private MapBlock PickCandidateBasedOnSize(List<MapBlock> blocks)
        {
            //PrintBlockList(blocks);
            int sum = 0;
            foreach (MapBlock block in blocks)
            {
                sum += block.Dimensions.X * block.Dimensions.Y;
            }
            int selection = rand.Next(sum);
            sum = 0;
            foreach (MapBlock block in blocks)
            {
                sum += block.Dimensions.X * block.Dimensions.Y;
                if (selection < sum)
                {
                    //Console.WriteLine("Winner: " + block.Name);
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
        public bool MapBlockContainsEntity(MapBlock block, Type entityType)
        {
            if (block.Entities == null)
            {
                return false;
            }
            foreach (var entity in block.Entities)
            {
                Type t = Type.GetType(entity.ClassName);
                if (t == entityType)
                {
                    return true;
                }
            }
            return false;
        }

        public void PrintLevel()
        {
            EntityDescription altar = null;
            List<Point> artefacts = new List<Point>();
            foreach (EntityDescription ed in entitiyDescriptions)
            {
                if (Type.GetType(ed.ClassName) == typeof(Altar))
                {
                    altar = ed;
                }
                if (Type.GetType(ed.ClassName) == typeof(Artefact))
                {
                    artefacts.Add(ed.SpawnTileCoordinate);
                }
            }
            Console.WriteLine("----------------Level----------------");
            for (int i = 0; i < collisionLayer.GetLength(0); i++)
            {
                for (int j = 0; j < collisionLayer.GetLength(1); j++)
                {
                    if(altar != null && new Point(j, i) == altar.SpawnTileCoordinate)
                    {
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("A");
                        Console.ForegroundColor = color;
                        Console.Write(",");
                    }
                    else if(artefacts.Contains(new Point(j, i)))
                    {
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("B");
                        Console.ForegroundColor = color;
                        Console.Write(",");
                    }
                    else
                    {
                        Console.Write(collisionLayer[i, j] + ",");
                    }
                }
                Console.Write("\n");
            }
            Console.WriteLine("----------------Level----------------");

        }
        public void PrintPlacedBlockInformation()
        {
            foreach(var k in placedBlockNames.Keys)
            {
                Console.WriteLine(placedBlockNames[k] + " " + k);
            }
        }

        public void PrintBlockList(List<MapBlock> blocks)
        {
            Console.Write("[ ");
            foreach (var block in blocks)
            {
                Console.Write(block.ToString()+", ");
            }
            Console.Write(" ]");
            //Console.WriteLine();
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
