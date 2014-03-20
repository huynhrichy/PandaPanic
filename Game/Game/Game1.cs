using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Ruminate.Utils;
using Ruminate.DataStructures;

namespace Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {                          
        // varaibles
                
        public const int SCREEN_WIDTH = 800, SCREEN_HEIGHT = 480;
        
        public static Texture2D defaultTexture;
                
        GraphicsDeviceManager graphics;
        SpriteBatch batch;

        Controller controller;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public SpriteFont TestSpriteFont;
        public Texture2D TestImageMap;
        public string TestMap;

        public SpriteFont GreySpriteFont;
        public Texture2D GreyImageMap;
        public string GreyMap;

        public Game1()
        {            
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };
            
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += delegate
            {
                if (controller.Widget != null) { controller.Widget.OnResize(); }
            };
            
            Content.RootDirectory = "Content";
        }

        // TODO: Add your initialization logic here      
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();                       
            
            IsMouseVisible = true;

            controller = new Controller();
            
            ContentManager manager = Content;

            //Music init
            //playNewSong();             
           
            defaultTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            defaultTexture.SetData(new[] { Color.White });

            base.Initialize();
        }       

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            batch = new SpriteBatch(GraphicsDevice);

            TestImageMap = Content.Load<Texture2D>("TestSkin\\ImageMap");
            TestMap = File.OpenText("Content\\TestSkin\\Map.txt").ReadToEnd();
            TestSpriteFont = Content.Load<SpriteFont>("TestSkin\\Font");

            GreyImageMap = Content.Load<Texture2D>("GreySkin\\ImageMap");
            GreyMap = File.OpenText("Content\\GreySkin\\Map.txt").ReadToEnd();
            GreySpriteFont = Content.Load<SpriteFont>("GreySkin\\Texture");

            ContentManager _content = Content;
            controller.Artist.startMedia(ref _content);
            //controller.startWidget(ref _content);
            DebugUtils.Init(graphics.GraphicsDevice, GreySpriteFont);
          
            controller.Widget.Init(this);           
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            ContentManager manager = Content;

            controller.Widget.Update();            
            controller.updateMode();            
            controller.update(ref gameTime, ref manager);
            if (controller.gameOver()) Exit();
           
            base.Update(gameTime);
        }       

        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);

            Console.WriteLine(fps);

            GraphicsDevice.Clear(controller.Widget.Color);
            
            controller.draw(ref batch);
           
            base.Draw(gameTime);
        }

        
    }
}