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

namespace Game.Game_Objects
{
    class Artist
    {
        // constants
        const int
        LABEL_SIZE = 20,
        MENU_Y_DISPLACEMENT = 110,
        TIMER_X = Game1.SCREEN_WIDTH / 2,
        TIMER_Y = 10,
        ROUND_X = Game1.SCREEN_WIDTH / 2,
        ROUND_Y = 30;

        // variables
        public delegate void Draw(ref SpriteBatch batch);

        SpriteFont font;
  
        Vector2[] corner;
        Rectangle mainFrame;
        String[] colors;
        Texture2D[] finalTextures;
        Texture2D bg, texture;
        Random random;

        Dictionary<string, Texture2D> textures;
        Dictionary<string, SoundEffect> sounds;
        Dictionary<string, Draw> drawBackground;

        public Artist()
        {     
            corner = new Vector2[] { 
                new Vector2(0, 0), 
                new Vector2(Game1.SCREEN_WIDTH - Player.LABEL_SIZE, 0), 
                new Vector2(0, Game1.SCREEN_HEIGHT - Player.LABEL_SIZE), 
                new Vector2(Game1.SCREEN_WIDTH - Player.LABEL_SIZE, Game1.SCREEN_HEIGHT - Player.LABEL_SIZE) };
            mainFrame = new Rectangle(0, 0, Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT);
            colors = new String[] { "pink", "green", "blue", "white" };          
            random = new Random();
            startDraw();
        }

        public string[] Colors
        {
            get { return colors; }
        }

        public Rectangle MainFrame
        {
            get { return mainFrame; }
        }
        
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        public Dictionary<string, Texture2D> Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        public Dictionary<string, SoundEffect> Sounds
        {
            get { return sounds; }
            set { sounds = value; }
        }

        public Dictionary<string, Draw> DrawBackground
        {
            get { return drawBackground; }
        }

        public void startMedia(ref ContentManager Content)
        {
            SoundEffect.MasterVolume = 0.1f;

            Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

            // load music
            sounds.Add("death", Content.Load<SoundEffect>("Music\\death_converted"));
            sounds.Add("victory", Content.Load<SoundEffect>("Music\\victory_converted"));
            sounds.Add("thrust", Content.Load<SoundEffect>("Music\\thrust_converted"));
            sounds.Add("stab", Content.Load<SoundEffect>("Music\\stab_converted"));
            sounds.Add("hit", Content.Load<SoundEffect>("Music\\hit_converted"));

         
            // load textures
            textures.Add("credit", Content.Load<Texture2D>("Textures\\credit"));
            textures.Add("HowToPlay", Content.Load<Texture2D>("Textures\\HowToPlay"));
            textures.Add("FINAL_MENU", Content.Load<Texture2D>("Textures\\FINAL_MENU"));
            textures.Add("FINAL_MENU2", Content.Load<Texture2D>("Textures\\FINAL_MENU2"));
            textures.Add("skull", Content.Load<Texture2D>("Textures\\skull"));
            textures.Add("floor", Content.Load<Texture2D>("Textures\\floor"));

            textures.Add("Pandas", Content.Load<Texture2D>("Textures\\Pandas"));

            // Load the background content.
            textures.Add("background", Content.Load<Texture2D>("Textures\\GameBackground"));

            textures.Add("block", Content.Load<Texture2D>("Textures\\GrassPixel"));   //texture for maze

            finalTextures = new Texture2D[5]{
                    Content.Load<Texture2D>("Textures\\FinalScoreBlue"),
                    Content.Load<Texture2D>("Textures\\FinalScoreGreen"),
                    Content.Load<Texture2D>("Textures\\FinalScoreBlue"),
                    Content.Load<Texture2D>("Textures\\FinalScoreWhite"),
                    Content.Load<Texture2D>("Textures\\FinalScoreDraw")
                };
            Textures = textures;
            Sounds = sounds;

            font = Content.Load<SpriteFont>("text");

            startBackground(random.Next(2) == 0);
        }

        public void startBackground(bool choice)
        {
            if (choice) bg = Textures["FINAL_MENU"];
            else bg = Textures["FINAL_MENU2"];
        }

        public void startDraw()
        {
            drawBackground = new Dictionary<string, Draw>();

            drawBackground.Add("background",(ref SpriteBatch batch) => batch.Draw(bg, mainFrame, Color.White));
            drawBackground.Add("credit", (ref SpriteBatch batch) => batch.Draw(Textures["credit"], mainFrame, Color.White));
            drawBackground.Add("how", (ref SpriteBatch batch) => batch.Draw(Textures["HowToPlay"], mainFrame, Color.White));
        }

        public void drawEntities(ref Collection _game, ref SpriteBatch batch)
        {
            foreach (Entity entity in _game.Entities)
            {             
                if(entity.GetType() == typeof(Player))
                {
                    Player _player = (Player)entity;
                    drawPlayer(ref batch, ref _player);
                }
                else if(entity.GetType() == typeof(Enemy))
                {
                    Enemy _enemy = (Enemy)entity;
                    drawEnemy(ref batch, ref _enemy);
                }
                else if(entity.GetType() == typeof(Block))
                {
                    Block _block = (Block)entity;
                    drawBlock(ref batch, ref _block);
                }            
            }
            
            for (int i = 0; i < _game.Players.Count; i++) drawScore(batch, corner[i], _game.Players[i].Score, font);
        }
        
        public void drawController(ref Controller _controller, ref SpriteBatch batch)
        {
            batch.DrawString(font, "Time: " + _controller.RoundTimer.Current / Controller.FPS, new Vector2(TIMER_X, TIMER_Y),
                Color.Black, 0, Vector2.Zero, new Vector2((float)0.75, (float)0.75), SpriteEffects.None, 0);

            batch.DrawString(Font, "Round#: " + (_controller.CurrentRound + 1), new Vector2(ROUND_X, ROUND_Y),
                Color.Black, 0, Vector2.Zero, new Vector2((float)0.75, (float)0.75), SpriteEffects.None, 0);
        }

        public void drawFinalScreen(ref Collection _game, ref Controller _controller, ref SpriteBatch batch)
        {

            batch.Draw(finalTextures[_controller.TypeOfMax], mainFrame, Color.White);

            String msg;
            for (int i = 0; i < _game.NumOfPlayers; i++)
            {
                msg = "Player " + i + " ( " + colors[i] + " ) Score: " + _game.Players[i].Score;
                batch.DrawString(Font, msg, new Vector2(270, MENU_Y_DISPLACEMENT + (50 * i)), Color.White);
            }
        }

        public void drawScore(SpriteBatch batch, Vector2 position, float value, SpriteFont font)
        {
            batch.DrawString(font, value.ToString(), position, Color.Black);
        }

        public void drawPlayer(ref SpriteBatch batch, ref Player player)
        {
            // draw target cikir
            batch.Draw(Game1.defaultTexture, player.Symbol[player.TargetType].Item2, player.Symbol[player.TargetType].Item1);

            Trail _trail = player.Trail;
            drawTrail(ref batch, ref _trail);

            //show label when a player dies
            if (!player.Alive) batch.Draw(Game1.defaultTexture, player.Bound, Color.Red);

            batch.Draw(textures["Pandas"], player.Bound, player.Source, Color.White);
        }

        public void drawEnemy(ref SpriteBatch batch, ref Enemy enemy)
        {
            batch.Draw(textures["Pandas"], enemy.Bound, enemy.Source, Color.White);
        }

        public void drawBlock(ref SpriteBatch batch, ref Block block)
        {
            batch.Draw(block.Texture, block.Bound, Color.White * 0.1f);
        }

        public void drawTrail(ref  SpriteBatch batch, ref Trail trail)
        {
            Color _color = Color.White;

            for (int i = Trail.SIZE - 1; i >= 0; i--)
            {
                _color.A = (byte)((trail.Positions.Count - i) * (255 / trail.Positions.Count));
                trail.updateRectangle(i);
                batch.Draw(textures["Pandas"], trail.Rectangle, trail.Sources[i], _color);
            }
        }

        public void drawFloor(ref SpriteBatch batch)
        {
            batch.Draw(textures["floor"], mainFrame, Color.White);
        }

        void playNewSong(ref ContentManager content)
        {
            String songName = "Music\\" + new Random().Next(1, 11);
            Song song = content.Load<Song>(songName);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);
            MediaPlayer.Volume = .4f;
            if (songName.Contains("6")) MediaPlayer.Volume = 0.2f;
            if (songName.Contains("9") || songName.Contains("10")) MediaPlayer.Volume = 1;
        }
    }
}
