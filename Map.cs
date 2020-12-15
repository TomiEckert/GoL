using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gol
{
    internal class Map
    {
        private const double CHANCE = 0.04;
        private const double SCALE = 1;

        private int Width { get; set; }
        private int Height { get; set; }

        private bool spaceDown;
        private Random random;
        private Texture2D green;

        public Map(int width, int height, GraphicsDeviceManager graphics)
        {
            random = new Random();
            Width = width;
            Height = height;

            green = new Texture2D(graphics.GraphicsDevice, 1, 1);
            Color[] data = new Color[1 * 1];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                data[pixel] = Color.Green;
            }
            green.SetData(data);
            GenerateNew();
        }
        public Map(GraphicsDeviceManager graphics)
        {
            random = new Random();
            Width = (int)Math.Floor(graphics.GraphicsDevice.Viewport.Width / SCALE);
            Height = (int)Math.Floor(graphics.GraphicsDevice.Viewport.Height / SCALE);

            green = new Texture2D(graphics.GraphicsDevice, 1, 1);
            Color[] data = new Color[1 * 1];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                data[pixel] = Color.Green;
            }
            green.SetData(data);
            GenerateNew();
        }

        private List<List<PointType>> stuff;

        public PointType this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1)
                    return PointType.Invalid;
                return stuff[x][y];
            }
            set
            {
                stuff[x][y] = value;
            }
        }

        private void GenerateNew()
        {
            stuff = new List<List<PointType>>();
            for (int x = 0; x < Width; x++)
            {
                List<PointType> temp = new List<PointType>();
                for (int y = 0; y < Height; y++)
                {
                    if (random.NextDouble() < CHANCE)
                        temp.Add(PointType.Alive);
                    else
                        temp.Add(PointType.Dead);
                }
                stuff.Add(temp);
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateControls();

            UpdateRules();
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
        }

        private void UpdateRules()
        {
            for (int x = 0; x < stuff.Count; x++)
            {
                for (int y = 0; y < stuff[x].Count; y++)
                {
                    RuleOne(x, y);
                    RuleTwo(x, y);
                }
            }
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
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !spaceDown)
            {
                spaceDown = true;
                GenerateNew();
            }
            else if (spaceDown && Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                spaceDown = false;
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
