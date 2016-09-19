// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using SEdge.Core.Maths;

namespace SEdge.Core
{
    /// <summary>
    /// Describes a 32-bit packed color.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct CustomColor : IEquatable<CustomColor>
    {
        static CustomColor()
        {
            TransparentBlack = new CustomColor(0);
            Transparent = new CustomColor(0);
            AliceBlue = new CustomColor(0xfffff8f0);
            AntiqueWhite = new CustomColor(0xffd7ebfa);
            Aqua = new CustomColor(0xffffff00);
            Aquamarine = new CustomColor(0xffd4ff7f);
            Azure = new CustomColor(0xfffffff0);
            Beige = new CustomColor(0xffdcf5f5);
            Bisque = new CustomColor(0xffc4e4ff);
            Black = new CustomColor(0xff000000);
            BlanchedAlmond = new CustomColor(0xffcdebff);
            Blue = new CustomColor(0xffff0000);
            BlueViolet = new CustomColor(0xffe22b8a);
            Brown = new CustomColor(0xff2a2aa5);
            BurlyWood = new CustomColor(0xff87b8de);
            CadetBlue = new CustomColor(0xffa09e5f);
            Chartreuse = new CustomColor(0xff00ff7f);
            Chocolate = new CustomColor(0xff1e69d2);
            Coral = new CustomColor(0xff507fff);
            CornflowerBlue = new CustomColor(0xffed9564);
            Cornsilk = new CustomColor(0xffdcf8ff);
            Crimson = new CustomColor(0xff3c14dc);
            Cyan = new CustomColor(0xffffff00);
            DarkBlue = new CustomColor(0xff8b0000);
            DarkCyan = new CustomColor(0xff8b8b00);
            DarkGoldenrod = new CustomColor(0xff0b86b8);
            DarkGray = new CustomColor(0xffa9a9a9);
            DarkGreen = new CustomColor(0xff006400);
            DarkKhaki = new CustomColor(0xff6bb7bd);
            DarkMagenta = new CustomColor(0xff8b008b);
            DarkOliveGreen = new CustomColor(0xff2f6b55);
            DarkOrange = new CustomColor(0xff008cff);
            DarkOrchid = new CustomColor(0xffcc3299);
            DarkRed = new CustomColor(0xff00008b);
            DarkSalmon = new CustomColor(0xff7a96e9);
            DarkSeaGreen = new CustomColor(0xff8bbc8f);
            DarkSlateBlue = new CustomColor(0xff8b3d48);
            DarkSlateGray = new CustomColor(0xff4f4f2f);
            DarkTurquoise = new CustomColor(0xffd1ce00);
            DarkViolet = new CustomColor(0xffd30094);
            DeepPink = new CustomColor(0xff9314ff);
            DeepSkyBlue = new CustomColor(0xffffbf00);
            DimGray = new CustomColor(0xff696969);
            DodgerBlue = new CustomColor(0xffff901e);
            Firebrick = new CustomColor(0xff2222b2);
            FloralWhite = new CustomColor(0xfff0faff);
            ForestGreen = new CustomColor(0xff228b22);
            Fuchsia = new CustomColor(0xffff00ff);
            Gainsboro = new CustomColor(0xffdcdcdc);
            GhostWhite = new CustomColor(0xfffff8f8);
            Gold = new CustomColor(0xff00d7ff);
            Goldenrod = new CustomColor(0xff20a5da);
            Gray = new CustomColor(0xff808080);
            Green = new CustomColor(0xff008000);
            GreenYellow = new CustomColor(0xff2fffad);
            Honeydew = new CustomColor(0xfff0fff0);
            HotPink = new CustomColor(0xffb469ff);
            IndianRed = new CustomColor(0xff5c5ccd);
            Indigo = new CustomColor(0xff82004b);
            Ivory = new CustomColor(0xfff0ffff);
            Khaki = new CustomColor(0xff8ce6f0);
            Lavender = new CustomColor(0xfffae6e6);
            LavenderBlush = new CustomColor(0xfff5f0ff);
            LawnGreen = new CustomColor(0xff00fc7c);
            LemonChiffon = new CustomColor(0xffcdfaff);
            LightBlue = new CustomColor(0xffe6d8ad);
            LightCoral = new CustomColor(0xff8080f0);
            LightCyan = new CustomColor(0xffffffe0);
            LightGoldenrodYellow = new CustomColor(0xffd2fafa);
            LightGray = new CustomColor(0xffd3d3d3);
            LightGreen = new CustomColor(0xff90ee90);
            LightPink = new CustomColor(0xffc1b6ff);
            LightSalmon = new CustomColor(0xff7aa0ff);
            LightSeaGreen = new CustomColor(0xffaab220);
            LightSkyBlue = new CustomColor(0xffface87);
            LightSlateGray = new CustomColor(0xff998877);
            LightSteelBlue = new CustomColor(0xffdec4b0);
            LightYellow = new CustomColor(0xffe0ffff);
            Lime = new CustomColor(0xff00ff00);
            LimeGreen = new CustomColor(0xff32cd32);
            Linen = new CustomColor(0xffe6f0fa);
            Magenta = new CustomColor(0xffff00ff);
            Maroon = new CustomColor(0xff000080);
            MediumAquamarine = new CustomColor(0xffaacd66);
            MediumBlue = new CustomColor(0xffcd0000);
            MediumOrchid = new CustomColor(0xffd355ba);
            MediumPurple = new CustomColor(0xffdb7093);
            MediumSeaGreen = new CustomColor(0xff71b33c);
            MediumSlateBlue = new CustomColor(0xffee687b);
            MediumSpringGreen = new CustomColor(0xff9afa00);
            MediumTurquoise = new CustomColor(0xffccd148);
            MediumVioletRed = new CustomColor(0xff8515c7);
            MidnightBlue = new CustomColor(0xff701919);
            MintCream = new CustomColor(0xfffafff5);
            MistyRose = new CustomColor(0xffe1e4ff);
            Moccasin = new CustomColor(0xffb5e4ff);
            MonoGameOrange = new CustomColor(0xff003ce7);
            NavajoWhite = new CustomColor(0xffaddeff);
            Navy = new CustomColor(0xff800000);
            OldLace = new CustomColor(0xffe6f5fd);
            Olive = new CustomColor(0xff008080);
            OliveDrab = new CustomColor(0xff238e6b);
            Orange = new CustomColor(0xff00a5ff);
            OrangeRed = new CustomColor(0xff0045ff);
            Orchid = new CustomColor(0xffd670da);
            PaleGoldenrod = new CustomColor(0xffaae8ee);
            PaleGreen = new CustomColor(0xff98fb98);
            PaleTurquoise = new CustomColor(0xffeeeeaf);
            PaleVioletRed = new CustomColor(0xff9370db);
            PapayaWhip = new CustomColor(0xffd5efff);
            PeachPuff = new CustomColor(0xffb9daff);
            Peru = new CustomColor(0xff3f85cd);
            Pink = new CustomColor(0xffcbc0ff);
            Plum = new CustomColor(0xffdda0dd);
            PowderBlue = new CustomColor(0xffe6e0b0);
            Purple = new CustomColor(0xff800080);
            Red = new CustomColor(0xff0000ff);
            RosyBrown = new CustomColor(0xff8f8fbc);
            RoyalBlue = new CustomColor(0xffe16941);
            SaddleBrown = new CustomColor(0xff13458b);
            Salmon = new CustomColor(0xff7280fa);
            SandyBrown = new CustomColor(0xff60a4f4);
            SeaGreen = new CustomColor(0xff578b2e);
            SeaShell = new CustomColor(0xffeef5ff);
            Sienna = new CustomColor(0xff2d52a0);
            Silver = new CustomColor(0xffc0c0c0);
            SkyBlue = new CustomColor(0xffebce87);
            SlateBlue = new CustomColor(0xffcd5a6a);
            SlateGray = new CustomColor(0xff908070);
            Snow = new CustomColor(0xfffafaff);
            SpringGreen = new CustomColor(0xff7fff00);
            SteelBlue = new CustomColor(0xffb48246);
            Tan = new CustomColor(0xff8cb4d2);
            Teal = new CustomColor(0xff808000);
            Thistle = new CustomColor(0xffd8bfd8);
            Tomato = new CustomColor(0xff4763ff);
            Turquoise = new CustomColor(0xffd0e040);
            Violet = new CustomColor(0xffee82ee);
            Wheat = new CustomColor(0xffb3def5);
            White = new CustomColor(uint.MaxValue);
            WhiteSmoke = new CustomColor(0xfff5f5f5);
            Yellow = new CustomColor(0xff00ffff);
            YellowGreen = new CustomColor(0xff32cd9a);
        }

        // ARGB
        private uint _packedValue;

        public CustomColor(uint packedValue)
        {
            this._packedValue = packedValue;
            // ARGB
            //_packedValue = (packedValue << 8) | ((packedValue & 0xff000000) >> 24);
            // ABGR			
            //_packedValue = (packedValue & 0xff00ff00) | ((packedValue & 0x000000ff) << 16) | ((packedValue & 0x00ff0000) >> 16);
        }

        /// <summary>
        /// Constructs an RGBA color from a <see cref="CustomColor"/> and an alpha value.
        /// </summary>
        /// <param name="color">A <see cref="CustomColor"/> for RGB values of new <see cref="CustomColor"/> instance.</param>
        /// <param name="alpha">The alpha component value from 0 to 255.</param>
        public CustomColor(CustomColor color, int alpha)
        {
            this._packedValue = 0;

            this.R = color.R;
            this.G = color.G;
            this.B = color.B;
            this.A = (byte) MathHelper.Clamp(alpha, Byte.MinValue, Byte.MaxValue);
        }

        /// <summary>
        /// Constructs an RGBA color from color and alpha value.
        /// </summary>
        /// <param name="color">A <see cref="CustomColor"/> for RGB values of new <see cref="CustomColor"/> instance.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public CustomColor(CustomColor color, float alpha)
        {
            this._packedValue = 0;

            this.R = color.R;
            this.G = color.G;
            this.B = color.B;
            this.A = (byte) MathHelper.Clamp(alpha * 255, Byte.MinValue, Byte.MaxValue);
        }

        /// <summary>
        /// Constructs an RGBA color from scalars which representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        public CustomColor(float r, float g, float b)
        {
            this._packedValue = 0;

            this.R = (byte) MathHelper.Clamp(r * 255, Byte.MinValue, Byte.MaxValue);
            this.G = (byte) MathHelper.Clamp(g * 255, Byte.MinValue, Byte.MaxValue);
            this.B = (byte) MathHelper.Clamp(b * 255, Byte.MinValue, Byte.MaxValue);
            this.A = 255;
        }

        /// <summary>
        /// Constructs an RGBA color from scalars which representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public CustomColor(int r, int g, int b)
        {
            this._packedValue = 0;
            this.R = (byte) MathHelper.Clamp(r, Byte.MinValue, Byte.MaxValue);
            this.G = (byte) MathHelper.Clamp(g, Byte.MinValue, Byte.MaxValue);
            this.B = (byte) MathHelper.Clamp(b, Byte.MinValue, Byte.MaxValue);
            this.A = (byte) 255;
        }

        /// <summary>
        /// Constructs an RGBA color from scalars which representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        /// <param name="alpha">Alpha component value from 0 to 255.</param>
        public CustomColor(int r, int g, int b, int alpha)
        {
            this._packedValue = 0;
            this.R = (byte) MathHelper.Clamp(r, Byte.MinValue, Byte.MaxValue);
            this.G = (byte) MathHelper.Clamp(g, Byte.MinValue, Byte.MaxValue);
            this.B = (byte) MathHelper.Clamp(b, Byte.MinValue, Byte.MaxValue);
            this.A = (byte) MathHelper.Clamp(alpha, Byte.MinValue, Byte.MaxValue);
        }

        /// <summary>
        /// Constructs an RGBA color from scalars which representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public CustomColor(float r, float g, float b, float alpha)
        {
            this._packedValue = 0;

            this.R = (byte) MathHelper.Clamp(r * 255, Byte.MinValue, Byte.MaxValue);
            this.G = (byte) MathHelper.Clamp(g * 255, Byte.MinValue, Byte.MaxValue);
            this.B = (byte) MathHelper.Clamp(b * 255, Byte.MinValue, Byte.MaxValue);
            this.A = (byte) MathHelper.Clamp(alpha * 255, Byte.MinValue, Byte.MaxValue);
        }

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        [DataMember]
        public byte B
        {
            get
            {
                unchecked
                {
                    return (byte) (this._packedValue >> 16);
                }
            }
            set { this._packedValue = (this._packedValue & 0xff00ffff) | ((uint) value << 16); }
        }

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        [DataMember]
        public byte G
        {
            get
            {
                unchecked
                {
                    return (byte) (this._packedValue >> 8);
                }
            }
            set { this._packedValue = (this._packedValue & 0xffff00ff) | ((uint) value << 8); }
        }

        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        [DataMember]
        public byte R
        {
            get
            {
                unchecked
                {
                    return (byte) this._packedValue;
                }
            }
            set { this._packedValue = (this._packedValue & 0xffffff00) | value; }
        }

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        [DataMember]
        public byte A
        {
            get
            {
                unchecked
                {
                    return (byte) (this._packedValue >> 24);
                }
            }
            set { this._packedValue = (this._packedValue & 0x00ffffff) | ((uint) value << 24); }
        }

        /// <summary>
        /// Compares whether two <see cref="CustomColor"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="CustomColor"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="CustomColor"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(CustomColor a, CustomColor b)
        {
            return (a.A == b.A &&
                    a.R == b.R &&
                    a.G == b.G &&
                    a.B == b.B);
        }

        /// <summary>
        /// Compares whether two <see cref="CustomColor"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="CustomColor"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="CustomColor"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
        public static bool operator !=(CustomColor a, CustomColor b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Gets the hash code of this <see cref="CustomColor"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="CustomColor"/>.</returns>
        public override int GetHashCode()
        {
            return this._packedValue.GetHashCode();
        }

        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        /// <param name="obj">The <see cref="CustomColor"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return ((obj is CustomColor) && Equals((CustomColor) obj));
        }

        #region CustomColor Bank

        /// <summary>
        /// TransparentBlack color (R:0,G:0,B:0,A:0).
        /// </summary>
        public static CustomColor TransparentBlack { get; private set; }

        /// <summary>
        /// Transparent color (R:0,G:0,B:0,A:0).
        /// </summary>
        public static CustomColor Transparent { get; private set; }

        /// <summary>
        /// AliceBlue color (R:240,G:248,B:255,A:255).
        /// </summary>
        public static CustomColor AliceBlue { get; private set; }

        /// <summary>
        /// AntiqueWhite color (R:250,G:235,B:215,A:255).
        /// </summary>
        public static CustomColor AntiqueWhite { get; private set; }

        /// <summary>
        /// Aqua color (R:0,G:255,B:255,A:255).
        /// </summary>
        public static CustomColor Aqua { get; private set; }

        /// <summary>
        /// Aquamarine color (R:127,G:255,B:212,A:255).
        /// </summary>
        public static CustomColor Aquamarine { get; private set; }

        /// <summary>
        /// Azure color (R:240,G:255,B:255,A:255).
        /// </summary>
        public static CustomColor Azure { get; private set; }

        /// <summary>
        /// Beige color (R:245,G:245,B:220,A:255).
        /// </summary>
        public static CustomColor Beige { get; private set; }

        /// <summary>
        /// Bisque color (R:255,G:228,B:196,A:255).
        /// </summary>
        public static CustomColor Bisque { get; private set; }

        /// <summary>
        /// Black color (R:0,G:0,B:0,A:255).
        /// </summary>
        public static CustomColor Black { get; private set; }

        /// <summary>
        /// BlanchedAlmond color (R:255,G:235,B:205,A:255).
        /// </summary>
        public static CustomColor BlanchedAlmond { get; private set; }

        /// <summary>
        /// Blue color (R:0,G:0,B:255,A:255).
        /// </summary>
        public static CustomColor Blue { get; private set; }

        /// <summary>
        /// BlueViolet color (R:138,G:43,B:226,A:255).
        /// </summary>
        public static CustomColor BlueViolet { get; private set; }

        /// <summary>
        /// Brown color (R:165,G:42,B:42,A:255).
        /// </summary>
        public static CustomColor Brown { get; private set; }

        /// <summary>
        /// BurlyWood color (R:222,G:184,B:135,A:255).
        /// </summary>
        public static CustomColor BurlyWood { get; private set; }

        /// <summary>
        /// CadetBlue color (R:95,G:158,B:160,A:255).
        /// </summary>
        public static CustomColor CadetBlue { get; private set; }

        /// <summary>
        /// Chartreuse color (R:127,G:255,B:0,A:255).
        /// </summary>
        public static CustomColor Chartreuse { get; private set; }

        /// <summary>
        /// Chocolate color (R:210,G:105,B:30,A:255).
        /// </summary>
        public static CustomColor Chocolate { get; private set; }

        /// <summary>
        /// Coral color (R:255,G:127,B:80,A:255).
        /// </summary>
        public static CustomColor Coral { get; private set; }

        /// <summary>
        /// CornflowerBlue color (R:100,G:149,B:237,A:255).
        /// </summary>
        public static CustomColor CornflowerBlue { get; private set; }

        /// <summary>
        /// Cornsilk color (R:255,G:248,B:220,A:255).
        /// </summary>
        public static CustomColor Cornsilk { get; private set; }

        /// <summary>
        /// Crimson color (R:220,G:20,B:60,A:255).
        /// </summary>
        public static CustomColor Crimson { get; private set; }

        /// <summary>
        /// Cyan color (R:0,G:255,B:255,A:255).
        /// </summary>
        public static CustomColor Cyan { get; private set; }

        /// <summary>
        /// DarkBlue color (R:0,G:0,B:139,A:255).
        /// </summary>
        public static CustomColor DarkBlue { get; private set; }

        /// <summary>
        /// DarkCyan color (R:0,G:139,B:139,A:255).
        /// </summary>
        public static CustomColor DarkCyan { get; private set; }

        /// <summary>
        /// DarkGoldenrod color (R:184,G:134,B:11,A:255).
        /// </summary>
        public static CustomColor DarkGoldenrod { get; private set; }

        /// <summary>
        /// DarkGray color (R:169,G:169,B:169,A:255).
        /// </summary>
        public static CustomColor DarkGray { get; private set; }

        /// <summary>
        /// DarkGreen color (R:0,G:100,B:0,A:255).
        /// </summary>
        public static CustomColor DarkGreen { get; private set; }

        /// <summary>
        /// DarkKhaki color (R:189,G:183,B:107,A:255).
        /// </summary>
        public static CustomColor DarkKhaki { get; private set; }

        /// <summary>
        /// DarkMagenta color (R:139,G:0,B:139,A:255).
        /// </summary>
        public static CustomColor DarkMagenta { get; private set; }

        /// <summary>
        /// DarkOliveGreen color (R:85,G:107,B:47,A:255).
        /// </summary>
        public static CustomColor DarkOliveGreen { get; private set; }

        /// <summary>
        /// DarkOrange color (R:255,G:140,B:0,A:255).
        /// </summary>
        public static CustomColor DarkOrange { get; private set; }

        /// <summary>
        /// DarkOrchid color (R:153,G:50,B:204,A:255).
        /// </summary>
        public static CustomColor DarkOrchid { get; private set; }

        /// <summary>
        /// DarkRed color (R:139,G:0,B:0,A:255).
        /// </summary>
        public static CustomColor DarkRed { get; private set; }

        /// <summary>
        /// DarkSalmon color (R:233,G:150,B:122,A:255).
        /// </summary>
        public static CustomColor DarkSalmon { get; private set; }

        /// <summary>
        /// DarkSeaGreen color (R:143,G:188,B:139,A:255).
        /// </summary>
        public static CustomColor DarkSeaGreen { get; private set; }

        /// <summary>
        /// DarkSlateBlue color (R:72,G:61,B:139,A:255).
        /// </summary>
        public static CustomColor DarkSlateBlue { get; private set; }

        /// <summary>
        /// DarkSlateGray color (R:47,G:79,B:79,A:255).
        /// </summary>
        public static CustomColor DarkSlateGray { get; private set; }

        /// <summary>
        /// DarkTurquoise color (R:0,G:206,B:209,A:255).
        /// </summary>
        public static CustomColor DarkTurquoise { get; private set; }

        /// <summary>
        /// DarkViolet color (R:148,G:0,B:211,A:255).
        /// </summary>
        public static CustomColor DarkViolet { get; private set; }

        /// <summary>
        /// DeepPink color (R:255,G:20,B:147,A:255).
        /// </summary>
        public static CustomColor DeepPink { get; private set; }

        /// <summary>
        /// DeepSkyBlue color (R:0,G:191,B:255,A:255).
        /// </summary>
        public static CustomColor DeepSkyBlue { get; private set; }

        /// <summary>
        /// DimGray color (R:105,G:105,B:105,A:255).
        /// </summary>
        public static CustomColor DimGray { get; private set; }

        /// <summary>
        /// DodgerBlue color (R:30,G:144,B:255,A:255).
        /// </summary>
        public static CustomColor DodgerBlue { get; private set; }

        /// <summary>
        /// Firebrick color (R:178,G:34,B:34,A:255).
        /// </summary>
        public static CustomColor Firebrick { get; private set; }

        /// <summary>
        /// FloralWhite color (R:255,G:250,B:240,A:255).
        /// </summary>
        public static CustomColor FloralWhite { get; private set; }

        /// <summary>
        /// ForestGreen color (R:34,G:139,B:34,A:255).
        /// </summary>
        public static CustomColor ForestGreen { get; private set; }

        /// <summary>
        /// Fuchsia color (R:255,G:0,B:255,A:255).
        /// </summary>
        public static CustomColor Fuchsia { get; private set; }

        /// <summary>
        /// Gainsboro color (R:220,G:220,B:220,A:255).
        /// </summary>
        public static CustomColor Gainsboro { get; private set; }

        /// <summary>
        /// GhostWhite color (R:248,G:248,B:255,A:255).
        /// </summary>
        public static CustomColor GhostWhite { get; private set; }

        /// <summary>
        /// Gold color (R:255,G:215,B:0,A:255).
        /// </summary>
        public static CustomColor Gold { get; private set; }

        /// <summary>
        /// Goldenrod color (R:218,G:165,B:32,A:255).
        /// </summary>
        public static CustomColor Goldenrod { get; private set; }

        /// <summary>
        /// Gray color (R:128,G:128,B:128,A:255).
        /// </summary>
        public static CustomColor Gray { get; private set; }

        /// <summary>
        /// Green color (R:0,G:128,B:0,A:255).
        /// </summary>
        public static CustomColor Green { get; private set; }

        /// <summary>
        /// GreenYellow color (R:173,G:255,B:47,A:255).
        /// </summary>
        public static CustomColor GreenYellow { get; private set; }

        /// <summary>
        /// Honeydew color (R:240,G:255,B:240,A:255).
        /// </summary>
        public static CustomColor Honeydew { get; private set; }

        /// <summary>
        /// HotPink color (R:255,G:105,B:180,A:255).
        /// </summary>
        public static CustomColor HotPink { get; private set; }

        /// <summary>
        /// IndianRed color (R:205,G:92,B:92,A:255).
        /// </summary>
        public static CustomColor IndianRed { get; private set; }

        /// <summary>
        /// Indigo color (R:75,G:0,B:130,A:255).
        /// </summary>
        public static CustomColor Indigo { get; private set; }

        /// <summary>
        /// Ivory color (R:255,G:255,B:240,A:255).
        /// </summary>
        public static CustomColor Ivory { get; private set; }

        /// <summary>
        /// Khaki color (R:240,G:230,B:140,A:255).
        /// </summary>
        public static CustomColor Khaki { get; private set; }

        /// <summary>
        /// Lavender color (R:230,G:230,B:250,A:255).
        /// </summary>
        public static CustomColor Lavender { get; private set; }

        /// <summary>
        /// LavenderBlush color (R:255,G:240,B:245,A:255).
        /// </summary>
        public static CustomColor LavenderBlush { get; private set; }

        /// <summary>
        /// LawnGreen color (R:124,G:252,B:0,A:255).
        /// </summary>
        public static CustomColor LawnGreen { get; private set; }

        /// <summary>
        /// LemonChiffon color (R:255,G:250,B:205,A:255).
        /// </summary>
        public static CustomColor LemonChiffon { get; private set; }

        /// <summary>
        /// LightBlue color (R:173,G:216,B:230,A:255).
        /// </summary>
        public static CustomColor LightBlue { get; private set; }

        /// <summary>
        /// LightCoral color (R:240,G:128,B:128,A:255).
        /// </summary>
        public static CustomColor LightCoral { get; private set; }

        /// <summary>
        /// LightCyan color (R:224,G:255,B:255,A:255).
        /// </summary>
        public static CustomColor LightCyan { get; private set; }

        /// <summary>
        /// LightGoldenrodYellow color (R:250,G:250,B:210,A:255).
        /// </summary>
        public static CustomColor LightGoldenrodYellow { get; private set; }

        /// <summary>
        /// LightGray color (R:211,G:211,B:211,A:255).
        /// </summary>
        public static CustomColor LightGray { get; private set; }

        /// <summary>
        /// LightGreen color (R:144,G:238,B:144,A:255).
        /// </summary>
        public static CustomColor LightGreen { get; private set; }

        /// <summary>
        /// LightPink color (R:255,G:182,B:193,A:255).
        /// </summary>
        public static CustomColor LightPink { get; private set; }

        /// <summary>
        /// LightSalmon color (R:255,G:160,B:122,A:255).
        /// </summary>
        public static CustomColor LightSalmon { get; private set; }

        /// <summary>
        /// LightSeaGreen color (R:32,G:178,B:170,A:255).
        /// </summary>
        public static CustomColor LightSeaGreen { get; private set; }

        /// <summary>
        /// LightSkyBlue color (R:135,G:206,B:250,A:255).
        /// </summary>
        public static CustomColor LightSkyBlue { get; private set; }

        /// <summary>
        /// LightSlateGray color (R:119,G:136,B:153,A:255).
        /// </summary>
        public static CustomColor LightSlateGray { get; private set; }

        /// <summary>
        /// LightSteelBlue color (R:176,G:196,B:222,A:255).
        /// </summary>
        public static CustomColor LightSteelBlue { get; private set; }

        /// <summary>
        /// LightYellow color (R:255,G:255,B:224,A:255).
        /// </summary>
        public static CustomColor LightYellow { get; private set; }

        /// <summary>
        /// Lime color (R:0,G:255,B:0,A:255).
        /// </summary>
        public static CustomColor Lime { get; private set; }

        /// <summary>
        /// LimeGreen color (R:50,G:205,B:50,A:255).
        /// </summary>
        public static CustomColor LimeGreen { get; private set; }

        /// <summary>
        /// Linen color (R:250,G:240,B:230,A:255).
        /// </summary>
        public static CustomColor Linen { get; private set; }

        /// <summary>
        /// Magenta color (R:255,G:0,B:255,A:255).
        /// </summary>
        public static CustomColor Magenta { get; private set; }

        /// <summary>
        /// Maroon color (R:128,G:0,B:0,A:255).
        /// </summary>
        public static CustomColor Maroon { get; private set; }

        /// <summary>
        /// MediumAquamarine color (R:102,G:205,B:170,A:255).
        /// </summary>
        public static CustomColor MediumAquamarine { get; private set; }

        /// <summary>
        /// MediumBlue color (R:0,G:0,B:205,A:255).
        /// </summary>
        public static CustomColor MediumBlue { get; private set; }

        /// <summary>
        /// MediumOrchid color (R:186,G:85,B:211,A:255).
        /// </summary>
        public static CustomColor MediumOrchid { get; private set; }

        /// <summary>
        /// MediumPurple color (R:147,G:112,B:219,A:255).
        /// </summary>
        public static CustomColor MediumPurple { get; private set; }

        /// <summary>
        /// MediumSeaGreen color (R:60,G:179,B:113,A:255).
        /// </summary>
        public static CustomColor MediumSeaGreen { get; private set; }

        /// <summary>
        /// MediumSlateBlue color (R:123,G:104,B:238,A:255).
        /// </summary>
        public static CustomColor MediumSlateBlue { get; private set; }

        /// <summary>
        /// MediumSpringGreen color (R:0,G:250,B:154,A:255).
        /// </summary>
        public static CustomColor MediumSpringGreen { get; private set; }

        /// <summary>
        /// MediumTurquoise color (R:72,G:209,B:204,A:255).
        /// </summary>
        public static CustomColor MediumTurquoise { get; private set; }

        /// <summary>
        /// MediumVioletRed color (R:199,G:21,B:133,A:255).
        /// </summary>
        public static CustomColor MediumVioletRed { get; private set; }

        /// <summary>
        /// MidnightBlue color (R:25,G:25,B:112,A:255).
        /// </summary>
        public static CustomColor MidnightBlue { get; private set; }

        /// <summary>
        /// MintCream color (R:245,G:255,B:250,A:255).
        /// </summary>
        public static CustomColor MintCream { get; private set; }

        /// <summary>
        /// MistyRose color (R:255,G:228,B:225,A:255).
        /// </summary>
        public static CustomColor MistyRose { get; private set; }

        /// <summary>
        /// Moccasin color (R:255,G:228,B:181,A:255).
        /// </summary>
        public static CustomColor Moccasin { get; private set; }

        /// <summary>
        /// MonoGame orange theme color (R:231,G:60,B:0,A:255).
        /// </summary>
        public static CustomColor MonoGameOrange { get; private set; }

        /// <summary>
        /// NavajoWhite color (R:255,G:222,B:173,A:255).
        /// </summary>
        public static CustomColor NavajoWhite { get; private set; }

        /// <summary>
        /// Navy color (R:0,G:0,B:128,A:255).
        /// </summary>
        public static CustomColor Navy { get; private set; }

        /// <summary>
        /// OldLace color (R:253,G:245,B:230,A:255).
        /// </summary>
        public static CustomColor OldLace { get; private set; }

        /// <summary>
        /// Olive color (R:128,G:128,B:0,A:255).
        /// </summary>
        public static CustomColor Olive { get; private set; }

        /// <summary>
        /// OliveDrab color (R:107,G:142,B:35,A:255).
        /// </summary>
        public static CustomColor OliveDrab { get; private set; }

        /// <summary>
        /// Orange color (R:255,G:165,B:0,A:255).
        /// </summary>
        public static CustomColor Orange { get; private set; }

        /// <summary>
        /// OrangeRed color (R:255,G:69,B:0,A:255).
        /// </summary>
        public static CustomColor OrangeRed { get; private set; }

        /// <summary>
        /// Orchid color (R:218,G:112,B:214,A:255).
        /// </summary>
        public static CustomColor Orchid { get; private set; }

        /// <summary>
        /// PaleGoldenrod color (R:238,G:232,B:170,A:255).
        /// </summary>
        public static CustomColor PaleGoldenrod { get; private set; }

        /// <summary>
        /// PaleGreen color (R:152,G:251,B:152,A:255).
        /// </summary>
        public static CustomColor PaleGreen { get; private set; }

        /// <summary>
        /// PaleTurquoise color (R:175,G:238,B:238,A:255).
        /// </summary>
        public static CustomColor PaleTurquoise { get; private set; }

        /// <summary>
        /// PaleVioletRed color (R:219,G:112,B:147,A:255).
        /// </summary>
        public static CustomColor PaleVioletRed { get; private set; }

        /// <summary>
        /// PapayaWhip color (R:255,G:239,B:213,A:255).
        /// </summary>
        public static CustomColor PapayaWhip { get; private set; }

        /// <summary>
        /// PeachPuff color (R:255,G:218,B:185,A:255).
        /// </summary>
        public static CustomColor PeachPuff { get; private set; }

        /// <summary>
        /// Peru color (R:205,G:133,B:63,A:255).
        /// </summary>
        public static CustomColor Peru { get; private set; }

        /// <summary>
        /// Pink color (R:255,G:192,B:203,A:255).
        /// </summary>
        public static CustomColor Pink { get; private set; }

        /// <summary>
        /// Plum color (R:221,G:160,B:221,A:255).
        /// </summary>
        public static CustomColor Plum { get; private set; }

        /// <summary>
        /// PowderBlue color (R:176,G:224,B:230,A:255).
        /// </summary>
        public static CustomColor PowderBlue { get; private set; }

        /// <summary>
        ///  Purple color (R:128,G:0,B:128,A:255).
        /// </summary>
        public static CustomColor Purple { get; private set; }

        /// <summary>
        /// Red color (R:255,G:0,B:0,A:255).
        /// </summary>
        public static CustomColor Red { get; private set; }

        /// <summary>
        /// RosyBrown color (R:188,G:143,B:143,A:255).
        /// </summary>
        public static CustomColor RosyBrown { get; private set; }

        /// <summary>
        /// RoyalBlue color (R:65,G:105,B:225,A:255).
        /// </summary>
        public static CustomColor RoyalBlue { get; private set; }

        /// <summary>
        /// SaddleBrown color (R:139,G:69,B:19,A:255).
        /// </summary>
        public static CustomColor SaddleBrown { get; private set; }

        /// <summary>
        /// Salmon color (R:250,G:128,B:114,A:255).
        /// </summary>
        public static CustomColor Salmon { get; private set; }

        /// <summary>
        /// SandyBrown color (R:244,G:164,B:96,A:255).
        /// </summary>
        public static CustomColor SandyBrown { get; private set; }

        /// <summary>
        /// SeaGreen color (R:46,G:139,B:87,A:255).
        /// </summary>
        public static CustomColor SeaGreen { get; private set; }

        /// <summary>
        /// SeaShell color (R:255,G:245,B:238,A:255).
        /// </summary>
        public static CustomColor SeaShell { get; private set; }

        /// <summary>
        /// Sienna color (R:160,G:82,B:45,A:255).
        /// </summary>
        public static CustomColor Sienna { get; private set; }

        /// <summary>
        /// Silver color (R:192,G:192,B:192,A:255).
        /// </summary>
        public static CustomColor Silver { get; private set; }

        /// <summary>
        /// SkyBlue color (R:135,G:206,B:235,A:255).
        /// </summary>
        public static CustomColor SkyBlue { get; private set; }

        /// <summary>
        /// SlateBlue color (R:106,G:90,B:205,A:255).
        /// </summary>
        public static CustomColor SlateBlue { get; private set; }

        /// <summary>
        /// SlateGray color (R:112,G:128,B:144,A:255).
        /// </summary>
        public static CustomColor SlateGray { get; private set; }

        /// <summary>
        /// Snow color (R:255,G:250,B:250,A:255).
        /// </summary>
        public static CustomColor Snow { get; private set; }

        /// <summary>
        /// SpringGreen color (R:0,G:255,B:127,A:255).
        /// </summary>
        public static CustomColor SpringGreen { get; private set; }

        /// <summary>
        /// SteelBlue color (R:70,G:130,B:180,A:255).
        /// </summary>
        public static CustomColor SteelBlue { get; private set; }

        /// <summary>
        /// Tan color (R:210,G:180,B:140,A:255).
        /// </summary>
        public static CustomColor Tan { get; private set; }

        /// <summary>
        /// Teal color (R:0,G:128,B:128,A:255).
        /// </summary>
        public static CustomColor Teal { get; private set; }

        /// <summary>
        /// Thistle color (R:216,G:191,B:216,A:255).
        /// </summary>
        public static CustomColor Thistle { get; private set; }

        /// <summary>
        /// Tomato color (R:255,G:99,B:71,A:255).
        /// </summary>
        public static CustomColor Tomato { get; private set; }

        /// <summary>
        /// Turquoise color (R:64,G:224,B:208,A:255).
        /// </summary>
        public static CustomColor Turquoise { get; private set; }

        /// <summary>
        /// Violet color (R:238,G:130,B:238,A:255).
        /// </summary>
        public static CustomColor Violet { get; private set; }

        /// <summary>
        /// Wheat color (R:245,G:222,B:179,A:255).
        /// </summary>
        public static CustomColor Wheat { get; private set; }

        /// <summary>
        /// White color (R:255,G:255,B:255,A:255).
        /// </summary>
        public static CustomColor White { get; private set; }

        /// <summary>
        /// WhiteSmoke color (R:245,G:245,B:245,A:255).
        /// </summary>
        public static CustomColor WhiteSmoke { get; private set; }

        /// <summary>
        /// Yellow color (R:255,G:255,B:0,A:255).
        /// </summary>
        public static CustomColor Yellow { get; private set; }

        /// <summary>
        /// YellowGreen color (R:154,G:205,B:50,A:255).
        /// </summary>
        public static CustomColor YellowGreen { get; private set; }

        #endregion

        /// <summary>
        /// Performs linear interpolation of <see cref="CustomColor"/>.
        /// </summary>
        /// <param name="value1">Source <see cref="CustomColor"/>.</param>
        /// <param name="value2">Destination <see cref="CustomColor"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="CustomColor"/>.</returns>
        public static CustomColor Lerp(CustomColor value1, CustomColor value2, Single amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);
            return new CustomColor(
                (int) MathHelper.Lerp(value1.R, value2.R, amount),
                (int) MathHelper.Lerp(value1.G, value2.G, amount),
                (int) MathHelper.Lerp(value1.B, value2.B, amount),
                (int) MathHelper.Lerp(value1.A, value2.A, amount));
        }

        /// <summary>
        /// Performs linear interpolation of <see cref="CustomColor"/> using <see cref="MathHelper.LerpPrecise"/> on MathHelper.
        /// Less efficient but more precise compared to <see cref="CustomColor.Lerp"/>.
        /// See remarks section of <see cref="MathHelper.LerpPrecise"/> on MathHelper for more info.
        /// </summary>
        /// <param name="value1">Source <see cref="CustomColor"/>.</param>
        /// <param name="value2">Destination <see cref="CustomColor"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="CustomColor"/>.</returns>
        public static CustomColor LerpPrecise(CustomColor value1, CustomColor value2, Single amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);
            return new CustomColor(
                (int) MathHelper.LerpPrecise(value1.R, value2.R, amount),
                (int) MathHelper.LerpPrecise(value1.G, value2.G, amount),
                (int) MathHelper.LerpPrecise(value1.B, value2.B, amount),
                (int) MathHelper.LerpPrecise(value1.A, value2.A, amount));
        }

        /// <summary>
        /// Multiply <see cref="CustomColor"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="CustomColor"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
        public static CustomColor Multiply(CustomColor value, float scale)
        {
            return new CustomColor((int) (value.R * scale), (int) (value.G * scale), (int) (value.B * scale), (int) (value.A * scale));
        }

        /// <summary>
        /// Multiply <see cref="CustomColor"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="CustomColor"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
        public static CustomColor operator *(CustomColor value, float scale)
        {
            return new CustomColor((int) (value.R * scale), (int) (value.G * scale), (int) (value.B * scale), (int) (value.A * scale));
        }

        /// <summary>
        /// Gets or sets packed value of this <see cref="CustomColor"/>.
        /// </summary>
        [CLSCompliant(false)]
        public UInt32 PackedValue
        {
            get { return this._packedValue; }
            set { this._packedValue = value; }
        }


        internal string DebugDisplayString
        {
            get
            {
                return string.Concat(
                    this.R.ToString(), "  ",
                    this.G.ToString(), "  ",
                    this.B.ToString(), "  ",
                    this.A.ToString()
                );
            }
        }


        /// <summary>
        /// Returns a <see cref="String"/> representation of this <see cref="CustomColor"/> in the format:
        /// {R:[red] G:[green] B:[blue] A:[alpha]}
        /// </summary>
        /// <returns><see cref="String"/> representation of this <see cref="CustomColor"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(25);
            sb.Append("{R:");
            sb.Append(this.R);
            sb.Append(" G:");
            sb.Append(this.G);
            sb.Append(" B:");
            sb.Append(this.B);
            sb.Append(" A:");
            sb.Append(this.A);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Translate a non-premultipled alpha <see cref="CustomColor"/> to a <see cref="CustomColor"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="r">Red component value.</param>
        /// <param name="g">Green component value.</param>
        /// <param name="b">Blue component value.</param>
        /// <param name="a">Alpha component value.</param>
        /// <returns>A <see cref="CustomColor"/> which contains premultiplied alpha data.</returns>
        public static CustomColor FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new CustomColor((byte) (r * a / 255), (byte) (g * a / 255), (byte) (b * a / 255), a);
        }

        #region IEquatable<CustomColor> Members

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="CustomColor"/>.
        /// </summary>
        /// <param name="other">The <see cref="CustomColor"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(CustomColor other)
        {
            return this.PackedValue == other.PackedValue;
        }

        #endregion
    }
}