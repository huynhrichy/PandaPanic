using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class Cell
    {
        public int X;
        public int Y;
        public bool visited = false;
        public bool isBlock = false;
        public Block block;
        public int id = 0;

        public Cell(int x, int y)
        {
            // TODO: Complete member initialization
            this.X = x;
            this.Y = y;
        }

        public Cell()
        {
            // TODO: Complete member initialization
        }

        public void setBlock(Block b)
        {
            block = b;
            isBlock = true;
        }
    }
}
