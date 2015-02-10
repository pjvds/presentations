using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Flightsim.Components
{
    public class CityGameCompnent : DrawableGameComponent
    {
        #region Members
        // De manager waarmee content uit de content pipeline word gehaalt.
        private ContentManager _content;

        // De texture die de plaatjes bevat voor de grond en gebouwen.
        private Texture2D _texture;
        
        // De breedte van de stad.
        private int _width;

        // De lengte van de stad.
        private int _height;
        
        // Het aantal gebouwen.
        private int _numberOfBuildings = 5;

        // De hoogte van elk gebouw.
        private int[] buildingheights = new int[] { 0, 6, 2, 5, 2, 4 };

        // De vertices waaruit onze stad is opgebouwd.
        private VertexPositionNormalTexture[] _vertices;

        // De schaal voor ons model.
        private Vector3 _scales = Vector3.One;

        // View matrix wordt gebruikt voor de tranformatie van world space naar view space. 
        // Het specificeerd een posititie in de world vanuit waar gekeken wordt.
        private Matrix _view;

        // Projection matrix specificeerd hoe de 3D view data naar 2D beeld wordt gevormd.
        private Matrix _projection;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the View Matrix.
        /// </summary>
        /// <remarks>
        /// De view matrix kun je zien als de positie van de camera.
        /// </remarks>
        public Matrix View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
            }
        }

        /// <summary>
        /// Gets or sets the Projection Matrix.
        /// </summary>
        /// <remarks>
        /// Projection matrix specificeerd hoe de 3D view data naar 2D beeld wordt gevormd.
        /// </remarks>
        public Matrix Projection
        {
            get
            {
                return _projection;
            }
            set
            {
                _projection = value;
            }
        }

        /// <summary>
        /// Gets or sets the Scales that will be used for the Model.
        /// </summary>
        /// <remarks>
        /// De schaal voor ons model.
        /// </remarks>
        public Vector3 Scales
        {
            get
            {
                return _scales;
            }
            set
            {
                _scales = value;
            }
        }
        #endregion

        #region Constructor
        public CityGameCompnent(Game game) : base(game)
        {
            // Init een nieuwe content manager voor de gespecificeerde game.
            _content = new ContentManager(game.Services);
        }
        #endregion

        public override void Initialize()
        {
            // Laad de vertices voor de stad en de texture.
            _vertices = GetVertices();
            _texture = _content.Load<Texture2D>("Content\\Textures\\texturemap");

            // Call base.
            base.Initialize();
        }

        private VertexPositionNormalTexture[] GetVertices()
        {
            // Laad het stads plan.
            int[,] cityPlan = LoadCityFromFile();
            List<VertexPositionNormalTexture> verticeslist = new List<VertexPositionNormalTexture>();
            float imagesintexture = 1 + _numberOfBuildings * 2;

            // Bouw vertices op.
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int currentbuilding = cityPlan[x, y];

                    verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, buildingheights[currentbuilding]), new Vector3(0, 0, 1), new Vector2(currentbuilding * 2 / imagesintexture, 0)));
                    verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, buildingheights[currentbuilding]), new Vector3(0, 0, 1), new Vector2(currentbuilding * 2 / imagesintexture, 1)));
                    verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, buildingheights[currentbuilding]), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 + 1) / imagesintexture, 1)));

                    verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, buildingheights[currentbuilding]), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 + 1) / imagesintexture, 1)));
                    verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y + 1, buildingheights[currentbuilding]), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 + 1) / imagesintexture, 0)));
                    verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, buildingheights[currentbuilding]), new Vector3(0, 0, 1), new Vector2(currentbuilding * 2 / imagesintexture, 0)));

                    if (y > 0)
                    {
                        if (cityPlan[x, y - 1] != cityPlan[x, y])
                        {
                            if (cityPlan[x, y - 1] > 0)
                            {
                                currentbuilding = cityPlan[x, y - 1];
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, 0f), new Vector3(0, 1, 0), new Vector2(currentbuilding * 2 / imagesintexture, 1)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, buildingheights[currentbuilding]), new Vector3(0, 1, 0), new Vector2(currentbuilding * 2 / imagesintexture, 0)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0f), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 1)));

                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, buildingheights[currentbuilding]), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 0)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0f), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 1)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, buildingheights[currentbuilding]), new Vector3(0, 1, 0), new Vector2(currentbuilding * 2 / imagesintexture, 0)));
                            }
                            if (cityPlan[x, y] > 0)
                            {
                                currentbuilding = cityPlan[x, y];
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, buildingheights[currentbuilding]), new Vector3(0, -1, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 0)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, 0f), new Vector3(0, -1, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 1)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0f), new Vector3(0, -1, 0), new Vector2(currentbuilding * 2 / imagesintexture, 1)));

                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, buildingheights[currentbuilding]), new Vector3(0, -1, 0), new Vector2(currentbuilding * 2 / imagesintexture, 0)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, buildingheights[currentbuilding]), new Vector3(0, -1, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 0)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0f), new Vector3(0, -1, 0), new Vector2(currentbuilding * 2 / imagesintexture, 1)));
                            }
                        }
                    }
                    if (x > 0)
                    {
                        if (cityPlan[x - 1, y] != cityPlan[x, y])
                        {
                            if (cityPlan[x - 1, y] > 0)
                            {
                                currentbuilding = cityPlan[x - 1, y];
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y + 1, buildingheights[currentbuilding]), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 0)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y + 1, 0f), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 1)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0f), new Vector3(1, 0, 0), new Vector2(currentbuilding * 2 / imagesintexture, 1)));

                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0f), new Vector3(1, 0, 0), new Vector2(currentbuilding * 2 / imagesintexture, 1)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, buildingheights[currentbuilding]), new Vector3(1, 0, 0), new Vector2(currentbuilding * 2 / imagesintexture, 0)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y + 1, buildingheights[currentbuilding]), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 0)));
                            }
                            if (cityPlan[x, y] > 0)
                            {
                                currentbuilding = cityPlan[x, y];
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y + 1, 0f), new Vector3(-1, 0, 0), new Vector2(currentbuilding * 2 / imagesintexture, 1)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, buildingheights[currentbuilding]), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 0)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0f), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 1)));

                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y + 1, 0f), new Vector3(-1, 0, 0), new Vector2(currentbuilding * 2 / imagesintexture, 1)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y + 1, buildingheights[currentbuilding]), new Vector3(-1, 0, 0), new Vector2(currentbuilding * 2 / imagesintexture, 0)));
                                verticeslist.Add(new VertexPositionNormalTexture(new Vector3(x, y, buildingheights[currentbuilding]), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesintexture, 0)));
                            }
                        }
                    }
                }
            }

            return verticeslist.ToArray();
        }

        private int[,] LoadCityFromFile()
        {
            // Lees het level bestand uit.
            String[] lines = File.ReadAllLines("Content\\Levels\\level.txt");

            // Zet de oppervlakte grootte van de stad.
            _width = lines[0].Length;
            _height = lines.Length;
            int[,] cityPlan = new int[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    // lees het nummer in de huidige positie.
                    int number = int.Parse(lines[y][x].ToString());
                    cityPlan[x, y] = number;
                }
            }

            return cityPlan;
        }

        public override void Draw(GameTime gameTime)
        {
            // Init standaard effect en set matrices.
            BasicEffect effect = new BasicEffect(GraphicsDevice, null);
            effect.World = Matrix.CreateScale(_scales) * Matrix.Identity;
            effect.View = View;
            effect.Projection = Projection;

            // Set texture en enable textures voor dit effect.
            effect.Texture = _texture;
            effect.TextureEnabled = true;

            // Begin effect en loop alle passes door.
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                // Begin huidige pass.
                pass.Begin();

                // Laad vertices in het device en teken ze.
                GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionNormalTexture.VertexElements);
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length / 3);

                // Eindig huidige pass.
                pass.End();
            }
            
            // Eindig effect en ruim het op.
            effect.End();
            effect.Dispose();

            // Call base
            base.Draw(gameTime);
        }
    }
}