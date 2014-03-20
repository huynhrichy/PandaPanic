using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game
{
    class Trail
    {
        public const int SIZE = 3, DELAY = 30;
     
        List<Vector2> positions;
        List<Rectangle> sources;
        Rectangle rectangle;
        Counter timer;

        public List<Vector2> Positions
        {
        get {return positions;}         
        }

        public List<Rectangle> Sources
        {
            get { return sources; }
        }

        public Rectangle Rectangle
        {
            get { return rectangle; }
        }

        public Trail(Rectangle source)
        {
            rectangle = new Rectangle(0, 0, Moveable.SIZE, Moveable.SIZE);

            positions = new List<Vector2>();
            sources = new List<Rectangle>();

            for (int i = 0; i < SIZE; i++)
            {
                positions.Add(new Vector2(0, 0));
                sources.Add(source);
            }

            timer = new Counter(DELAY);
        }

        public void updateRectangle(int index)
        {
            rectangle.X = (int)(Positions[index].X);
            rectangle.Y = (int)(Positions[index].Y);
        }

        public void update(Vector2 position,Rectangle source)
        {
            if (!timer.isReady()) return;

            for (int i = SIZE - 1; i >= 1; i--)
            {
                positions[i] = positions[i - 1];
                sources[i] = sources[i - 1];
            }

            positions[0] = position;
            sources[0] = source;
        }

       
    }
}
