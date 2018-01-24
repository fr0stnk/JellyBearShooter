using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    /// <summary>
    /// A burger
    /// </summary>
    public class Burger
    {
        #region Fields

        // graphic and drawing info
        Texture2D sprite;
        Rectangle drawRectangle;

        // burger stats
        int healthPoints = GameConstants.BurgerInitialHealth;

        // shooting support
        bool canShoot = true;
        int elapsedCooldownMilliseconds = 0;

        // sound effect
        SoundEffect shootSound;


        #endregion

        #region Constructors

        /// <summary>
        ///  Constructs a burger
        /// </summary>
        /// <param name="contentManager">the content manager for loading content</param>
        /// <param name="spriteName">the sprite name</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        /// <param name="shootSound">the sound the burger plays when shooting</param>
        public Burger(ContentManager contentManager, string spriteName, int x, int y,
            SoundEffect shootSound)
        {
            LoadContent(contentManager, spriteName, x, y);
            this.shootSound = shootSound;
        }

        #endregion

        #region Properties

        public int health
        {
            set { if (healthPoints > 0) healthPoints = value; }
            get { return healthPoints; }
        }

        /// <summary>
        /// Gets the collision rectangle for the burger
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the burger's location based on mouse. Also fires 
        /// french fries as appropriate
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="mouse">the current state of the mouse</param>
        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
            // burger should only respond to input if it still has health
            if (health > 0)
            {
                Vector2 velocity = Vector2.Zero; 
                
                // move burger using keyboard
                if (keyboard.IsKeyDown(Keys.D))
                {
                    velocity += new Vector2(1, 0);
                }
                if (keyboard.IsKeyDown(Keys.S))
                {
                    velocity += new Vector2(0, 1);
                }
                if (keyboard.IsKeyDown(Keys.A))
                {
                    velocity += new Vector2(-1, 0);
                }
                if (keyboard.IsKeyDown(Keys.W))
                {
                    velocity += new Vector2(0, -1);
                }

                drawRectangle.X += (int)(velocity.X * GameConstants.BurgerMovementAmount);
                drawRectangle.Y += (int)(velocity.Y * GameConstants.BurgerMovementAmount);

                // clamp burger in window
                if (drawRectangle.Left < 0)
                {
                    drawRectangle.X = 0;
                }
                if (drawRectangle.Right > GameConstants.WindowWidth)
                {
                    drawRectangle.X = GameConstants.WindowWidth - drawRectangle.Width;
                }
                if (drawRectangle.Top < 0)
                {
                    drawRectangle.Y = 0;
                }
                if (drawRectangle.Bottom > GameConstants.WindowHeight)
                {
                    drawRectangle.Y = GameConstants.WindowHeight - drawRectangle.Height;
                }
            }

            // update shooting allowed
            // timer concept (for animations) introduced in Chapter 7
            if (!canShoot)
            {
                elapsedCooldownMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedCooldownMilliseconds >= GameConstants.BurgerTotalCooldownMilliseconds ||
                    keyboard.IsKeyUp(Keys.Space))
                {
                    canShoot = true;
                    elapsedCooldownMilliseconds = 0;
                }
            }

            // shoot if appropriate
            if (health > 0 &&
                keyboard.IsKeyDown(Keys.Space) &&
                canShoot)
            {
                canShoot = false;
                Projectile projectile = new Projectile(ProjectileType.FrenchFries,
                    Game1.GetProjectileSprite(ProjectileType.FrenchFries),
                    drawRectangle.Center.X,
                    drawRectangle.Center.Y - GameConstants.FrenchFriesProjectileOffset,
                    -GameConstants.FrenchFriesProjectileSpeed);
                Game1.AddProjectile(projectile);
            }
        }

        /// <summary>
        /// Draws the burger
        /// </summary>
        /// <param name="spriteBatch">the sprite batch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, drawRectangle, Color.White);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the burger
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the burger</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        private void LoadContent(ContentManager contentManager, string spriteName,
            int x, int y)
        {
            // load content and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            drawRectangle = new Rectangle(x - sprite.Width / 2,
                y - sprite.Height / 2, sprite.Width,
                sprite.Height);
        }

        #endregion
    }
}
