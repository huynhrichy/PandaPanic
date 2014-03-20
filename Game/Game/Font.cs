using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
namespace Game
{
    class Font
    {
        public SpriteFont font {get;set;}
        static Font instance;
        public Font(SpriteFont sFont)
        {
            font = sFont;
            instance = this;
        }

        public static Font getFont()
        {
            return instance;
        }
    }
}
