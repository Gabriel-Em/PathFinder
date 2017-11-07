using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
    class MapState
    {
        public Block[,] Map { get; set; }

        // 0 = Empty
        // 1 = Wall
        // 2 = Player
        // 3 = Destination

        public MapState()
        {
            Map = new Block[10, 19];

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    Map[i, j] = new Block(i, j);
        }

        public MapState(MapState map)
        {
            Map = new Block[10, 19];

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    Map[i, j] = new Block(map.getBlock(i, j));
        }

        public void setBlockType(int i, int j, int blockType)  // sets the type of the block at index i,j to blockType
        {
            // types of blocks:
            //
            // 0 - empty
            // 1 - wall
            // 2 - player
            // 3 - destination

            if (blockType == 2)
                clearPlayer();  // since we're setting the currentBlock as the playerBlock we have to clear the old playerBlock (there can only be 1 player at a time)
            if (blockType == 3)
            {
                clearDestination(); // same as above in regard to destinationBlock
                Map[i, j].IsDestination = true;
            }

            Map[i, j].Type = blockType;
        }


        private void clearPlayer()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    if (Map[i, j].Type == 2)
                    {
                        Map[i, j].Type = 0;
                        break;
                    }
        }

        private void clearDestination()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    if (Map[i, j].Type == 3)
                    {
                        Map[i, j].Type = 0;
                        Map[i, j].IsDestination = false;
                        break;
                    }
        }

        public Tuple<int, int> getPlayerBlockCoord()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    if (Map[i, j].Type == 2)
                        return new Tuple<int, int>(i, j);

            return null;
        }

        public Block getPlayerBlock()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    if (Map[i, j].Type == 2)
                        return Map[i, j];
            return null;
        }

        public Tuple<int, int> getDestinationBlockCoord()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    if (Map[i, j].Type == 3)
                        return new Tuple<int, int>(i, j);

            return null;
        }

        public Block getBlock(int i, int j)
        {
            return Map[i, j];
        }

        public Block getAdjacentBlock(Block bl,int side)    // 0 - left, 1 - right, 2 - top, 3 - bottom
        {
            int i = bl.Coord.Item1, j = bl.Coord.Item2;

            switch(side)
            {
                case 0: if (j == 0) return null; else return Map[i, j - 1];
                case 1: if (j == 18) return null; else return Map[i, j + 1];
                case 2: if (i == 0) return null; else return Map[i - 1,j];
                default: if (i == 9) return null; else return Map[i + 1,j];
            }
        }

        public int getManhattanDistance(Tuple<int, int> sourceBlockCoord)
        {
            Tuple<int, int> destCoord = getDestinationBlockCoord();

            List<int> adjacentBlockCosts = new List<int>() { 0, 0, 0, 0 };

            adjacentBlockCosts[0] = sourceBlockCoord.Item1 > 0 ? Map[sourceBlockCoord.Item1 - 1, sourceBlockCoord.Item2].G : 0;
            adjacentBlockCosts[1] = sourceBlockCoord.Item1 < 9 ? Map[sourceBlockCoord.Item1 + 1, sourceBlockCoord.Item2].G : 0;
            adjacentBlockCosts[2] = sourceBlockCoord.Item2 > 0 ? Map[sourceBlockCoord.Item1, sourceBlockCoord.Item2 - 1].G : 0;
            adjacentBlockCosts[3] = sourceBlockCoord.Item2 < 18 ? Map[sourceBlockCoord.Item1, sourceBlockCoord.Item2 + 1].G : 0;

            return Math.Abs(sourceBlockCoord.Item1 - destCoord.Item1) + Math.Abs(sourceBlockCoord.Item2 - destCoord.Item2);

            // replace the above return with the below one for Absolute Shortest Path (with the disadvantage of a longer search time)
            // return adjacentBlockCosts.Min() * (Math.Abs(sourceBlockCoord.Item1 - destCoord.Item1) + Math.Abs(sourceBlockCoord.Item2 - destCoord.Item2));
        }

        public void setBlock(Block block)
        {
            Map[block.Coord.Item1, block.Coord.Item2] = new Block(block);
        }

        public Block getBlockByIndex(int index)
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    if (Map[i, j].Index == index)
                        return Map[i, j];
            return null;
        }

        public void Reset()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    Map[i, j] = new Block(i, j);
        }
    }
}
