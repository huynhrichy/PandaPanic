using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;

namespace Game
{

    /// <summary>
    /// This screen is for demonstrating the available widget and their capabilities. 
    /// </summary>
    class WidgetDemonstration : Screen
    {
        const int   BUTTON_DISPLACEMENT_Y = 100,
                    TEXTBOX_DISPLACEMENT_X = 300,
                    TEXTBOX_DISPLACEMENT_Y = 100,
                    TEXTBOX_WIDTH = 100,
                    TEXTBOX_HEIGHT = 40,
                    LABEL_DISPLACEMENT_X = 200,
                    LABEL_DISPLACEMENT_Y = 100,
                    NUM_OF_BUTTONS = 6,
                    SETTING_INDEX = 9;

        public Gui _gui;

        int buttonPressed;

        public int ButtonPressed
        {
            get { return buttonPressed; }
            set { buttonPressed = value; }
        }

        public Widget[] Widget
        {
            get { return _gui.Widgets; }
        }

        public override void Init(Game1 game)
        {   
           
            Color = Color.White;

            var beaker = game.Content.Load<Texture2D>("beaker");

            var skin = new Skin(game.GreyImageMap, game.GreyMap);
            var text = new Text(game.GreySpriteFont, Color.Black);

            var testSkin = new Skin(game.TestImageMap, game.TestMap);
            var testText = new Text(game.TestSpriteFont, Color.Black);

            var testSkins = new[] { new Tuple<string, Skin>("testSkin", testSkin) };
            var testTexts = new[] { new Tuple<string, Text>("testText", testText) };
            
            _gui = new Gui(game, skin, text, testSkins, testTexts)
            {
                
                Widgets = new Widget[] {                                           
                    //By default the Button is as wide as the width of the label plus the edge of the button graphic
                    new Button(10, BUTTON_DISPLACEMENT_Y + (40 * 0), "Start Game", buttonEvent: delegate(Widget widget) {
                        buttonPressed = 1;
                    }) { Skin = "testSkin", Text = "testText" },                    
                    new Button(10, BUTTON_DISPLACEMENT_Y + (40 * 1), "Options", buttonEvent: delegate(Widget widget) {
                        buttonPressed = 2;
                    }) { Skin = "testSkin", Text = "testText" },
                    new Button(10, BUTTON_DISPLACEMENT_Y + (40 * 2), "Instructions", buttonEvent: delegate(Widget widget) {
                        buttonPressed = 3;
                    }) { Skin = "testSkin", Text = "testText" },
                    new Button(10, BUTTON_DISPLACEMENT_Y + (40 * 3), "Credits", buttonEvent: delegate(Widget widget) {
                        buttonPressed = 4;
                    }) { Skin = "testSkin", Text = "testText" },
                    new Button(10, BUTTON_DISPLACEMENT_Y + (40 * 4), "Exit", buttonEvent: delegate(Widget widget) {
                        buttonPressed = 5;
                    }) { Skin = "testSkin", Text = "testText" },
                    new Button(10, BUTTON_DISPLACEMENT_Y + (40 * 5), "Back", buttonEvent: delegate(Widget widget) {
                        buttonPressed = 6;
                    }) { Skin = "testSkin", Text = "testText", Visible = false },
             
                    new Label(LABEL_DISPLACEMENT_X, LABEL_DISPLACEMENT_Y + (40 * 0), "Time"){Visible = false},
                    new Label(LABEL_DISPLACEMENT_X, LABEL_DISPLACEMENT_Y + (40 * 1), "Rounds"){Visible = false},
                    new Label(LABEL_DISPLACEMENT_X, LABEL_DISPLACEMENT_Y + (40 * 2), "Players"){Visible = false},

                    new SingleLineTextBox(TEXTBOX_DISPLACEMENT_X, TEXTBOX_DISPLACEMENT_Y + (40 * 0), TEXTBOX_WIDTH, TEXTBOX_HEIGHT){Visible = false},
                    new SingleLineTextBox(TEXTBOX_DISPLACEMENT_X, TEXTBOX_DISPLACEMENT_Y + (40 * 1), TEXTBOX_WIDTH, TEXTBOX_HEIGHT){Visible = false},
                    new SingleLineTextBox(TEXTBOX_DISPLACEMENT_X, TEXTBOX_DISPLACEMENT_Y + (40 * 2), TEXTBOX_WIDTH, TEXTBOX_HEIGHT){Visible = false},                   
                }
            };
        }

        public override void OnResize()
        {
            _gui.Resize(); 
        }

        public override void Update()
        {
            _gui.Update();
        }

        public override void Draw()
        {
            _gui.Draw();
        }

        public void setBackVisible(bool visible)
        {
            _gui.Widgets[NUM_OF_BUTTONS - 1].Visible = visible;
        }

        public void setActive(bool _active)
        {
            for (int i = 0; i < _gui.Widgets.Length; i++)
                _gui.Widgets[i].Active = _active;
        }

        public void setMainMenuVisible(bool visibility)
        {
           
            for (int i = 0; i < NUM_OF_BUTTONS -1; i++)
                _gui.Widgets[i].Visible = visibility;
        }

        public void setSettingsVisibility(bool visibility)
        {
            for (int i = NUM_OF_BUTTONS; i < _gui.Widgets.Length; i++)
                _gui.Widgets[i].Visible = visibility;
        }

        public void setSettings(int[] values)
        {
            SingleLineTextBox textbox;

            for (int i = 0; i < 3; i++ )
            {
                textbox = (SingleLineTextBox)(_gui.Widgets[SETTING_INDEX + i]);
                textbox.Value = values[i].ToString();
            }
        }
        
        public int[] getSettings()
        {
            SingleLineTextBox textbox;
            int[] settings = new int[3]; 
            for (int i = 0; i < 3; i++)
            {
                textbox = (SingleLineTextBox)(_gui.Widgets[SETTING_INDEX + i]);
                settings[i] = Convert.ToInt32(textbox.Value);
            }
            return settings;
        }
    }
}
