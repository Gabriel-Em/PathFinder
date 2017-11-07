using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
    class PathFinder
    {
        public Tuple<List<Block>,List<Block>> A_star(MapState mapState)  
        {
            // A_star will return the shortest path to destination AND a list with all blocks that were visited during the search

            Block currentBlock;
            List<Block> Visited = new List<Block>();
            PriorityQueue ToVisit = new PriorityQueue();

            ToVisit.Enqueue(mapState.getPlayerBlock());

            while (ToVisit.Count > 0)
            {
                currentBlock = ToVisit.Dequeue();
                Visited.Add(currentBlock);

                currentBlock.Marked = true;                 // we mark the current block as a block we stepped on (we visited it)
                mapState.setBlock(currentBlock);            // we update currentBlock on the map
                mapState.setBlockType(currentBlock.Coord.Item1, currentBlock.Coord.Item2, 2);   // we set the currentBlock as the block the player is currently on

                if (currentBlock.IsDestination)
                    return new Tuple<List<Block>, List<Block>>(generatePath(mapState), Visited);
                else
                {
                    List<Block> nonVisitedAdjacentBlocks = getNonVisitedAdjacentBlocks(ref currentBlock, ref mapState);

                    foreach (Block block in nonVisitedAdjacentBlocks)
                        ToVisit.Enqueue(block);
                }
            }

            return null;
        }

        private List<Block> generatePath(MapState mapState) // we generate the shortest path from the destination node through every parent node until the origin
        {
            List<Block> shortestPath = new List<Block>();

            Block currentBlock = mapState.getPlayerBlock();
            shortestPath.Add(new Block(currentBlock));

            while(currentBlock.ParentBlockIndex != -1)
            {
                currentBlock = mapState.getBlockByIndex(currentBlock.ParentBlockIndex);
                shortestPath.Insert(0, new Block(currentBlock));
            }

            return shortestPath;
        }

        private List<Block> getNonVisitedAdjacentBlocks(ref Block currentBlock, ref MapState mapState)
        {
            List<Block> adjacentBlocks = new List<Block>();

            Block adjBlock;

            for (int i = 0; i < 4; i++)
            {
                adjBlock = mapState.getAdjacentBlock(currentBlock, i);  // i = 0 - left adjacent block, 1 - right, 2 - top, 3 - bottom

                if (adjBlock != null)
                    if ((adjBlock.Type == 0 || adjBlock.Type == 3) && !adjBlock.Marked)     // if we can travel to the adjacent block (no walls) and we haven't marked it yet (we haven't stepped on it)
                    {
                        if (adjBlock.G != 0)    // if we have seen this block before but it wasn't marked (we haven't visited it - we haven't stepped on it yet, but it is added to the ToVisit list)
                        {
                            if (adjBlock.G > currentBlock.G + 1)    // if the path (to block) through currentBlock is shorter than the path the block already holds
                            {
                                adjBlock.ParentBlockIndex = currentBlock.Index; // we mark the currentBlock as the parent block
                                adjBlock.G = currentBlock.G + 1;                // we update the path to block
                                adjacentBlocks.Add(new Block(adjBlock));
                            }
                        }
                        else  // if this is a block we never came across before (we haven't visited nor been anywhere near it)
                        {
                            adjBlock.ParentBlockIndex = currentBlock.Index;
                            adjBlock.G = currentBlock.G + 1;
                            adjBlock.H = mapState.getManhattanDistance(adjBlock.Coord);
                            adjacentBlocks.Add(new Block(adjBlock));
                        }
                    }
            }

            return adjacentBlocks;
        }
    }
}
