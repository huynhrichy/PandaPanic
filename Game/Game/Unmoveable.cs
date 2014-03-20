using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game
{
    abstract class Unmoveable : Entity
    {
        public Unmoveable(Texture2D text, Vector2 pos): base(text,pos)
        {

        }
    }
}
