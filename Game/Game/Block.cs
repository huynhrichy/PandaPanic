using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game
{
    class Block : Unmoveable
    {
        public Vector2 Apos;
        public Block parent;
        public Block child;
        public Cell cell;// Corresponding Cell
        public int loc; // relative location of block to the parent: from 0 to 4 , 0 for root 

        public Block(Texture2D text, Vector2 pos, Block p, int l)
            : base(text,pos)
        {
            Apos = pos;
            parent = p;
            loc = l;
            Bound = new Rectangle((int)Position.X, (int)Position.Y, 30, 30);
            // width 30 
        }

        public void SetChild(Block c)
        {
            child = c;
        }

        public void SetParent(Block p)
        {
            parent = p;
        }
        
        public override void update()
        {
            base.update();
        }
    }
}
