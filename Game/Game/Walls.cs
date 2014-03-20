using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    class Wall : Unmoveable
    {
        public Wall(Texture2D text, Vector2 pos)
            : base(text, pos)
        {
            
        }
    }
}
