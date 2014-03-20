using Microsoft.Xna.Framework;

namespace Game {

    public abstract class Screen {

        public Color Color { get; set; }

        public abstract void Init(Game1 game);
        public abstract void OnResize();
        public abstract void Update();
        public abstract void Draw();
    }
}
