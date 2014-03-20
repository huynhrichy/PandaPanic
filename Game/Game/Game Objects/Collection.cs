using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Game
{
    class Collection
    {
        // constants
        public enum STATE { SETTING, MAINMENU, LOCALMENU, HOSTMENU, JOINMENU, CREDITS, INSTRUCTIONS, REGULAR, ENDGAME, NETWORK, PAUSED, EXIT }

        const int NUM_OF_AI = 10;
                    
        Vector2[] initialPositions, corners;
        PlayerIndex[] indexes;

        // variables
        List<Player> players;  //added from chris' code
        List<Enemy> enemies;
        List<Entity> entities;                
        
        STATE state;
        Board board;
        Random random;
        int numOfPlayers, max;       

        // accessors
        public STATE State
        {
            get { return state; }
            set { state = value; }
        }

        public int NumOfPlayers
        {
            get { return numOfPlayers; }
            set { numOfPlayers = value; }
        }
        
        public List<Player> Players
        {
            get { return players; }
            set { players = value; }
        }

        public List<Entity> Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        public List<Enemy> Enemies
        {
            get { return enemies; }
            set { enemies = value; }
        }
        
        public Random Random
        {
            get { return random; }
        }

        // constructor
        public Collection(int _numOfPlayers)
        {
            
            initialPositions = new Vector2[] { 
                new Vector2(0, 0), 
                new Vector2(Game1.SCREEN_WIDTH - Moveable.SIZE, 0), 
                new Vector2(0, Game1.SCREEN_HEIGHT - Moveable.SIZE), 
                new Vector2(Game1.SCREEN_WIDTH - Moveable.SIZE, Game1.SCREEN_HEIGHT - Moveable.SIZE) 
            };
            indexes = new PlayerIndex[] { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };
           
            entities = new List<Entity>();
            random = new Random();
            state = STATE.MAINMENU;
            numOfPlayers = _numOfPlayers;
        }

        // methods
        public void startPlayers()
        {
            players = new List<Player>();
            List<Texture2D> sprites = new List<Texture2D>();

            for (int i = 0; i < numOfPlayers; i++)
            {
                entities.Add(new Player(indexes[i], initialPositions[i], i, (i + 1) % numOfPlayers));
            }

            players = entities.Where(i => i.GetType() == typeof(Player)).ToList().Cast<Player>().ToList();
        }

        public void startEnemies()
        {
            enemies = new List<Enemy>();

            // adds the AI objects
            for (int i = 0; i < NUM_OF_AI; i++)
            {
                Enemy enemy = new Enemy(random.Next(4), random);
                entities.Add(enemy);
            }

            enemies = entities.Where(i => i.GetType() == typeof(Enemy)).ToList().Cast<Enemy>().ToList();
        }

        /*
        public void startBoard()
        {
            // Create Board
            board = new Board(artist.Textures["background"], new Vector2(), artist.MainFrame, artist.Textures["block"], random);

            board.generateField();
            entities.Add(board);
            board.AddtoEntities(entities);
        }        
        */

        void setPlayerStates(Player.STATE state)
        {
            players.ForEach(player => player.State = state);
        }

        void playervsplayer()
        {
            foreach (Player player1 in players)
            {
                if (!player1.Alive) continue;

                foreach (Player player2 in players)
                {

                    if (!(player1.State == Player.STATE.ATTACK
                       && player2.Alive
                          && !player1.Equals(player2)
                        && player1.Bound.Intersects(player2.Bound)))
                        continue;
                    {
                        if (player1.TargetType == player2.Type)
                        {
                            //player1.Score += (int)time.getTime();
                            player2.die();
                            /*sounds["hit"].Play();
                            sounds["death"].Play();
                            sounds["stab"].Play();*/
                        }
                        else player1.Score -= 25;
                    }
                }
            }
        }

        void playervsenemy()
        {
            foreach (Player player in players)
            {
                if (!player.Alive) continue;

                foreach (Enemy enemy in enemies)
                {

                    if (!(player.State == Player.STATE.ATTACK
                        && enemy.Alive
                        && player.Bound.Intersects(enemy.Bound)))
                        continue;

                    player.Score -= 5;
                    enemy.die();
                    /*sounds["hit"].Play();
                    sounds["death"].Play();*/
                }
            }
        }

        public void resetRound()
        {
            State = STATE.REGULAR;

            for (int i = 0; i < NumOfPlayers; i++) Players[i].reset(initialPositions[i]);

            Enemies.ForEach(enemy => enemy.reset());
        }

        public void resetGame()
        {
            State = STATE.MAINMENU;
            Players.ForEach(player => player.Score = 0);
        }

        public void regularPlay(ref GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                state = STATE.EXIT;

            playervsplayer();
            playervsenemy();           

            switch (State)
            {
                case STATE.PAUSED:
                    setPlayerStates(Player.STATE.PAUSED);
                    break;
                default:
                    setPlayerStates(Player.STATE.NORMAL);
                    break;
            }
        }

        public void pausedPlay()
        {
            players.ForEach(player => player.update());            
        }
                                      
    }
}
