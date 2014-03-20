using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Game
{
    class Player : Moveable
    {
        // constants
        public const int LABEL_SIZE = 20,
            START_SCORE = 0,
            START_SPEED = 1,
            DISPLACEMENT_PANDA_Y = 190;

        Rectangle CORNER1 = new Rectangle(0, 0, LABEL_SIZE, LABEL_SIZE),
                        CORNER2 = new Rectangle(Game1.SCREEN_WIDTH - LABEL_SIZE, 0, LABEL_SIZE, LABEL_SIZE),
                        CORNER3 = new Rectangle(0, Game1.SCREEN_HEIGHT - LABEL_SIZE, LABEL_SIZE, LABEL_SIZE),
                        CORNER4 = new Rectangle(Game1.SCREEN_WIDTH - LABEL_SIZE, Game1.SCREEN_HEIGHT - LABEL_SIZE, LABEL_SIZE, LABEL_SIZE);

        static Keys[][] KEY_SET; 
        public enum STATE { NORMAL, PAUSED, ATTACK, DEAD }  

        // varaibles
        int score, type, targetType;
        float speed;
        STATE state;
        PlayerIndex playerNumber;     
        Trail trail;
        Dictionary<String, Rectangle> sourceRectangle;
        List<Tuple<Vector2, Rectangle, Rectangle>> action;
        List<Tuple<Color, Rectangle>> symbol; // the target's square
        Keys[] control;
        Rectangle source;

        // accessors 
        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public int TargetType
        {
            get { return targetType; }
            set { targetType = value; }
        }

        public STATE State
        {
            get { return state; }
            set { state = value; }
        }

        public Color playerColor { get; set; }

        public PlayerIndex PlayerNumber
        {
            get { return PlayerNumber; }
            set
            {
                playerNumber = value;
                switch (value)
                {
                    case PlayerIndex.One:
                        playerColor = Color.Pink;
                        break;
                    case PlayerIndex.Two:
                        playerColor = Color.Green;
                        break;
                    case PlayerIndex.Three:
                        playerColor = Color.LightBlue;
                        break;
                    case PlayerIndex.Four:
                        playerColor = Color.White;
                        break;
                    default:
                        break;
                }
            }
        }  

        public Trail Trail
        {
            get { return trail; }
        }
        
        public List<Tuple<Color, Rectangle>> Symbol
        {
            get { return symbol; }
        }

        public Rectangle Source
        {
            get { return source; }
        }

        // constructor     
        public Player(PlayerIndex _index, Vector2 _position, int _type, int _target)
            : base(null, _position)
        {
            KEY_SET = new Keys[][] { new Keys[] { Keys.W, Keys.A, Keys.S, Keys.D, Keys.Space }, new Keys[] { Keys.I, Keys.J, Keys.K, Keys.L, Keys.RightShift } };
            
            playerNumber = _index;
            type = _type;
            targetType = _target;
            control = KEY_SET[type];            
            score = START_SCORE;            
            speed = START_SPEED;
             
            startSource();
            initAction();
            initSymbol();

            source = sourceRectangle["down"];

            trail = new Trail(source);            
        }

        //deadLabelFont = Font.getFont().font;
        //deadLabelColor = Color.Yellow;

        // methods    
        
        void startSource()
        {
            int displacement = DISPLACEMENT_PANDA_Y * type;

            sourceRectangle = new Dictionary<string, Rectangle>();

            sourceRectangle.Add("down", new Rectangle(0, displacement, 127, 156));
            sourceRectangle.Add("left", new Rectangle(137, displacement, 106, 162));
            sourceRectangle.Add("up", new Rectangle(274, displacement, 127, 156));
            sourceRectangle.Add("right", new Rectangle(411, displacement, 106, 162));
            sourceRectangle.Add("frontStab", new Rectangle(548, displacement, 127, 190));
            sourceRectangle.Add("leftStab", new Rectangle(685, displacement, 137, 165));
            sourceRectangle.Add("backStab", new Rectangle(821, displacement, 127, 184));
            sourceRectangle.Add("rightStab", new Rectangle(958, displacement, 127, 165));
            sourceRectangle.Add("dead", new Rectangle(1095, displacement, 165, 147));
        }

        void initSymbol()
        {
            symbol = new List<Tuple<Color, Rectangle>>();
            symbol.Add(new Tuple<Color, Rectangle>(Color.Pink, CORNER1));
            symbol.Add(new Tuple<Color, Rectangle>(Color.Green, CORNER2));
            symbol.Add(new Tuple<Color, Rectangle>(Color.Blue, CORNER3));
            symbol.Add(new Tuple<Color, Rectangle>(Color.Gray, CORNER4));
        }

        void initAction()
        {
            action = new List<Tuple<Vector2, Rectangle, Rectangle>>();

            action.Add(new Tuple<Vector2, Rectangle, Rectangle>(
                new Vector2(0, -speed), this.sourceRectangle["up"], this.sourceRectangle["backStab"]));
            action.Add(new Tuple<Vector2, Rectangle, Rectangle>(
                new Vector2 ( -speed, 0 ), this.sourceRectangle["left"], this.sourceRectangle["leftStab"]));
            action.Add(new Tuple<Vector2, Rectangle, Rectangle>(
                new Vector2(0, speed),  this.sourceRectangle["down"], this.sourceRectangle["frontStab"]));
            action.Add(new Tuple<Vector2, Rectangle, Rectangle>(
                new Vector2(speed, 0),  this.sourceRectangle["right"], this.sourceRectangle["rightStab"]));            
        }

        public override void update()
        {
            if (!Alive) return;

            /*
             * every player has a list of keys
             * the first 4 characters are the directional keys up, left, down, right, and the attack key
             * the keys are stored in an array. each key is check to determine if it was pressed
             * the last element is the attack key. It is checked to determine what type of textures should be used
             * if the space key is pressed the attack texture related to the direction is used
             * otherwise the regular image is used
             */

            if (Keyboard.GetState().IsKeyDown(control.Last()))
            {
                State = STATE.ATTACK;
                for (int i = 0; i < control.Length - 1; i++)
                {
                    if (Keyboard.GetState().IsKeyDown(control[i]))
                    {
                        source = action[i].Item3;             
                        Position += action[i].Item1;
                    }
                }
            }
            else
            {
                State = STATE.NORMAL;
                for (int i = 0; i < control.Length - 1; i++)
                {
                    if (Keyboard.GetState().IsKeyDown(control[i]))
                    {
                        source = action[i].Item2;            
                        Position += action[i].Item1;
                    }
                }
            }

            base.update();
            updateBound();
            trail.update(Position, source);            
        }        

        public void die()
        {
            Alive = false;
            source = sourceRectangle["dead"];
        }

        public void reset(Vector2 position)
        {
            Alive = true;
            state = STATE.NORMAL;
            source = sourceRectangle["up"];

            speed = START_SPEED;
            Position = position;
        }
    }
}
