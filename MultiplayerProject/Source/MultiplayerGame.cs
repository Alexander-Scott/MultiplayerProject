﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using MultiplayerProject.Source;

namespace MultiplayerProject
{
    public class MultiplayerGame : Game
    {
        GraphicsDeviceManager   _graphics;
        SpriteBatch             _spriteBatch;
        Player                  _player;

        KeyboardState           _currentKeyboardState;
        KeyboardState           _previousKeyboardState;

        GamePadState            _currentGamePadState;
        GamePadState            _previousGamePadState;

        MouseState              _currentMouseState;
        MouseState              _previousMouseState;

        float                   _playerMoveSpeed;

        EnemyManager            _enemyManager;
        LaserManager            _laserManager;
        CollisionManager        _collisionManager;
        ExplosionManager        _explosionManager;

        // Image used to display the static background
        Texture2D mainBackground;
        Rectangle rectBackground;

        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

        public MultiplayerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Initialize the player class
            _player = new Player();

            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();
            rectBackground = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            _playerMoveSpeed = 8.0f;
            TouchPanel.EnabledGestures = GestureType.FreeDrag;

            _enemyManager = new EnemyManager();
            _enemyManager.Initalise(Content, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            _laserManager = new LaserManager();
            _laserManager.Initalise(Content, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            _collisionManager = new CollisionManager();

            _explosionManager = new ExplosionManager();
            _explosionManager.Initalise(Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            _player.Initialize(playerAnimation, playerPosition);

            bgLayer1.Initialize(Content, "bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
            bgLayer2.Initialize(Content, "bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
            mainBackground = Content.Load<Texture2D>("mainbackground");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _previousGamePadState = _currentGamePadState;
            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;

            _currentKeyboardState = Keyboard.GetState();
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
            _currentMouseState = Mouse.GetState();

            UpdatePlayer(gameTime);

            // Update the parallaxing background
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            _enemyManager.Update(gameTime);
            _laserManager.Update(gameTime);
            _explosionManager.Update(gameTime);

            _collisionManager.CheckCollision(_enemyManager.Enemies, _laserManager.Lasers, _explosionManager);

            base.Update(gameTime);
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            _player.Update(gameTime);

            // Windows 8 Touch Gestures for MonoGame
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();

                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    _player.Position += gesture.Delta;
                }
            }

            //Get Mouse State then Capture the Button type and Respond Button Press
            Vector2 mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);

            if (_currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 posDelta = mousePosition - _player.Position;

                posDelta.Normalize();
                posDelta = posDelta * _playerMoveSpeed;

                _player.Position = _player.Position + posDelta;
            }

            // Thumbstick controls
            _player.Position.X += _currentGamePadState.ThumbSticks.Left.X * _playerMoveSpeed;
            _player.Position.Y -= _currentGamePadState.ThumbSticks.Left.Y * _playerMoveSpeed;

            // Keyboard/Dpad controls
            if (_currentKeyboardState.IsKeyDown(Keys.Left) || _currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                _player.Position.X -= _playerMoveSpeed;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Right) || _currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                _player.Position.X += _playerMoveSpeed;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Up) || _currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                _player.Position.Y -= _playerMoveSpeed;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Down) || _currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                _player.Position.Y += _playerMoveSpeed;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.Space) || _currentGamePadState.Buttons.X == ButtonState.Pressed)
            {
                _laserManager.FireLaser(gameTime, _player.Position);
            }

            // Make sure that the player does not go out of bounds
            _player.Position.X = MathHelper.Clamp(_player.Position.X, 0, GraphicsDevice.Viewport.Width);
            _player.Position.Y = MathHelper.Clamp(_player.Position.Y, 0, GraphicsDevice.Viewport.Height);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Start drawing
            _spriteBatch.Begin();

            //Draw the Main Background Texture
            _spriteBatch.Draw(mainBackground, rectBackground, Color.White);

            // Draw the moving background
            bgLayer1.Draw(_spriteBatch);
            bgLayer2.Draw(_spriteBatch);

            _enemyManager.Draw(_spriteBatch);

            _laserManager.Draw(_spriteBatch);

            _explosionManager.Draw(_spriteBatch);

            // Draw the Player
            _player.Draw(_spriteBatch);

            // Stop drawing
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
