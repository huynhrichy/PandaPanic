using System;
using System.IO;
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
    class Controller
    {
        delegate void Menu();

        public enum STATE { SETTING, MAINMENU, CREDITS, INSTRUCTIONS, REGULAR, ENDGAME, PAUSED, EXIT }
        public const int EMPTY_INPUT = 0,
            PAUSE_TIME = 5,
            DEFAULT_NUM_OF_ROUNDS = 2,
            DEFAULT_ROUND_TIME = 30,
            NUM_OF_PLAYERS = 2,
            FPS = 60;

        Menu[] gameMenu;
        STATE state;

        int currentRound, typeOfMax;
        Collection collection;
        Game.Game_Objects.Artist artist;
        Counter roundTimer, pauseTimer;
        bool initGame;

        Dictionary<String, int> gameSettings;

        // GUI components
        WidgetDemonstration widget;

        public Game.Game_Objects.Artist Artist
        {
            get { return artist; }
        }

        public int CurrentRound
        {
            get { return currentRound; }
        }

        public Counter RoundTimer
        {
            get { return roundTimer; }
        }

        public int TypeOfMax
        {
            get { return typeOfMax; }
        }

        public WidgetDemonstration Widget
        {
            get { return widget; }
        }

        public Controller()
        {
            gameMenu = new Menu[]{
                startGame, settings, instructions, credits, exit, back
            };

            gameSettings = new Dictionary<String, int>();
            gameSettings.Add("round time", DEFAULT_ROUND_TIME);
            gameSettings.Add("#rounds", DEFAULT_NUM_OF_ROUNDS);
            gameSettings.Add("#players", NUM_OF_PLAYERS);

            state = STATE.MAINMENU;
            pauseTimer = new Counter(PAUSE_TIME * FPS);
            roundTimer = new Counter(DEFAULT_ROUND_TIME * FPS);
            artist = new Game.Game_Objects.Artist();
            widget = new WidgetDemonstration();
            initGame = true;

        }

        public void startGame()
        {
            state = STATE.REGULAR;

            widget.setMainMenuVisible(false);
            widget.setActive(false);

            if (!initGame) return;

            initGame = false;
            collection = new Collection(gameSettings["#players"]);
            collection.startPlayers();
            collection.startEnemies();
            //collection.startBoard();

        }

        public void settings()
        {
            state = STATE.SETTING;

            widget.setMainMenuVisible(false);
            widget.setSettingsVisibility(true);
            widget.setBackVisible(true);

            widget.setSettings(gameSettings.Values.ToArray());
        }

        public void instructions()
        {
            state = STATE.INSTRUCTIONS;

            widget.setMainMenuVisible(false);
            widget.setBackVisible(true);
        }

        public void credits()
        {
            state = STATE.CREDITS;

            widget.setMainMenuVisible(false);
            widget.setBackVisible(true);

            // reset input
            widget.ButtonPressed = EMPTY_INPUT;
        }

        public void exit()
        {
            state = STATE.EXIT;
        }

        public void back()
        {
            if (state == STATE.SETTING)
            {
                setSettings(widget.getSettings());
                widget.setSettingsVisibility(false);
            }
            state = STATE.MAINMENU;
            widget.setMainMenuVisible(true);
            widget.setBackVisible(false);
        }

        public void updateMode()
        {
            if (widget.ButtonPressed != EMPTY_INPUT)
                gameMenu[widget.ButtonPressed - 1]();

            // reset input
            widget.ButtonPressed = EMPTY_INPUT;
        }

        public bool gameOver()
        {
            return state == STATE.EXIT;
        }

        public void maxPlayerScore()
        {
            int max = 0;
            typeOfMax = 0;
            foreach (Player _player in collection.Players)
            {
                if (_player.Score > max)
                {
                    max = _player.Score;
                    typeOfMax = _player.Type;
                }
            }
        }

        public void update(ref GameTime gameTime, ref ContentManager content)
        {
            switch (state)
            {
                case STATE.REGULAR:
                    collection.Entities.ForEach(entity => entity.update());
                    collection.regularPlay(ref gameTime);
                    foreach (Player p in collection.Players)
                    {
                        if (!p.Alive)
                        {
                            state = STATE.PAUSED;
                            maxPlayerScore();
                            break;
                        }
                    }

                    if (roundTimer.isReady())
                    {
                        state = STATE.PAUSED;
                        maxPlayerScore();
                    }

                    break;
                case STATE.PAUSED:
                    collection.pausedPlay();

                    if (!pauseTimer.isReady()) return;

                    if (currentRound < gameSettings["#rounds"])
                    {
                        state = STATE.REGULAR;
                        currentRound++;
                        collection.resetRound();
                        roundTimer.reset();
                        //playNewSong(ref content);
                    }
                    else state = STATE.ENDGAME;
                    break;
                case STATE.ENDGAME:
                    if (Keyboard.GetState().IsKeyDown(Keys.B))
                    {
                        currentRound = 0;
                        collection.resetGame();
                        state = STATE.MAINMENU;
                        widget.setActive(true);
                        widget.setMainMenuVisible(true);
                    }
                    break;
                default:
                    break;
            }
        }

        public void draw(ref SpriteBatch batch)
        {
            batch.Begin();
            switch (state)
            {
                case STATE.MAINMENU:
                    artist.DrawBackground["background"](ref batch);
                    break;
                case STATE.SETTING:
                    artist.DrawBackground["background"](ref batch);
                    break;
                case STATE.PAUSED:
                    goto case STATE.REGULAR;
                case STATE.REGULAR:
                    Controller _controller = this;
                    artist.drawFloor(ref batch);
                    artist.drawController(ref _controller, ref batch);
                    artist.drawEntities(ref collection, ref batch);
                    break;
                case STATE.ENDGAME:
                    Controller __controller = this;
                    artist.drawFinalScreen(ref collection, ref __controller, ref batch);
                    break;
                case STATE.CREDITS:
                    artist.DrawBackground["credit"](ref batch);
                    break;
                case STATE.INSTRUCTIONS:
                    artist.DrawBackground["how"](ref batch);
                    break;
            }
            batch.End();

            widget.Draw();
        }

        public void setSettings(int[] _settings)
        {
            gameSettings["round time"] = _settings[0];
            gameSettings["#rounds"] = _settings[1];
            gameSettings["#players"] = _settings[2];

        }
    }
}
