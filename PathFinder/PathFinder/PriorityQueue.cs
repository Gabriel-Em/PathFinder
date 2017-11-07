using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
    class PriorityQueue
    {
        private List<Block> queue;

        public int Count { get { return queue.Count(); } }

        public List<Block> toList()
        {
            return queue.ToList();
        }

        public PriorityQueue()
        {
            queue = new List<Block>();
        }

        private void EliminateBadEstimate(Block block)
        {
            for (int i = 0; i < queue.Count(); i++)
                if (block.Index == queue[i].Index)
                {
                    queue.RemoveAt(i);
                    break;
                }
        }

        public void Enqueue(Block block)
        {
            EliminateBadEstimate(block);

            bool wasAdded = false;

            for (int i = 0; i < queue.Count(); i++)
                if (block.F <= queue[i].F)
                {
                    queue.Insert(i, block);

                    wasAdded = true;
                    break;
                }

            if (!wasAdded)
                queue.Add(block);
        }

        public Block Dequeue()
        {
            Block block = queue[0];
            queue.RemoveAt(0);

            return block;
        }
    }
}
