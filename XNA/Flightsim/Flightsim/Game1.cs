using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using Flightsim.Components;

namespace XNAseries2
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;

        /// <summary>
        /// View matrix wordt gebruikt voor de tranformatie van world space naar view space. 
        /// Het specificeerd een posititie in de world vanuit waar gekeken wordt.
        /// </summary>
        private Matrix _viewMatrix;

        /// <summary>
        /// Projection matrix specificeerd hoe de 3D view data naar 2D beeld wordt gevormd.
        /// </summary>
        private Matrix _projectionMatrix;

        /// <summary>
        /// De snelheid van ons airplane.
        /// </summary>
        float _airplaneVelocity = 0f;

        /// <summary>
        /// De airplane.
        /// </summary>
        AirplaneGameComponent _airplane;

        /// <summary>
        /// De stad/level.
        /// </summary>
        CityGameCompnent _city;

        /// <summary>
        /// De skybox.
        /// </summary>
        SkyboxGameComponent _skybox;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
        }


        protected override void Initialize()
        {
            // Init airplane component.
            _airplane = new AirplaneGameComponent(this);
            _airplane.Scales = new Vector3(0.00001f);
            _airplane.UpdateOrder = 20;
            _airplane.DrawOrder = 20;
            Components.Add(_airplane);

            // Init city component.
            _city = new CityGameCompnent(this);
            _city.Scales = new Vector3(1.8f);
            _city.UpdateOrder = 10;
            _city.DrawOrder = 10;
            Components.Add(_city);

            // Init skybox component.
            _skybox = new SkyboxGameComponent(this);
            _skybox.Scales = new Vector3(1f);
            _skybox.UpdateOrder = 5;
            _skybox.DrawOrder = 5;
            Components.Add(_skybox);

            // Call base.
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // Haal de state op van gamepad 1.
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            // Reset airplane als er op Back is gedrukt.
            if (state.Buttons.Back == ButtonState.Pressed)
            {
                _airplane.Position = new Vector3(8, 2, 4);
                _airplane.Rotation = Quaternion.Identity;
                _airplaneVelocity = 0;
            }

            // Als de airplane niet in hyperspeed is.
            if (!_airplane.Hyperspeed)
            {
                // Als de snelheid niet hoger is dan 5.
                if (_airplaneVelocity < 5)
                {
                    // Voeg snelheid toe afhankelijk van de state van de rechter trigger.
                    _airplaneVelocity += state.Triggers.Right;
                }

                // Haal de snelheid een klein stukje omlaag.
                _airplaneVelocity *= 0.98f;
            }
            else
            {
                // Voeg snelheid toe afhankelijk van de state van de rechter trigger.
                _airplaneVelocity += state.Triggers.Right;

                // Zorg er voor dat de snelheid niet boven de 50 uitkomt.
                _airplaneVelocity = MathHelper.Min(_airplaneVelocity, 50);
            }

            // Bereken nieuwe rotatie voor de airplane.
            Vector2 rotation = new Vector2(0);
            rotation.X = -state.ThumbSticks.Left.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * Math.Max(Math.Min(_airplaneVelocity, 5) / 2, 1);
            rotation.Y = state.ThumbSticks.Left.X * (float)gameTime.ElapsedGameTime.TotalSeconds * Math.Max(Math.Min(_airplaneVelocity, 5) / 2, 1);

            // Voeg rotatie toe aan de airplane.
            _airplane.Rotation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), rotation.Y);
            _airplane.Rotation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 1f, 0), rotation.Y) * Quaternion.CreateFromAxisAngle(new Vector3(1f, 0, 0), rotation.X);

            // Bereken positie verschil met de rotatie.
            Vector3 addPosition = Vector3.Transform(Vector3.UnitY, Matrix.CreateFromQuaternion(_airplane.Rotation));
            addPosition.Normalize();

            // Verplaats de airplane.
            _airplane.Position += addPosition * ((float)gameTime.ElapsedGameTime.TotalSeconds / 2f * (_airplaneVelocity * 3));

            // Update camera positie.
            Vector3 cameraPosition = new Vector3(0, -0.1f, 0.02f);
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateFromQuaternion(_airplane.Rotation));
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateTranslation(_airplane.Position));

            // Update camera up (hoek) positie.
            Vector3 cameraUpPosition = new Vector3(0, 0, 1f);
            cameraUpPosition = Vector3.Transform(cameraUpPosition, Matrix.CreateFromQuaternion(_airplane.Rotation));

            // Set view en projection matrix.
            _viewMatrix = Matrix.CreateLookAt(cameraPosition, _airplane.Position, cameraUpPosition);
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.Window.ClientBounds.Width / (float)this.Window.ClientBounds.Height, 0.001f, 500000.0f);

            // Set matrices voor de airplane.
            _airplane.Projection = _projectionMatrix;
            _airplane.View = _viewMatrix;

            // Set matrices voor de city.
            _city.Projection = _projectionMatrix;
            _city.View = _viewMatrix;

            // Set matrices voor de skybox.
            _skybox.World = Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(_airplane.Position);
            _skybox.Projection = _projectionMatrix;
            _skybox.View = _viewMatrix;

            // Call base.
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Maak het volledige scherm blauw.
            _graphics.GraphicsDevice.Clear(Color.DarkSlateBlue);

            // Call base.
            base.Draw(gameTime);
        }
    }
}