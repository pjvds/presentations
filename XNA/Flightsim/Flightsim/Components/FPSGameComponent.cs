using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Flightsim.Components
{
    public class FPSGameComponent : DrawableGameComponent
    {
        // De manager waarmee content uit de content pipeline word gehaalt.
        private ContentManager _content;

        // Font waarmee we de FPS gaan tonen.
        private SpriteFont _defaultFont;

        // Batch waar we de sprite font me gaan tekeken.
        private SpriteBatch _spriteBatch;

        // Het aantal frames per seconden.
        private float _fps;

        // Het aantal frames sinds de laatste frameberekening.
        private int _frameCount;

        // Aantal seconden sinds de laatste frameberekening.
        private float _timeSinceLastUpdate;

        public FPSGameComponent(Game game)
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
                // Init sprite batch als deze er nog niet is.
                if (_spriteBatch == null)
                {
                    _spriteBatch = new SpriteBatch(GraphicsDevice);
                }

                // Laad het font in.
                _defaultFont = _content.Load<SpriteFont>("Content\\Fonts\\DefaultFont");
            }

            // Call base.
            base.LoadGraphicsContent(loadAllContent);
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            // Ruim content op, indien nodig.
            if (unloadAllContent)
            {
                // Ruim sprite batch op als deze bestaat.
                if (_spriteBatch != null)
                {
                    _spriteBatch.Dispose();
                    _spriteBatch = null;
                }

                // Ruim alle content op die via de content manager geladen is.
                _content.Unload();
            }

            // Call base.
            base.UnloadGraphicsContent(unloadAllContent);
        }

        public override void Update(GameTime gameTime)
        {
            // Verkrijg aantal secondes sinds laatste update call.
            float elapsedSeconds = (float)gameTime.ElapsedRealTime.TotalSeconds;

            // Hoog de frame count op en aantal seconden sinds laatste berekening.
            _frameCount++;
            _timeSinceLastUpdate += elapsedSeconds;

            // Als er meer dan 10ms verstreken zijn, bereken FPS.
            if (_timeSinceLastUpdate > 0.1f)
            {
                // Bereken FPS en reset counters.
                _fps = _frameCount / _timeSinceLastUpdate;
                _frameCount = 0;
                _timeSinceLastUpdate -= 0.1f;
            }

            // Call base.
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Maak string die we gaan tekenen.
            String fpsString = String.Concat(_fps, " fps");

            // Bereid graphicsdevice voor op het tekenen van sprites.
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);

            // Teken string op het scherm.
            _spriteBatch.DrawString(_defaultFont, fpsString, Vector2.One, Color.White);
            
            // Flush alle tekeningen.
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
