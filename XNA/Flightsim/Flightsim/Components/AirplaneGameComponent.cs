using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Flightsim.Components
{
    public class AirplaneGameComponent : DrawableGameComponent
    {
        // De manager waarmee content uit de content pipeline word gehaalt.
        private ContentManager _content;

        // Het 3D model.
        private Model _model;

        // View matrix wordt gebruikt voor de tranformatie van world space naar view space. 
        // Het specificeerd een posititie in de world vanuit waar gekeken wordt.
        private Matrix _view;

        // Projection matrix specificeerd hoe de 3D view data naar 2D beeld wordt gevormt.
        private Matrix _projection;

        // De positie van het vliegtuig.
        private Vector3 _position = new Vector3(8, 2, 1);

        // De rotatie van het vliegtuig.
        private Quaternion _rotation = Quaternion.Identity;

        // De schaal voor ons model.
        private Vector3 _scales = Vector3.One;

        // De rotatie om de Z as.
        private float _roll = 0f;

        // De rotatie om de X as.
        private float _pitch = 0f;

        // De engine de verandwoordelijk is voor het geluid.
        private AudioEngine _audioEngine;

        // Een collectie van geluid informatie.
        private WaveBank _waveBank;

        // Een collectie van cues (geluiden).
        private SoundBank _soundBank;

        // Een Cue voor het motor geluid (geluid zelf).
        private Cue _engineSound = null;

        // Laatste state van button A.
        private ButtonState _previousAButtonState = ButtonState.Released;

        // Is hyperspeed enabled.
        private bool _hyperspeed;

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

        /// <summary>
        /// Gets or sets the position of the airplane.
        /// </summary>
        /// <remarks>
        /// Dit representeerd de View matrix, de positie van het model.
        /// </remarks>
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the airplane.
        /// </summary>
        /// <remarks>
        /// De rotatie van onze airplane.
        /// </remarks>
        public Quaternion Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        /// <summary>
        /// Gets the current hyperspeed state.
        /// </summary>
        public bool Hyperspeed
        {
            get
            {
                return _hyperspeed;
            }
        }

        public AirplaneGameComponent(Game game)
            : base(game)
        {
            // Init een nieuwe content manager voor de gespecificeerde game.
            _content = new ContentManager(game.Services);
        }

        public override void Initialize()
        {
            _audioEngine = new AudioEngine("Content\\Audio\\MyGameAudio.xgs");
            _waveBank = new WaveBank(_audioEngine, "Content\\Audio\\Wave Bank.xwb");
            _soundBank = new SoundBank(_audioEngine, "Content\\Audio\\Sound Bank.xsb");

            base.Initialize();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            // Laad content, indien nodig.
            if (loadAllContent)
            {
                // Laad het model in.
                _model = _content.Load<Model>("Content\\Models\\airplane");
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



        public override void Update(GameTime gameTime)
        {
            // Haal de state op van gamepad 1.
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            if (currentState.IsConnected)
            {
                // Laat de controller trillen, afhankelijk van hoeveel 'gas' we geven.
                GamePad.SetVibration(PlayerIndex.One, currentState.Triggers.Right, currentState.Triggers.Right);

                // Speel geluid als de rechter trigger ingedrukt is.
                if (currentState.Triggers.Right > 0)
                {
                    // Init geluid als deze er nog niet is.
                    if (_engineSound == null)
                    {
                        _engineSound = _soundBank.GetCue("engine_2");
                        _engineSound.Play();
                    }

                    // Als het geluid gepauzeerd is, resume.
                    else if (_engineSound.IsPaused)
                    {
                        _engineSound.Resume();
                    }
                }
                else
                {
                    // Als het geluid aanwezig is en het speelt op dit moment af, pauzeer.
                    if (_engineSound != null && _engineSound.IsPlaying)
                    {
                        _engineSound.Pause();
                    }
                }

                // Speel geluid als de rechter trigger ingedrukt is.
                if (currentState.Buttons.A == ButtonState.Pressed)
                {
                    if (_previousAButtonState == ButtonState.Released)
                    {
                        // Speel geluid af.
                        _soundBank.PlayCue("hyperspace_activate");
                        
                        // Set in hyperspeed.
                        _hyperspeed = true;
                    }
                }
                else
                {
                    _hyperspeed = false;
                }

                // Set de huidige state voor button A.
                _previousAButtonState = currentState.Buttons.A;

                // Set de roll adhv de linker stick en verminder dit iets.
                _roll += -currentState.ThumbSticks.Left.X * 3;
                _roll *= 0.90f;

                // Set de pitch adhv de linker stick en verminder dit iets.
                _pitch += -currentState.ThumbSticks.Left.Y * 3;
                _pitch *= 0.90f;
            }

            // Call base.
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Voor elke mesh in het model, teken.
            foreach (ModelMesh modmesh in _model.Meshes)
            {
                // Voor elk effect in de huidige mesh, teken.
                foreach (BasicEffect currenteffect in modmesh.Effects)
                {
                    // Gebruik standaard lighting.
                    currenteffect.EnableDefaultLighting();

                    // World matrix wordt gebruikt voor de tranformatie van model space naar world space.
                    Matrix drawingWorld = Matrix.CreateScale(_scales) *
                        Matrix.CreateRotationX(MathHelper.ToRadians(_pitch)) *
                        Matrix.CreateRotationX(MathHelper.ToRadians(90f)) *
                        Matrix.CreateFromQuaternion(Rotation) *
                        Matrix.CreateTranslation(Position);

                    // Set World, View en Projection informatie.
                    currenteffect.World = drawingWorld;
                    currenteffect.View = View *
                        Matrix.CreateRotationZ(MathHelper.ToRadians(_roll));

                    // Set de projection.
                    currenteffect.Projection = Projection;
                }
                
                // Teken de mesh.
                modmesh.Draw();
            }

            // Call base.
            base.Draw(gameTime);
        }
    }
}