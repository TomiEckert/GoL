using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gol
{
    internal class Map
    {
        private const double CHANCE = 0.08;
        private const double SCALE = 4;
        bool run = false;
        Point cursor;

        private int Width { get; set; }
        private int Height { get; set; }

        private bool spaceDown;
        private Random random;
        private Texture2D green;
        private Texture2D white;

        public Map(int width, int height, GraphicsDeviceManager graphics)
        {
            random = new Random();
            Width = width;
            Height = height;

            SetupColors(graphics);
            GenerateNew(false);
        }
        public Map(GraphicsDeviceManager graphics)
        {
            random = new Random();
            Width = (int)Math.Floor(graphics.GraphicsDevice.Viewport.Width / SCALE);
            Height = (int)Math.Floor(graphics.GraphicsDevice.Viewport.Height / SCALE);
            SetupColors(graphics);
            
            GenerateNew(false);
        }

        private void SetupColors(GraphicsDeviceManager graphics)
        {
            cursor = Mouse.GetState().Position;

            green = new Texture2D(graphics.GraphicsDevice, 1, 1);
            Color[] data = new Color[1 * 1];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                data[pixel] = Color.Green;
            }
            green.SetData(data);

            white = new Texture2D(graphics.GraphicsDevice, 1, 1);
            data = new Color[1 * 1];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                data[pixel] = Color.White;
            }
            white.SetData(data);
        }

        private List<List<PointType>> stuff;
        private List<List<PointType>> oldStuff;
        private bool backDown;
        private bool mouseDown;
        private bool enterDown;

        public PointType this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1)
                    return PointType.Invalid;
                return oldStuff[x][y];
            }
            set
            {
                stuff[x][y] = value;
            }
        }

        private void GenerateNew(bool empty)
        {
            stuff = new List<List<PointType>>();
            oldStuff = new List<List<PointType>>();
            for (int x = 0; x < Width; x++)
            {
                List<PointType> temp = new List<PointType>();
                for (int y = 0; y < Height; y++)
                {
                    if (!empty && random.NextDouble() < CHANCE)
                        temp.Add(PointType.Alive);
                    else
                        temp.Add(PointType.Dead);
                }
                stuff.Add(temp);
                oldStuff.Add(temp.Clone());
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateControls();

            if (run)
                UpdateRules();

            UpdateStuff();
        }

        private void UpdateStuff()
        {
            for (int x = 0; x < Width; x++)
            {
                oldStuff[x] = stuff[x].Clone();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (this[x, y] == PointType.Alive)
                        spriteBatch.Draw(green, new Rectangle((int)(x * SCALE), (int)(y * SCALE), (int)SCALE, (int)SCALE), Color.White);
                }
            }

            var mouse = Mouse.GetState().Position;
            
            var mX = (int)(mouse.X - SCALE / 2);
            var mY = (int)(mouse.Y - SCALE / 2);

            mX = (int)((int)(mX / SCALE) * SCALE);
            mY = (int)((int)(mY / SCALE) * SCALE);

            cursor = new Point(mX, mY);

            var size = new Point((int)SCALE, (int)SCALE);

            spriteBatch.Draw(white, new Rectangle(cursor, size), Color.White);
        }

        private void UpdateRules()
        {
            Parallel.For(0, Width, x =>
            {
                for (int y = 0; y < Height; y++)
                {
                    RuleOne(x, y);
                    RuleTwo(x, y);
                }
            });
        }

        private void RuleOne(int x, int y)
        {
            PointType current = this[x, y];
            if (current == PointType.Invalid || current == PointType.Dead)
                return;

            int aliveNeighbours = GetAliveNeighbourCount(x, y, 4);

            if (aliveNeighbours != 2 && aliveNeighbours != 3)
                this[x, y] = PointType.Dead;
        }

        private void RuleTwo(int x, int y)
        {
            if (this[x, y] == PointType.Dead && GetAliveNeighbourCount(x, y, 4) == 3)
                this[x, y] = PointType.Alive;
        }

        private int GetAliveNeighbourCount(int x, int y, int max)
        {
            int aliveNeighbours = 0;

            for (int nx = -1; nx < 2; nx++)
            {
                for (int ny = -1; ny < 2; ny++)
                {
                    if (nx == 0 && ny == 0)
                        continue;

                    PointType testSubject = this[x + nx, y + ny];
                    if (testSubject == PointType.Invalid)
                        continue;

                    if (testSubject == PointType.Alive)
                        aliveNeighbours++;

                    if (aliveNeighbours == max)
                        return aliveNeighbours;
                }
            }

            return aliveNeighbours;
        }

        private void UpdateControls()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !enterDown)
            {
                enterDown = true;
                GenerateNew(false);
            }
            else if (enterDown && Keyboard.GetState().IsKeyUp(Keys.Enter))
            {
                enterDown = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !spaceDown)
            {
                spaceDown = true;
                run = !run;
            }
            else if (spaceDown && Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                spaceDown = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Back) && !backDown)
            {
                backDown = true;
                GenerateNew(true);
            }
            else if (spaceDown && Keyboard.GetState().IsKeyUp(Keys.Back))
            {
                backDown = false;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !mouseDown)
            {
                mouseDown = true;
                var pos = cursor;
                var cur = this[(int)(pos.X / SCALE), (int)(pos.Y / SCALE)];
                if (cur != PointType.Invalid)
                    this[(int)(pos.X / SCALE), (int)(pos.Y / SCALE)] = cur == PointType.Alive ? PointType.Dead : PointType.Alive;
            }
            else if (mouseDown && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                mouseDown = false;
            }
        }
    }

    internal enum PointType
    {
        Dead,
        Alive,
        Invalid
    }
}
