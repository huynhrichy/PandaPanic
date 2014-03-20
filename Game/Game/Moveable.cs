using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Game
{
    abstract class Moveable : Entity
    {
        // constants
        public const int SIZE = 70;

        // variables        
        private bool alive;

        // accessors
        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }
              
        // constructor
        public Moveable(Texture2D texture, Vector2 position)
            : base(texture, position)
        {
            alive = true;
            Bound = new Rectangle((int)Position.X, (int)Position.Y, SIZE, SIZE);
        }
        
        // methods
        public override void update()
        {
            // wraps the moveable object around the screen
            if (Position.X > Game1.SCREEN_WIDTH)
                Position = new Vector2(-SIZE, Position.Y);
            else if (Position.X < -SIZE)
                Position = new Vector2(Game1.SCREEN_WIDTH, Position.Y);
            if (Position.Y > Game1.SCREEN_HEIGHT)
                Position = new Vector2(Position.X, -SIZE);
            else if (Position.Y < -SIZE)
                Position = new Vector2(Position.X, Game1.SCREEN_HEIGHT);

            // sets the bounds position to the position variable. it is for collisions
            updateBound();
        }
    }
}
