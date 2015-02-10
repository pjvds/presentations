using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Flightsim.Components
{
    public class SkyboxGameComponent : DrawableGameComponent
    {
        // De manager waarmee content uit de content pipeline word gehaalt.
        private ContentManager _content;

        // Het model van de skybox.
        private Model _skyboxModel;

        // De positie van de skybox.
        private Matrix _world;

        // View matrix wordt gebruikt voor de tranformatie van world space naar view space. 
        // Het specificeerd een posititie in de world vanuit waar gekeken wordt.
        private Matrix _view;

        // Projection matrix specificeerd hoe de 3D view data naar 2D beeld wordt gevormt.
        private Matrix _projection;

        // De positie van het vliegtuig.
        private Vector3 _position = new Vector3(8, 2, 1);

        // De schaal voor ons model.
        private Vector3 _scales = Vector3.One;

        /// <summary>
        /// Gets or sets the Word Matrix.
        /// </summary>
        /// <remarks>
        /// World matrix wordt gebruikt om de positie te bepalen voor het model.
        /// </remarks>
        public Matrix World
        {
            get
            {
                return _world;
            }
            set
            {
                _world = value;
            }
        }

        /// <summary>
        /// Gets or sets the View Matrix.
        /// </summary>
        /// <remarks>
        /// View matrix wordt gebruikt voor de tranformatie van world space naar view space. 
        /// Het specificeerd een posititie in de world vanuit waar gekeken wordt.
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
        
        public SkyboxGameComponent(Game game)
            : base(game)
        {
            // Init een nieuwe content manager voor de gespecificeerde game.
            _content = new ContentManager(game.Services);
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            // Laad content, indien nodig.
            if (loadAllContent)
            {
                // Laad het model in.
                _skyboxModel = _content.Load<Model>("Content\\Models\\skybox");
            }

            // Call base.
            base.LoadGraphicsContent(loadAllContent);
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            // Ruim content op, indien nodig.
            if (unloadAllContent)
            {
                // Ruim alle content op die via de content manager geladen is.
                _content.Unload();
            }

            // Call base.
            base.UnloadGraphicsContent(unloadAllContent);
        }

        public override void Draw(GameTime gameTime)
        {
            // Voor elke mesh in het model, teken.
            foreach (ModelMesh modmesh in _skyboxModel.Meshes)
            {
                // Voor elk effect in de huidige mesh, teken.
                foreach (BasicEffect currenteffect in modmesh.Effects)
                {
                    // World matrix wordt gebruikt voor de tranformatie van model space naar world space.
                    Matrix drawingWorld = World * Matrix.CreateScale(_scales) *
                        //Matrix.CreateRotationX(MathHelper.ToRadians(pitch)) *
                        Matrix.CreateRotationX(MathHelper.ToRadians(90f));// *
                        //Matrix.CreateFromQuaternion(Rotation) *
                        //Matrix.CreateTranslation(Position);

                    // Set World, View en Projection informatie.
                    currenteffect.World = Matrix.CreateScale(20f, 10, 20) * World;
                    currenteffect.View = View;// *
                        //Matrix.CreateRotationZ(MathHelper.ToRadians(roll));

                    currenteffect.Projection = Projection;
                }
                modmesh.Draw();
            }

            base.Draw(gameTime);

        }
    }
}