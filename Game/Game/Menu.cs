using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game
{
    public class Menu
    {
        private bool initialized = false;
        private List<MenuItem> menuItems { get; set; }
        public int Count
        {
            get { return menuItems.Count; }
        }
        public string Title { get; set; }
        public string InfoText { get; set; }
        private int lastNavigated { get; set; }
        private int _selectedIndex;
        public int selectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            protected set
            {
                if (value >= menuItems.Count || value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _selectedIndex = value;
            }
        }
        public MenuItem SelectedItem
        {
            get
            {
                return menuItems[selectedIndex];
            }
        }

        public Menu(string title)
        {
            menuItems = new List<MenuItem>();
            Title = title;
            InfoText = "";
        }

        public Menu(string title, string infoText)
        {
            menuItems = new List<MenuItem>();
            Title = title;
            InfoText = infoText;
        }

        public virtual void AddMenuItem(string title, Action<Buttons> action)
        {
            AddMenuItem(title, action, "");
        }
        
        public virtual void AddMenuItem(string title, Action<Buttons> action, string description)
        {
            menuItems.Add(new MenuItem { Title = title, Description = description, Action = action });
            selectedIndex = 0;
        }

        public virtual void AddString(SpriteBatch batch, SpriteFont font, string s, Vector2 pos)
        {
            batch.DrawString(font, s, pos, Color.Black);
        }

        public void DrawMenu(SpriteBatch batch, int screenWidth, SpriteFont font)
        {
            DrawMenu(batch, screenWidth,font, 100, new Vector2(1000,1000), Color.Gray, Color.White);
        }

        public void DrawBG(SpriteBatch batch, Texture2D t, Rectangle r)
        {
            batch.Draw(t, r, Color.White);
        }

        public void DrawMenu(SpriteBatch batch, int screenWidth, SpriteFont font, int yPos, Vector2 descriptionPos, Color itemColor, Color selectedColor)
        {
            batch.DrawString(font, Title, new Vector2(screenWidth / 2 - font.MeasureString(Title).X / 2, yPos), Color.Black);
            yPos += (int)font.MeasureString(Title).Y + 10;
            for (int i = 0; i < Count; i++)
            {
                Color color = itemColor;
                if (i == selectedIndex)
                {
                    color = selectedColor;
                }
                batch.DrawString(font, menuItems[i].Title, new Vector2(screenWidth / 2 - font.MeasureString(menuItems[i].Title).X / 2, yPos), color);
                yPos += (int)font.MeasureString(menuItems[i].Title).Y + 10;
            }
            batch.DrawString(font, menuItems[selectedIndex].Description, descriptionPos, selectedColor);
        }

        public void Navigate(KeyboardState keyboardState, GamePadState gamePadState, GameTime gameTime)
        {
            if (!initialized)
            {
                lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                initialized = true;
            }
            if (gameTime.TotalGameTime.TotalMilliseconds - lastNavigated > 250)
            {
                if ((gamePadState.ThumbSticks.Left.Y < -0.5
                            || gamePadState.DPad.Down == ButtonState.Pressed) || keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)
                            && selectedIndex < Count - 1)
                {
                    if (selectedIndex<menuItems.Count-1)selectedIndex++;
                    lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
                if ((gamePadState.ThumbSticks.Left.Y > 0.5
                           || gamePadState.DPad.Up == ButtonState.Pressed) || keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)
                            && selectedIndex > 0)
                {
                    if (selectedIndex>0)selectedIndex--;
                    lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
                if (gamePadState.Buttons.A == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Enter))
                {
                    SelectedItem.Action(Buttons.A);
                    System.Threading.Thread.Sleep(200);
                    lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
                else if (gamePadState.Buttons.B == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Enter))
                {
                    SelectedItem.Action(Buttons.B);
                    lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
                else if (gamePadState.Buttons.X == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Enter))
                {
                    SelectedItem.Action(Buttons.X);
                    lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
                else if (gamePadState.Buttons.Y == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Enter))
                {
                    SelectedItem.Action(Buttons.Y);
                    lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
        }
    }
}
