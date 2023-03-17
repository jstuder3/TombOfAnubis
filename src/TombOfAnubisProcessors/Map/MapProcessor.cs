using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubis;

namespace TombOfAnubis
{
    [ContentProcessor(DisplayName = "Map Processor")]
    public class MapProcessor : ContentProcessor<Map, Map>
    {
        public override Map Process(Map input, ContentProcessorContext context)
        {
            input.BaseLayer = new int[input.CollisionLayer.Length];
            int mapDimensionsX = input.MapDimensions.X;
            int mapDimensionsY = input.MapDimensions.Y;
            for (int x = 0; x < mapDimensionsX; x++)
            {
                for(int y = 0;  y < mapDimensionsY; y++)
                {
                    if( x == 0 && y==0) // Top left
                    {
                        input.BaseLayer[y * mapDimensionsX + x] = 15;
                    }else if( x == mapDimensionsX - 1 && y == 0) // Top right
                    {
                        input.BaseLayer[y * mapDimensionsX + x] = 15;
                    }
                    else if (x == 0 && y == mapDimensionsY - 1) // Bottom left
                    {
                        input.BaseLayer[y * mapDimensionsX + x] = 15;
                    }
                    else if (x == mapDimensionsX - 1 && y == mapDimensionsY - 1) // Bottom right
                    {
                        input.BaseLayer[y * mapDimensionsX + x] = 15;
                    }
                    else if (x == 0) // Top border
                    {
                        input.BaseLayer[y * mapDimensionsX + x] = 13;
                    }
                    else if (x == mapDimensionsX - 1) // Bottom border
                    {
                        input.BaseLayer[y * mapDimensionsX + x] = 14;
                    }
                    else if (y == 0) //Left border
                    {
                        input.BaseLayer[y * mapDimensionsX + x] = 11;
                    }
                    else if (y == mapDimensionsY - 1) // Right border
                    {
                        input.BaseLayer[y * mapDimensionsX + x] = 12;
                    }
                    else
                    {
                        bool center = input.CollisionLayer[y * mapDimensionsX + x] != 0;
                        bool up = input.CollisionLayer[(y - 1) * mapDimensionsX + x] != 0;
                        bool down = input.CollisionLayer[(y + 1) * mapDimensionsX + x] != 0;
                        bool left = input.CollisionLayer[y * mapDimensionsX + x - 1] != 0;
                        bool right = input.CollisionLayer[y * mapDimensionsX + x + 1] != 0;

                        if (center)
                        {
                            if (!up & !down & !left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 0;
                            }
                            if (up & !down & !left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 1;
                            }
                            if (!up & down & !left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 2;
                            }
                            if (!up & !down & left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 3;
                            }
                            if (!up & !down & !left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 4;
                            }
                            if (up & down & !left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 5;
                            }
                            if (up & !down & left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 6;
                            }
                            if (up & !down & !left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 7;
                            }
                            if (!up & down & left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 8;
                            }
                            if (!up & down & !left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 9;
                            }
                            if (!up & !down & left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 10;
                            }
                            if (up & down & left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 11;
                            }
                            if (up & down & !left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 12;
                            }
                            if (up & !down & left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 13;
                            }
                            if (!up & down & left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 14;
                            }
                            if (up & down & left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 15;
                            }
                        }
                        else
                        {
                            if (!up & !down & !left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 16;
                            }
                            if (up & !down & !left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 17;
                            }
                            if (!up & down & !left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 18;
                            }
                            if (!up & !down & left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 19;
                            }
                            if (!up & !down & !left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 20;
                            }
                            if (up & down & !left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 21;
                            }
                            if (up & !down & left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 22;
                            }
                            if (up & !down & !left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 23;
                            }
                            if (!up & down & left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 24;
                            }
                            if (!up & down & !left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 25;
                            }
                            if (!up & !down & left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 26;
                            }
                            if (up & down & left & !right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 27;
                            }
                            if (up & down & !left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 28;
                            }
                            if (up & !down & left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 29;
                            }
                            if (!up & down & left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 30;
                            }
                            if (up & down & left & right)
                            {
                                input.BaseLayer[y * mapDimensionsX + x] = 31;
                            }
                        }
                    }
                }
            }
            
            return input;
        }
    }
}
