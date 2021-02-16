using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;

namespace NetDaemon.Common.Reactive.Services
{
    /// <inheritdoc />
    public class LightEntity : RxEntityBase
    {
        /// <inheritdoc />
        public LightEntity(INetDaemonRxApp daemon, IEnumerable<string> entityIds) : base(daemon, entityIds)
        {
        }

        public void TurnOn(LightEntityTurnOnOptions options)
        {
            base.TurnOn(options);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class LightEntityExtensions
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="pct"></param>
        public static void DimTo(this LightEntity entity,short pct)
        {
            entity.TurnOn(new LightEntityTurnOnOptions()
            {
                Brightness_Pct = pct
            });
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class LightEntityTurnOnOptions : FluentExpandoObject
    {
        /// <summary>
        /// Number that represents the time (in seconds) the light should take to transition to the new state.
        /// </summary>
        public int Transition
        {
            get => (int) this["transition"];
            set => this["transition"] = value;
        }

        /// <summary>
        /// String with the name of one of the built-in profiles (relax, energize, concentrate, reading) or one of the custom profiles defined in light_profiles.csv in the current working directory.
        /// Light profiles define an xy color, brightness and a transition value (if no transition is desired, set to 0 or leave out the column entirely).
        /// If a profile is given, and a brightness is set, then the profile brightness will be overwritten.
        /// </summary>
        public string Profile
        {
            get => (string) this["profile"];
            set => this["profile"] = value;
        }

        /// <summary>
        /// A list containing two floats representing the hue and saturation of the color you want the light to be. Hue is scaled 0-360, and saturation is scaled 0-100.
        /// </summary>
        public HSColor HsColor
        {
            get => new HSColor(this["hs_color"].ToString());
            set
            {
                if (value.Hue < 0 || value.Hue > 360)
                    throw new ArgumentOutOfRangeException(nameof(Tuple<short, short>.Item1),
                        "Hue value must be between 0 and 360");
                if (value.Saturation < 0 || value.Saturation > 100)
                    throw new ArgumentOutOfRangeException(nameof(Tuple<short, short>.Item2),
                        "Saturation value must be between 0 and 100");
                this["hs_color"] = value.ToString();
            }
        }

        /// <summary>
        /// A list containing two floats representing the xy color you want the light to be. Two comma-separated floats that represent the color in XY.
        /// </summary>
        public XYColor XyColor
        {
            get => new XYColor(this["xy_color"].ToString());
            set => this["xy_color"] = value.ToString();
        }

        /// <summary>
        /// A list containing three integers between 0 and 255 representing the RGB color you want the light to be. Three comma-separated integers that represent the color in RGB, within square brackets.
        /// Note that the specified RGB value will not change the light brightness, only the color.
        /// </summary>
        public RGBColor RgbColor
        {
            get => new RGBColor(this["rgb_color"].ToString());
            set
            {
                if (value.Red < 0 || value.Red > 255)
                    throw new ArgumentException(nameof(RGBColor.Red),
                        $"The value for {nameof(RGBColor.Red)} must be between 0 and 255");
                if (value.Green < 0 || value.Green > 255)
                    throw new ArgumentException(nameof(RGBColor.Green),
                        $"The value for {nameof(RGBColor.Green)} must be between 0 and 255");
                if (value.Blue < 0 || value.Blue > 255)
                    throw new ArgumentException(nameof(RGBColor.Blue),
                        $"The value for {nameof(RGBColor.Blue)} must be between 0 and 255");

                this["rgb_color"] = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public short White_Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Color_Temp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Kelvin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CSS3Color Color_Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short Brightness { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short Brightness_Pct { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short Brightness_Step { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short Brightness_Step_Pct { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Flash { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Effect { get; set; }
    }

    /// <summary>
    /// A list containing two floats representing the xy color you want the light to be. Two comma-separated floats that represent the color in XY.
    /// </summary>
    public class XYColor
    {
        /// <summary>
        /// Gets or sets a value for X
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets a value for Y
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="XYColor"/>.
        /// </summary>
        /// <param name="value">Two comma-separated floats that represent the color in XY</param>
        public XYColor(string? value)
        {
            if (value == null) return;
            var parts = value.Split(",");
            if (parts.Length != 2) return;
            X = Int32.Parse(parts[0]);
            Y = Int32.Parse(parts[1]);
        }

        /// <summary>
        /// Returns two comma-separated floats that represent the color in XY
        /// </summary>
        /// <returns>X,Y</returns>
        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }

    /// <summary>
    /// Class that holds a Hue and Saturation value.
    /// </summary>
    public class HSColor
    {
        /// <summary>
        /// Gets or sets the Hue value. Must be between 0 and 360.
        /// </summary>
        public short Hue { get; set; }

        /// <summary>
        /// Gets or Sets the Saturation value. Must be between 0 and 100.
        /// </summary>
        public short Saturation { get; set; }

        /// <summary>
        /// Creates a new <see cref="HSColor"/> instance.
        /// </summary>
        /// <param name="color">A string Hue and Saturation separated by comma</param>
        public HSColor(string? color)
        {
            if (color == null) return;
            var parts = color.Split(",");
            if (parts.Length != 2) return;
            Hue = short.Parse(parts[0]);
            Saturation = short.Parse(parts[1]);
        }

        /// <summary>
        /// Returns a string value for <see cref="Hue"/> and <see cref="Saturation"/>
        /// </summary>
        /// <returns>A string Hue and Saturation separated by comma</returns>
        public override string ToString()
        {
            return $"{Hue},{Saturation}";
        }
    }

    public class CSS3Color
    {
        public const string AliceBlue = "aliceblue";
        public const string AntiqueWhite = "antiquewhite";
        public const string Aqua = "aqua";
        public const string Aquamarine = "aquamarine";
        public const string Azure = "azure";
        public const string Beige = "beige";
        public const string Bisque = "bisque";
        public const string Black = "black";
        public const string BlanchedAlmond = "blanchedalmond";
        public const string Blue = "blue";
        public const string BlueViolet = "blueviolet";
        public const string Brown = "brown";
        public const string BurlyWood = "burlywood";
        public const string CadetBlue = "cadetblue";
        public const string Chartreuse = "chartreuse";
        public const string Chocolate = "chocolate";
        public const string Coral = "coral";
        public const string CornflowerBlue = "cornflowerblue";
        public const string Cornsilk = "cornsilk";
        public const string Crimson = "crimson";
        public const string Cyan = "cyan";
        public const string DarkBlue = "darkblue";
        public const string DarkCyan = "darkcyan";
        public const string DarkGoldenrod = "darkgoldenrod";
        public const string DarkGray = "darkgray";
        public const string DarkGreen = "darkgreen";
        public const string DarkGrey = "darkgrey";
        public const string DarkKhaki = "darkkhaki";
        public const string DarkMagenta = "darkmagenta";
        public const string DarkOliveGreen = "darkolivegreen";
        public const string DarkOrange = "darkorange";
        public const string DarkOrchid = "darkorchid";
        public const string DarkRed = "darkred";
        public const string DarkSalmon = "darksalmon";
        public const string DarkSeaGreen = "darkseagreen";
        public const string DarkSlateBlue = "darkslateblue";
        public const string DarkSlateGray = "darkslategray";
        public const string DarkSlateGrey = "darkslategrey";
        public const string DarkTurquoise = "darkturquoise";
        public const string DarkViolet = "darkviolet";
        public const string DeepPink = "deeppink";
        public const string DeepSkyBlue = "deepskyblue";
        public const string DimGray = "dimgray";
        public const string DodgerBlue = "dodgerblue";
        public const string FireBrick = "firebrick";
        public const string FloralWhite = "floralwhite";
        public const string ForestGreen = "forestgreen";
        public const string Fuchsia = "fuchsia";
        public const string Gainsboro = "gainsboro";
        public const string GhostWhite = "ghostwhite";
        public const string Gold = "gold";
        public const string Goldenrod = "goldenrod";
        public const string Gray = "gray";
        public const string Green = "green";
        public const string GreenYellow = "greenyellow";
        public const string Grey = "grey";
        public const string Honeydew = "honeydew";
        public const string HotPink = "hotpink";
        public const string IndianRed = "indianred";
        public const string Indigo = "indigo";
        public const string Ivory = "ivory";
        public const string Khaki = "khaki";
        public const string Lavender = "lavender";
        public const string LavenderBlush = "lavenderblush";
        public const string LawnGreen = "lawngreen";
        public const string LemonChiffon = "lemonchiffon";
        public const string LightBlue = "lightblue";
        public const string LightCoral = "lightcoral";
        public const string LightCyan = "lightcyan";
        public const string LightGoldenrodYellow = "lightgoldenrodyellow";
        public const string LightGray = "lightgray";
        public const string LightGreen = "lightgreen";
        public const string LightGrey = "lightgrey";
        public const string LightPink = "lightpink";
        public const string LightSalmon = "lightsalmon";
        public const string LightSeaGreen = "lightseagreen";
        public const string LightSkyBlue = "lightskyblue";
        public const string LightSlateGray = "lightslategray";
        public const string LightSlateGrey = "lightslategrey";
        public const string LightSteelBlue = "lightsteelblue";
        public const string LightYellow = "lightyellow";
        public const string Lime = "lime";
        public const string LimeGreen = "limegreen";
        public const string Linen = "linen";
        public const string Magenta = "magenta";
        public const string Maroon = "maroon";
        public const string MediumAquamarine = "mediumaquamarine";
        public const string MediumBlue = "mediumblue";
        public const string MediumOrchid = "mediumorchid";
        public const string MediumPurple = "mediumpurple";
        public const string MediumSeaGreen = "mediumseagreen";
        public const string MediumSlateBlue = "mediumslateblue";
        public const string MediumSpringGreen = "mediumspringgreen";
        public const string MediumTurquoise = "mediumturquoise";
        public const string MediumVioletRed = "mediumvioletred";
        public const string MidnightBlue = "midnightblue";
        public const string MintCream = "mintcream";
        public const string MistyRose = "mistyrose";
        public const string Moccasin = "moccasin";
        public const string NavajoWhite = "navajowhite";
        public const string Navy = "navy";
        public const string OldLace = "oldlace";
        public const string Olive = "olive";
        public const string OliveDrab = "olivedrab";
        public const string Orange = "orange";
        public const string OrangeRed = "orangered";
        public const string Orchid = "orchid";
        public const string PaleGoldenrod = "palegoldenrod";
        public const string PaleGreen = "palegreen";
        public const string PaleTurquoise = "paleturquoise";
        public const string PaleVioletRed = "palevioletred";
        public const string PapayaWhip = "papayawhip";
        public const string PeachPuff = "peachpuff";
        public const string Peru = "peru";
        public const string Pink = "pink";
        public const string Plum = "plum";
        public const string PowderBlue = "powderblue";
        public const string Purple = "purple";
        public const string Rebeccapurple = "rebeccapurple";
        public const string Red = "red";
        public const string RosyBrown = "rosybrown";
        public const string RoyalBlue = "royalblue";
        public const string SaddleBrown = "saddlebrown";
        public const string Salmon = "salmon";
        public const string SandyBrown = "sandybrown";
        public const string SeaGreen = "seagreen";
        public const string Seashell = "seashell";
        public const string Sienna = "sienna";
        public const string Silver = "silver";
        public const string SkyBlue = "skyblue";
        public const string SlateBlue = "slateblue";
        public const string SlateGray = "slategray";
        public const string SlateGrey = "slategrey";
        public const string Snow = "snow";
        public const string SpringGreen = "springgreen";
        public const string SteelBlue = "steelblue";
        public const string Tan = "tan";
        public const string Teal = "teal";
        public const string Thistle = "thistle";
        public const string Tomato = "tomato";
        public const string Turquoise = "turquoise";
        public const string Violet = "violet";
        public const string Wheat = "wheat";
        public const string White = "white";
        public const string WhiteSmoke = "whitesmoke";
        public const string Yellow = "yellow";
        public const string YellowGreen = "yellowgreen";
    }

    public class RGBColor
    {
        public short Red { get; set; }
        public short Green { get; set; }
        public short Blue { get; set; }

        public RGBColor(string? value)
        {
            if (value == null) return;
            var parts = value.TrimStart('[').TrimEnd(']').Split(",");
            if (parts.Length != 3) return;
            Red = short.Parse(parts[0]);
            Green = short.Parse(parts[1]);
            Blue = short.Parse(parts[2]);
        }

        public override string ToString()
        {
            return $"[{Red},{Green},{Blue}]";
        }
    }
}