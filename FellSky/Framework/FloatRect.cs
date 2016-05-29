using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{
    public struct FloatRect
    {
        #region Private Fields

        private static FloatRect emptyFloatRect = new FloatRect();

        #endregion

        #region Public Fields

        /// <summary>
        /// The x coordinate of the top-left corner of this <see cref="FloatRect"/>.
        /// </summary>
        public float X;

        /// <summary>
        /// The y coordinate of the top-left corner of this <see cref="FloatRect"/>.
        /// </summary>
        public float Y;

        /// <summary>
        /// The width of this <see cref="FloatRect"/>.
        /// </summary>
        public float Width;

        /// <summary>
        /// The height of this <see cref="FloatRect"/>.
        /// </summary>
        public float Height;

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns a <see cref="FloatRect"/> with X=0, Y=0, Width=0, Height=0.
        /// </summary>
        public static FloatRect Empty
        {
            get { return emptyFloatRect; }
        }

        /// <summary>
        /// Returns the x coordinate of the left edge of this <see cref="FloatRect"/>.
        /// </summary>
        public float Left
        {
            get { return this.X; }
        }

        /// <summary>
        /// Returns the x coordinate of the right edge of this <see cref="FloatRect"/>.
        /// </summary>
        public float Right
        {
            get { return (this.X + this.Width); }
        }

        /// <summary>
        /// Returns the y coordinate of the top edge of this <see cref="FloatRect"/>.
        /// </summary>
        public float Top
        {
            get { return this.Y; }
        }

        /// <summary>
        /// Returns the y coordinate of the bottom edge of this <see cref="FloatRect"/>.
        /// </summary>
        public float Bottom
        {
            get { return (this.Y + this.Height); }
        }

        /// <summary>
        /// Whether or not this <see cref="FloatRect"/> has a <see cref="Width"/> and
        /// <see cref="Height"/> of 0, and a <see cref="Location"/> of (0, 0).
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return ((((this.Width == 0) && (this.Height == 0)) && (this.X == 0)) && (this.Y == 0));
            }
        }

        /// <summary>
        /// The top-left coordinates of this <see cref="FloatRect"/>.
        /// </summary>
        public Vector2 Location
        {
            get
            {
                return new Vector2(this.X, this.Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// The width-height coordinates of this <see cref="FloatRect"/>.
        /// </summary>
        public Vector2 Size
        {
            get
            {
                return new Vector2(this.Width, this.Height);
            }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        /// <summary>
        /// A <see cref="Vector2"/> located in the center of this <see cref="FloatRect"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="Width"/> or <see cref="Height"/> is an odd number,
        /// the center point will be rounded down.
        /// </remarks>
        public Vector2 Center
        {
            get
            {
                return new Vector2(this.X + (this.Width / 2), this.Y + (this.Height / 2));
            }
        }

        #endregion

        #region Internal Properties

        internal string DebugDisplayString
        {
            get
            {
                return string.Concat(
                    this.X, "  ",
                    this.Y, "  ",
                    this.Width, "  ",
                    this.Height
                    );
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="FloatRect"/> struct, with the specified
        /// position, width, and height.
        /// </summary>
        /// <param name="x">The x coordinate of the top-left corner of the created <see cref="FloatRect"/>.</param>
        /// <param name="y">The y coordinate of the top-left corner of the created <see cref="FloatRect"/>.</param>
        /// <param name="width">The width of the created <see cref="FloatRect"/>.</param>
        /// <param name="height">The height of the created <see cref="FloatRect"/>.</param>
        public FloatRect(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Creates a new instance of <see cref="FloatRect"/> struct, with the specified
        /// location and size.
        /// </summary>
        /// <param name="location">The x and y coordinates of the top-left corner of the created <see cref="FloatRect"/>.</param>
        /// <param name="size">The width and height of the created <see cref="FloatRect"/>.</param>
        public FloatRect(Vector2 location, Vector2 size)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Width = size.X;
            this.Height = size.Y;
        }

        public FloatRect(Rectangle rect) : this()
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets whether or not the provided coordinates lie within the bounds of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><c>true</c> if the provided coordinates lie inside this <see cref="FloatRect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(int x, int y)
        {
            return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
        }

        /// <summary>
        /// Gets whether or not the provided coordinates lie within the bounds of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><c>true</c> if the provided coordinates lie inside this <see cref="FloatRect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(float x, float y)
        {
            return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Point"/> lies within the bounds of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="FloatRect"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Point"/> lies inside this <see cref="FloatRect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(Point value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Point"/> lies within the bounds of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="FloatRect"/>.</param>
        /// <param name="result"><c>true</c> if the provided <see cref="Point"/> lies inside this <see cref="FloatRect"/>; <c>false</c> otherwise. As an output parameter.</param>
        public void Contains(ref Point value, out bool result)
        {
            result = ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Vector2"/> lies within the bounds of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="FloatRect"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Vector2"/> lies inside this <see cref="FloatRect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(Vector2 value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Vector2"/> lies within the bounds of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="FloatRect"/>.</param>
        /// <param name="result"><c>true</c> if the provided <see cref="Vector2"/> lies inside this <see cref="FloatRect"/>; <c>false</c> otherwise. As an output parameter.</param>
        public void Contains(ref Vector2 value, out bool result)
        {
            result = ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="FloatRect"/> lies within the bounds of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="value">The <see cref="FloatRect"/> to check for inclusion in this <see cref="FloatRect"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="FloatRect"/>'s bounds lie entirely inside this <see cref="FloatRect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(FloatRect value)
        {
            return ((((this.X <= value.X) && ((value.X + value.Width) <= (this.X + this.Width))) && (this.Y <= value.Y)) && ((value.Y + value.Height) <= (this.Y + this.Height)));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="FloatRect"/> lies within the bounds of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="value">The <see cref="FloatRect"/> to check for inclusion in this <see cref="FloatRect"/>.</param>
        /// <param name="result"><c>true</c> if the provided <see cref="FloatRect"/>'s bounds lie entirely inside this <see cref="FloatRect"/>; <c>false</c> otherwise. As an output parameter.</param>
        public void Contains(ref FloatRect value, out bool result)
        {
            result = ((((this.X <= value.X) && ((value.X + value.Width) <= (this.X + this.Width))) && (this.Y <= value.Y)) && ((value.Y + value.Height) <= (this.Y + this.Height)));
        }

        /// <summary>
        /// Adjusts the edges of this <see cref="FloatRect"/> by specified horizontal and vertical amounts. 
        /// </summary>
        /// <param name="horizontalAmount">Value to adjust the left and right edges.</param>
        /// <param name="verticalAmount">Value to adjust the top and bottom edges.</param>
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2;
            Height += verticalAmount * 2;
        }

        /// <summary>
        /// Gets whether or not the other <see cref="FloatRect"/> intersects with this FloatRect.
        /// </summary>
        /// <param name="value">The other FloatRect for testing.</param>
        /// <returns><c>true</c> if other <see cref="FloatRect"/> intersects with this FloatRect; <c>false</c> otherwise.</returns>
        public bool Intersects(FloatRect value)
        {
            return value.Left < Right &&
                   Left < value.Right &&
                   value.Top < Bottom &&
                   Top < value.Bottom;
        }


        /// <summary>
        /// Gets whether or not the other <see cref="FloatRect"/> intersects with this FloatRect.
        /// </summary>
        /// <param name="value">The other FloatRect for testing.</param>
        /// <param name="result"><c>true</c> if other <see cref="FloatRect"/> intersects with this FloatRect; <c>false</c> otherwise. As an output parameter.</param>
        public void Intersects(ref FloatRect value, out bool result)
        {
            result = value.Left < Right &&
                     Left < value.Right &&
                     value.Top < Bottom &&
                     Top < value.Bottom;
        }

        /// <summary>
        /// Creates a new <see cref="FloatRect"/> that contains overlapping region of two other FloatRects.
        /// </summary>
        /// <param name="value1">The first <see cref="FloatRect"/>.</param>
        /// <param name="value2">The second <see cref="FloatRect"/>.</param>
        /// <returns>Overlapping region of the two FloatRects.</returns>
        public static FloatRect Intersect(FloatRect value1, FloatRect value2)
        {
            FloatRect FloatRect;
            Intersect(ref value1, ref value2, out FloatRect);
            return FloatRect;
        }

        /// <summary>
        /// Creates a new <see cref="FloatRect"/> that contains overlapping region of two other FloatRects.
        /// </summary>
        /// <param name="value1">The first <see cref="FloatRect"/>.</param>
        /// <param name="value2">The second <see cref="FloatRect"/>.</param>
        /// <param name="result">Overlapping region of the two FloatRects as an output parameter.</param>
        public static void Intersect(ref FloatRect value1, ref FloatRect value2, out FloatRect result)
        {
            if (value1.Intersects(value2))
            {
                float right_side = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
                float left_side = Math.Max(value1.X, value2.X);
                float top_side = Math.Max(value1.Y, value2.Y);
                float bottom_side = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
                result = new FloatRect(left_side, top_side, right_side - left_side, bottom_side - top_side);
            }
            else
            {
                result = new FloatRect(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Changes the <see cref="Location"/> of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="offsetX">The x coordinate to add to this <see cref="FloatRect"/>.</param>
        /// <param name="offsetY">The y coordinate to add to this <see cref="FloatRect"/>.</param>
        public void Offset(float offsetX, float offsetY)
        {
            X += (int)offsetX;
            Y += (int)offsetY;
        }

        /// <summary>
        /// Changes the <see cref="Location"/> of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="FloatRect"/>.</param>
        public void Offset(Point amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        /// <summary>
        /// Changes the <see cref="Location"/> of this <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="FloatRect"/>.</param>
        public void Offset(Vector2 amount)
        {
            X += (int)amount.X;
            Y += (int)amount.Y;
        }

        /// <summary>
        /// Returns a <see cref="String"/> representation of this <see cref="FloatRect"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Width:[<see cref="Width"/>] Height:[<see cref="Height"/>]}
        /// </summary>
        /// <returns><see cref="String"/> representation of this <see cref="FloatRect"/>.</returns>
        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Width:" + Width + " Height:" + Height + "}";
        }

        /// <summary>
        /// Creates a new <see cref="FloatRect"/> that completely contains two other FloatRects.
        /// </summary>
        /// <param name="value1">The first <see cref="FloatRect"/>.</param>
        /// <param name="value2">The second <see cref="FloatRect"/>.</param>
        /// <returns>The union of the two FloatRects.</returns>
        public static FloatRect Union(FloatRect value1, FloatRect value2)
        {
            float x = Math.Min(value1.X, value2.X);
            float y = Math.Min(value1.Y, value2.Y);
            return new FloatRect(x, y,
                                 Math.Max(value1.Right, value2.Right) - x,
                                     Math.Max(value1.Bottom, value2.Bottom) - y);
        }

        /// <summary>
        /// Creates a new <see cref="FloatRect"/> that completely contains two other FloatRects.
        /// </summary>
        /// <param name="value1">The first <see cref="FloatRect"/>.</param>
        /// <param name="value2">The second <see cref="FloatRect"/>.</param>
        /// <param name="result">The union of the two FloatRects as an output parameter.</param>
        public static void Union(ref FloatRect value1, ref FloatRect value2, out FloatRect result)
        {
            result.X = Math.Min(value1.X, value2.X);
            result.Y = Math.Min(value1.Y, value2.Y);
            result.Width = Math.Max(value1.Right, value2.Right) - result.X;
            result.Height = Math.Max(value1.Bottom, value2.Bottom) - result.Y;
        }

        public static implicit operator Rectangle(FloatRect rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        #endregion
    }
}
