﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerProject.Source
{
    public class MainMenu
    {
        public static event StringDelegate OnServerStartRequested;
        public static event StringDelegate OnClientStartRequested;

        private SpriteFont _font;
        private string _message;
        private bool _awaitingInput;

        public MainMenu()
        {
            _awaitingInput = true;

            _message = "Press S to be a server \nPress C to be a client";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, _message, new Vector2(161, 161), Color.Black);
        }

        public void Initalise(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _font = content.Load<SpriteFont>("Font");
        }

        public void ProcessInput(GameTime gameTime, InputInformation inputInfo)
        {
            if (!_awaitingInput)
                return;

            if (inputInfo.CurrentKeyboardState.IsKeyDown(Keys.S))
            {
                OnServerStartRequested("str");
                _awaitingInput = false;
            }
            else if (inputInfo.CurrentKeyboardState.IsKeyDown(Keys.C))
            {
                OnClientStartRequested("str");
                _awaitingInput = false;
            }
        }

        public void Update(GameTime gameTime)
        {
           
        }

        public void SetMessage(string message)
        {
            _message = message;
        }

        public void Reset()
        {
            _awaitingInput = true;
        }
    }
}
