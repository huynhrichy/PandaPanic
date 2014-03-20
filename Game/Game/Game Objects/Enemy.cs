using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game
{
    class Enemy : Moveable
    {
        // constants
        const int MAX_SPEED = 2, 
            DISPLACEMENT_PANDA_Y = 190;

        // variables
        delegate bool Speed();

        Vector2 speed;
        Random random;
        Counter counter;

        int type, moveType, time, radius;
        double circleSpeed;

        Counter moveSwitch, pause;
        bool stop;
        Rectangle rectangle, source;
   
        Dictionary<String, Vector2> positions;
        List<Tuple<Speed, Rectangle>> textureDecision;
        Dictionary<String, Rectangle> sources;
        
        // accessors
        public Rectangle Rectangle
        {
            get { return rectangle; }
        }

        public Rectangle Source
        {
            get { return source; }
            set { source = value; }
        }

        // constructor
        public Enemy(int _type, Vector2 position, Vector2 speed, Random random)
            : base(null, position)
        {
            this.speed = speed;
            this.random = random;
           
            circleSpeed = ((Math.PI * 2) * 0.0001) / 1;

            type = _type;
            positions = new Dictionary<string, Vector2>();
            positions.Add("target", Vector2.One);
            positions.Add("center", Vector2.One);
            startSource();
        }

        public Enemy(int type, Random random) :
            this(
           type,
    
           new Vector2(random.Next(0, (int)Game1.SCREEN_WIDTH), random.Next(0, (int)Game1.SCREEN_HEIGHT)),
           new Vector2(0, 0),
           random)
        {
            speed.X = random.Next(-MAX_SPEED, MAX_SPEED);
            speed.Y = random.Next(-MAX_SPEED, MAX_SPEED);

            Alive = true;
            moveType = 0;

            moveSwitch = new Counter(random.Next(1000, 3000));
            counter = new Counter(random.Next(10, 20));
            pause = new Counter(random.Next(100, 200));

            rectangle = new Rectangle((int)Position.X, (int)Position.Y, SIZE, SIZE);

            textureDecision = new List<Tuple<Speed, Rectangle>>();

            textureDecision.Add(new Tuple<Speed, Rectangle>(() => speed.Y < 0, sources["up"]));
            textureDecision.Add(new Tuple<Speed, Rectangle>(() => speed.X < 0, sources["left"]));
            textureDecision.Add(new Tuple<Speed, Rectangle>(() => speed.Y > 0, sources["down"]));
            textureDecision.Add(new Tuple<Speed, Rectangle>(() => speed.X > 0, sources["right"]));
        }

        // methods
        void startSource()
        {
            int displacement = DISPLACEMENT_PANDA_Y * type;

            sources = new Dictionary<string, Rectangle>();

            sources.Add("down", new Rectangle(0, displacement, 127, 156));
            sources.Add("left", new Rectangle(137, displacement, 106, 162));
            sources.Add("up", new Rectangle(274, displacement, 127, 156));
            sources.Add("right", new Rectangle(411, displacement, 106, 162));
            sources.Add("frontStab", new Rectangle(548, displacement, 127, 190));
            sources.Add("leftStab", new Rectangle(685, displacement, 137, 165));
            sources.Add("backStab", new Rectangle(821, displacement, 127, 184));
            sources.Add("rightStab", new Rectangle(958, displacement, 127, 165));
            sources.Add("dead", new Rectangle(1095, displacement, 165, 147));
        }

        public override void update()
        {
            if (!Alive) return;

            for (int i = 0; i < textureDecision.Count; i++)
                if (textureDecision[i].Item1()) Source = textureDecision[i].Item2;

            if (moveSwitch.isReady())
            {
                moveType = random.Next(6);
                switch (moveType)
                {
                    case 3:
                    case 4:
                        speed.X = random.Next(-MAX_SPEED, MAX_SPEED);
                        speed.Y = random.Next(-MAX_SPEED, MAX_SPEED);
                        break;
                    case 5:
                        AI01();
                        break;
                }
            }

            switch (moveType)
            {
                case 0:
                    if (counter.isReady()) AI01();
                    Position += speed;
                    break;
                case 1:
                    if (counter.isReady()) AI02();
                    Position += speed;
                    break;
                case 2:
                    if (counter.isReady()) AIstaircase();
                    Position += speed;
                    break;
                case 3:
                    AIwaveX();
                    break;
                case 4:
                    AIwaveY();
                    break;
                case 5:
                    if (pause.isReady()) stop = !stop;
                    if (!stop) Position += speed;
                    break;
            }

            updateBound();
            base.update();
        }

        private void AI01()
        {
            if (random.Next(2) == 0)
            {
                speed.X = random.Next(-MAX_SPEED, MAX_SPEED);
                speed.Y = 0;
            }
            else
            {
                speed.X = 0;
                speed.Y = random.Next(-MAX_SPEED, MAX_SPEED);
            }
        }

        private void AI02()
        {
            speed.X = random.Next(-MAX_SPEED, MAX_SPEED);
            speed.Y = random.Next(-MAX_SPEED, MAX_SPEED);
        }

        private void AIstaircase()
        {
            if (speed.X != 0)
            {
                speed.Y = speed.X;
                speed.X = 0;
            }
            else
            {
                speed.X = speed.Y;
                speed.Y = 0;
            }
        }

        private void AIwaveY()
        {
            updatePosition(speed.X, 2 * (float)Math.Cos(Position.X / 20));
        }

        private void AIwaveX()
        {
            updatePosition(2 * (float)Math.Cos(Position.Y / 20), speed.Y);
        }

        public void die()
        {
            Alive = false;
            speed.X = 0;
            speed.Y = 0;            
            source = sources["dead"]; 
        }

        public void Circle()
        {
            ++time;
            updatePosition(positions["center"].X + Math.Sin(time * circleSpeed) * radius,
            positions["center"].Y + Math.Cos(time * circleSpeed) * radius);
        }

        public void reset()
        {
            Alive = true;            
            source = sources["up"]; 
        }

    }
}
