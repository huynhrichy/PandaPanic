using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game
{
    abstract class Entity
    {
        private Vector2 position;
        private Texture2D texture;
        private Rectangle bound;

        public Rectangle Bound
        {
            get { return bound; }
            set { bound = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }            
        }

        public void updateBound()
        {
            bound.X = (int)Position.X;
            bound.Y = (int)Position.Y;
        }

        public void updatePosition(double x, double y)
        {
            position.X += (float)x;
            position.Y += (float)y;
        }

        public void updatePosition(float[] values)
        {
            position.X += values[0];
            position.Y += values[1];
        }

        public Entity(Texture2D text, Vector2 pos)
        {
            position = pos;
            texture = text;            
        }

        public virtual void update()
        {
            
        }

        
    }
}
