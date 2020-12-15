using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gol
{
    internal class Gol : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Map map;
        private int count = 0;

        public Gol()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            graphics.ToggleFullScreen();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            map = new Map(graphics);
        }

        protected override void Update(GameTime gameTime)
        {
            map.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            count++;
            if (count >= 200000)
                Exit();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            map.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}
