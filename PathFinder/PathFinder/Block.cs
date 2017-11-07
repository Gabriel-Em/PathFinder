using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
    class Block
    {
        public bool Marked { get; set; }
        public Tuple<int,int> Coord { get; set; }
        public int ParentBlockIndex { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F { get { return G + H; } }
        public int Type { get; set; }
        public bool IsDestination { get; set; }
        public int Index { get; set; }

        // 0 = Empty
        // 1 = Wall
        // 2 = Player
        // 3 = Destination

        public Block(int i,int j)
        {
            Marked = false;
            Coord = new Tuple<int, int>(i, j);
            ParentBlockIndex = -1;
            Index = i * 19 + j + 1;
            G = 0;
            H = 0;
            Type = 0;
            IsDestination = false;
        }

        public Block(Block block)
        {
            Marked = block.Marked;
            Coord = new Tuple<int, int>(block.Coord.Item1, block.Coord.Item2);
            ParentBlockIndex = block.ParentBlockIndex;
            Index = block.Index;
            G = block.G;
            H = block.H;
            Type = block.Type;
            IsDestination = block.IsDestination;
        }
    }
}
