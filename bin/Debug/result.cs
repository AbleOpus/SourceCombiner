using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RubiksCubeSolver
{
    static class ExtensionMethods
    {
        public static Color Desaturate(this Color color)
        {
            var b = (int)(255 * color.GetBrightness());
            return Color.FromArgb(b, b, b);
        }

        public static Color ChangeBrightness(this Color color, int factor)
        {
            int R = (color.R + factor > 255) ? 255 : color.R + factor;
            int G = (color.G + factor > 255) ? 255 : color.G + factor;
            int B = (color.B + factor > 255) ? 255 : color.B + factor;

            R = (color.R + factor < 0) ? 0 : R;
            G = (color.G + factor < 0) ? 0 : G;
            B = (color.B + factor < 0) ? 0 : B;

            return Color.FromArgb(R, G, B);
        }

        public static Rectangle ToRect(this RectangleF rectF)
        {
            return new Rectangle((int)(rectF.X + 0.5), (int)(rectF.Y + 0.5),
            (int)(rectF.Width + 0.5), (int)(rectF.Height + 0.5));
        }

        /// <summary>
        /// Gets the opposite case of a letter. For example, if 'c' is specified,
        /// then 'C' will be returned
        /// </summary>
        public static char GetOppositeCase(this char c)
        {
            if (!char.IsLetter(c)) return c;
            return char.IsLower(c) ? char.ToUpper(c) : char.ToLower(c);
        }

        /// <summary>
        /// Shows a typical error message box with an error icon and the applications
        /// product name as the title
        /// </summary>
        public static void ShowMessage(this Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName,
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static IEnumerable<Point> Offset(this IEnumerable<Point> points, int x, int y)
        {
            var list = new List<Point>();

            foreach (Point point in points)
                list.Add(new Point(point.X + x, point.Y + y));

            return list;
        }

        /// <summary>
        /// Gets a rectangle that just encompasses all of the Points
        /// </summary>
        public static Rectangle GetBounds(this IEnumerable<Point> points)
        {
            int left = int.MaxValue;
            int top = int.MaxValue;
            int right = int.MinValue;
            int bottom = int.MinValue;

            foreach (Point pos in points)
            {
                if (pos.X < left) left = pos.X;
                else if (pos.X > right) right = pos.X;

                if (pos.Y < top) top = pos.Y;
                else if (pos.Y > bottom) bottom = pos.Y;
            }
            // Using indexes so add one
            return new Rectangle(left, top, right - left + 1, bottom - top + 1);
        }

        /// <summary>
        /// Returns the center point of this Rectangle
        /// </summary>
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        /// <summary>
        /// Compares two colors using there RGB value (and not their names)
        /// </summary>
        public static bool RgbEquals(this Color c, Color color)
        {
            return (color.R == c.R && color.G == c.G && color.B == c.B);
        } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RubiksCubeSolver.Views;

namespace RubiksCubeSolver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

using System.Drawing;
using System.Drawing.Drawing2D;

namespace RubiksCubeSolver
{
    public abstract class RoundedRectangleF
    {
        public enum RectangleCorners
        {
            None = 0, TopLeft = 1, TopRight = 2, BottomLeft = 4, BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }

        public static GraphicsPath Create(float x, float y, float width, float height,
            float radius, RectangleCorners corners = RectangleCorners.All)
        {
            float xw = x + width;
            float yh = y + height;
            float xwr = xw - radius;
            float yhr = yh - radius;
            float xr = x + radius;
            float yr = y + radius;
            float r2 = radius * 2;
            float xwr2 = xw - r2;
            float yhr2 = yh - r2;

            GraphicsPath p = new GraphicsPath();
            p.StartFigure();

            //Top Left Corner
            if ((RectangleCorners.TopLeft & corners) == RectangleCorners.TopLeft)
            {
                p.AddArc(x, y, r2, r2, 180, 90);
            }
            else
            {
                p.AddLine(x, yr, x, y);
                p.AddLine(x, y, xr, y);
            }

            //Top Edge
            p.AddLine(xr, y, xwr, y);

            //Top Right Corner
            if ((RectangleCorners.TopRight & corners) == RectangleCorners.TopRight)
            {
                p.AddArc(xwr2, y, r2, r2, 270, 90);
            }
            else
            {
                p.AddLine(xwr, y, xw, y);
                p.AddLine(xw, y, xw, yr);
            }

            //Right Edge
            p.AddLine(xw, yr, xw, yhr);

            //Bottom Right Corner
            if ((RectangleCorners.BottomRight & corners) == RectangleCorners.BottomRight)
            {
                p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
            }
            else
            {
                p.AddLine(xw, yhr, xw, yh);
                p.AddLine(xw, yh, xwr, yh);
            }

            //Bottom Edge
            p.AddLine(xwr, yh, xr, yh);

            //Bottom Left Corner
            if ((RectangleCorners.BottomLeft & corners) == RectangleCorners.BottomLeft)
            {
                p.AddArc(x, yhr2, r2, r2, 90, 90);
            }
            else
            {
                p.AddLine(xr, yh, x, yh);
                p.AddLine(x, yh, x, yhr);
            }

            //Left Edge
            p.AddLine(x, yhr, x, yr);

            p.CloseFigure();
            return p;
        }

        public static GraphicsPath Create(RectangleF rect, float radius, RectangleCorners c)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius, c); }

        public static GraphicsPath Create(RectangleF rect, float radius)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius); }
    }
}

using System;
using System.Drawing;

namespace RubiksCubeSolver
{
    [Serializable]
    class Settings : SettingsBase<Settings>
    {
        /// <summary>
        /// Gets or sets the colors to pick from
        /// </summary>
        public Color[] Palette { get; set; }

        public Color[][,] CubeColors { get; set; }

        public override void Reset()
        {
            // Default to my cube's color configuration
            Palette = new[]
            {
                Color.White,
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.Orange,
                Color.Green
            };
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace RubiksCubeSolver
{
    /// <summary>
    /// Provides base functionality for an user/application settings class.
    /// The implementation provides automated binary serialization to the users AppData 
    /// </summary>
    [Serializable]
    public abstract class SettingsBase<T> where T : SettingsBase<T>, new()
    {
        static SettingsBase()
        {
            Load();
        }

        private static void Load()
        {
            Instance = new T(); // We have to instantiate to get the SavePath
            //  Instance.ExploreSettingsDirectory();

            // If file exist, load from it, otherwise set this instance properties to default
            if (File.Exists(Instance.GetSavePath()))
            {
                try
                {
                    using (FileStream stream = File.OpenRead(Instance.GetSavePath()))
                        Instance = (T)(new BinaryFormatter().Deserialize(stream));
                    Debug.WriteLine("Settings loaded from file");
                }
                catch
                {
                    Instance = new T();
                    Instance.Reset();
                    Debug.WriteLine("Settings could not be loaded, reverting to defaults");
                }
            }
            else Instance.Reset();
        }

        /// <summary>
        /// Loads settings from file into the default instance. Use this
        /// when the contents of the settings file changes in an
        /// untraditional manner
        /// </summary>
        public void Reload()
        {
            Load();
        }

        /// <summary>
        /// Deletes the settings file and loads default values
        /// </summary>
        public void Clear()
        {
            string fileName = GetSavePath();

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                Reset();
            }
        }

        /// <summary>
        /// Provides implementation for restoring settings to their default values
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Gets the save path with the format as such: 
        /// <example>...AppData\Roaming\{AssemblyName}\{TypeName}.dat</example>
        /// </summary>
        public virtual string GetSavePath()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dir = Path.Combine(dir, Application.ProductName);
            string typeName = this.GetType().Name.Replace("_", " ");
            return Path.Combine(dir, typeName + ".dat");
        }

        /// <summary>
        /// Gets the save location of these settings
        /// </summary>
        public string GetSaveDirectory()
        {
            return Path.GetDirectoryName(GetSavePath());
        }

        /// <summary>
        /// Opens the directory in which the settings are saved in windows explorer
        /// </summary>
        public void ExploreSettingsDirectory()
        {
            string dir = Path.GetDirectoryName(GetSavePath());

            if (Directory.Exists(dir))
                Process.Start(dir);
        }

        /// <summary>
        /// Saves the settings (to a place specified by the DataSave property). If the 
        /// directory for the settings does not exist, then it is created
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Raises when serialization has failed</exception>
        /// <exception cref="System.IO.EndOfStreamException">Raises when the settings file is corrupt</exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public void Save()
        {
            string fileName = GetSavePath();
            string directory = Path.GetDirectoryName(fileName);

            // The directory might not exist already
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var fileStreamtream = File.OpenWrite(fileName))
                new BinaryFormatter().Serialize(fileStreamtream, this);
        }

        /// <summary>
        /// Gets the only instance of this class
        /// </summary>
        public static T Instance { get; private set; }
    }
}
using System.Drawing;

namespace RubiksCubeSolver.ColorGrid
{
    /// <summary>
    /// Provides a rendering functionality to render a ColorGrid in different ways.
    /// All rendering logic works on a percentage basis (0-1)
    /// </summary>
    static class ColorGridRendering
    {
        /// <summary>
        /// Gets the grid cell point that contains the specified absolute position
        /// </summary>
        /// <returns>-1, if no cell contains the specified position</returns>
        public static Point GetGridPointFromPosition(ColorGridStyle style, Point pos, Size drawRegion)
        {
            var rects = GetAllCellBounds(style, drawRegion);

            for (int x = 0; x < rects.GetLength(0); x++)
            {
                for (int y = 0; y < rects.GetLength(1); y++)
                {
                    if (rects[x, y].Contains(pos)) return new Point(y, x);
                }
            }

            return new Point(-1, -1);
        }

        /// <summary>
        /// Gets the Bounds of a cell that contains the specified absolute position
        /// </summary>
        /// <returns>Rectangle.Empty, if no cell contains the specified position</returns>
        public static RectangleF GetCellRectFromPosition(ColorGridStyle style, Point pos, Size drawRegion)
        {
            foreach (var rect in GetAllCellBounds(style, drawRegion))
                if (rect.Contains(pos)) return rect;

            return Rectangle.Empty;
        }

        /// <summary>
        /// Gets the bounding rectangle for each of the cells in the grid
        /// </summary>
        public static RectangleF[,] GetAllCellBounds(ColorGridStyle style, Size drawRegion)
        {
            var rects = new RectangleF[style.Colors.GetLength(0), style.Colors.GetLength(1)];
            float cellDim = GetCellDimension(style, drawRegion);
            // They will always be centered
            float xOffset = (drawRegion.Width / 2f) - style.Colors.GetLength(0) * cellDim / 2;
            float yOffset = (drawRegion.Height / 2f) - style.Colors.GetLength(1) * cellDim / 2;

            for (int row = 0; row < style.Colors.GetLength(0); row++)
            {
                for (int clm = 0; clm < style.Colors.GetLength(1); clm++)
                {
                    float y = yOffset + cellDim * clm;
                    float x = xOffset + cellDim * row;
                    float dim = cellDim;
                    rects[row, clm] = new RectangleF(x, y, dim, dim);
                }
            }

            return rects;
        }

        /// <summary>
        /// Combimes rectangles in a way that the master rect just ecompasses the
        /// size AND the locations of the child rects
        /// </summary>
        private static RectangleF UniteRects(RectangleF[,] rects)
        {
            float lowestX = float.MaxValue;
            float lowestY = float.MaxValue;
            float greatestRight = 0;
            float greateastBottom = 0;

            foreach (RectangleF rect in rects)
            {
                if (rect.Right > greatestRight) greatestRight = rect.Right;
                if (rect.Bottom > greateastBottom) greateastBottom = rect.Bottom;
                if (rect.X < lowestX) lowestX = rect.X;
                if (rect.Y < lowestY) lowestY = rect.Y;
            }

            return new RectangleF(lowestX, lowestY, 
                greatestRight - lowestX, greateastBottom - lowestY);
        }

        /// <summary>
        /// Gets the dimension of all cells
        /// </summary>
        private static float GetCellDimension(ColorGridStyle style, Size drawRegion)
        {
            int gridWidth = style.Colors.GetLength(0);
            int gridHeight = style.Colors.GetLength(1);
            double widthRatio = (double)drawRegion.Width / gridWidth;
            double heightRatio = (double)drawRegion.Height / gridHeight;

            if (widthRatio > heightRatio)
            {
                return (float)drawRegion.Height / gridHeight;
            }

            return (float)drawRegion.Width / gridWidth;
        }

        public static RectangleF GetMasterRectangle(ColorGridStyle style, Size drawRegion)
        {
            var bounds = GetAllCellBounds(style, drawRegion);
            return UniteRects(bounds);
        }

        /// <summary>
        /// Draws the grid center middle of the region
        /// </summary>
        public static void Draw(ColorGridStyle style, Graphics graphics, Size drawRegion, bool enabled)
        {
            float cellDim = GetCellDimension(style, drawRegion);
            float xOffset = (drawRegion.Width / 2f) - style.Colors.GetLength(0) * cellDim / 2;
            float yOffset = (drawRegion.Height / 2f) - style.Colors.GetLength(1) * cellDim / 2;
            float spacing = style.CellSpacingScale * cellDim;
            var master = GetMasterRectangle(style, drawRegion);
            var backPath = RoundedRectangleF.Create(master, style.RoundedRadius);
            graphics.FillPath(enabled ? Brushes.Black : Brushes.DimGray, backPath);

            for (int row = 0; row < style.Colors.GetLength(0); row++)
            {
                for (int clm = 0; clm < style.Colors.GetLength(1); clm++)
                {
                    float dim = cellDim - spacing*2;
                    var brush = new SolidBrush(style.Colors[clm, row]);
                    if (!enabled) brush.Color = brush.Color.Desaturate();
                    float x = (xOffset + cellDim * row) + spacing;
                    float y = (yOffset + cellDim * clm) + spacing;
                    var path = RoundedRectangleF.Create(x, y, dim, dim, style.RoundedRadius);
                    graphics.FillPath(brush, path);
                }
            }
        }
    }
}

using System.Drawing;

namespace RubiksCubeSolver.ColorGrid
{
    /// <summary>
    /// Represents parameters as required information for the ColorGridRenderer, which
    /// describe the appearance and layout of a color grid
    /// </summary>
    public class ColorGridStyle
    {
        public Color[,] Colors { get; set; }

        /// <summary>
        /// Gets or sets the relative thickness of the border pen
        /// </summary>
        public float CellSpacingScale { get; set; }

        /// <summary>
        /// Gets or sets the corner radius for the rounded rect used with the ColorGridRenderer
        /// </summary>
        public int RoundedRadius { get; set; }


        public ColorGridStyle(Color[,] colors, float cellSpacing, int roundedRadius)
        {
            Colors = colors;
            CellSpacingScale = cellSpacing;
            RoundedRadius = roundedRadius;
        }
    }
}




using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("RubiksCubeSolver")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("RubiksCubeSolver")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ab3d6eb9-be9c-45ed-985a-8efa8d4a46ee")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RubiksCubeSolver.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RubiksCubeSolver.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Lock {
            get {
                object obj = ResourceManager.GetObject("Lock", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RubiksCubeSolver.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
    }
}

using System.Drawing;

namespace RubiksCubeSolver.Rubiks
{
    class ColorDefect
    {
        /// <summary>
        /// Gets the colors that are defective. This will only contain
        /// colors that have too many occurences
        /// </summary>
        public Color[] Colors { get; private set; }

        /// <summary>
        /// Gets the type of color defect
        /// </summary>
        public ColorDefectType Type { get; private set; }

        public ColorDefect(Color[] colors, ColorDefectType type)
        {
            Type = type;
            Colors = colors ?? new Color[] { };
        }
    }
}

using System;
using System.Drawing;

namespace RubiksCubeSolver.Rubiks
{
    /// <summary>
    /// Represents a color layout for a rubiks cube
    /// </summary>
    class CubeColorScheme
    {
        public readonly static CubeColorScheme DevsScheme = new CubeColorScheme
            (Color.White, Color.Yellow, Color.Red, Color.Orange, Color.Blue, Color.Green);

        #region Properties
        /// <summary>
        /// Gets the top color of the cube
        /// </summary>
        public Color UpColor { get; private set; }

        /// <summary>
        /// Gets the bottom color of the cube
        /// </summary>
        public Color DownColor { get; private set; }

        /// <summary>
        /// Gets the left color of the cube
        /// </summary>
        public Color LeftColor { get; private set; }

        /// <summary>
        /// Gets the right color of the cube
        /// </summary>
        public Color RightColor { get; private set; }

        /// <summary>
        /// Gets the front color of the cube
        /// </summary>
        public Color FrontColor { get; private set; }

        /// <summary>
        /// Gets the back color of the cube
        /// </summary>
        public Color BackColor { get; private set; }
        #endregion

        public CubeColorScheme(Color frontColor, Color backColor, Color rightColor,
            Color leftColor, Color upColor, Color downColor)
        {
            FrontColor = frontColor;
            BackColor = backColor;
            RightColor = rightColor;
            LeftColor = leftColor;
            UpColor = upColor;
            DownColor = downColor;
        }

        /// <summary>
        /// Gets the color of the face specified
        /// </summary>
        public static Color FromFaceColors(Color[,] face)
        {
            return face[1, 1];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCubeSolver.Rubiks
{
    /// <summary>
    /// Represents a single rubiks cube movement
    /// </summary>
    class CubeMove
    {
        /// <summary>
        /// Gets the side corresponding to the move
        /// </summary>
        public CubeSide Side { get; private set; }

        /// <summary>
        /// Gets the move direction
        /// </summary>
        public Rotation Rotation { get; private set;}

        public CubeMove(CubeSide side, Rotation rotation)
        {
            Side = side;
            Rotation = rotation;
        }

        /// <summary>
        /// Creates an instance of CubeMove from a single notation 
        /// </summary>
        public CubeMove(string notation)
        {
            switch (notation.Trim().ToLower())
            {
                case "f":
                    Rotation = Rotation.CW;
                    Side = CubeSide.Front;
                    break;

                case "fi":
                    Rotation = Rotation.CCW;
                    Side = CubeSide.Front;
                    break;

                case "b":
                    Rotation = Rotation.CW;
                    Side = CubeSide.Back;
                    break;

                case "bi":
                    Rotation = Rotation.CCW;
                    Side = CubeSide.Back;
                    break;

                case "r":
                    Rotation = Rotation.CW;
                    Side = CubeSide.Right;
                    break;

                case "ri":
                    Rotation = Rotation.CCW;
                    Side = CubeSide.Right;
                    break;

                case "l":
                    Rotation = Rotation.CW;
                    Side = CubeSide.Left;
                    break;

                case "li":
                    Rotation = Rotation.CCW;
                    Side = CubeSide.Left;
                    break;

                case "u":
                    Rotation = Rotation.CW;
                    Side = CubeSide.Up;
                    break;

                case "ui":
                    Rotation = Rotation.CCW;
                    Side = CubeSide.Up;
                    break;

                case "d":
                    Rotation = Rotation.CW;
                    Side = CubeSide.Down;
                    break;

                case "di":
                    Rotation = Rotation.CCW;
                    Side = CubeSide.Down;
                    break;

                default:
                    throw new ArgumentException("Value is not valid notation", "notation");
            }
        }

        public override string ToString()
        {
            string text = string.Empty;

            switch (Side)
            {
                case CubeSide.Front: text += "Front "; break;
                case CubeSide.Back: text += "Back "; break;
                case CubeSide.Right: text += "Right "; break;
                case CubeSide.Left: text += "Left "; break;
                case CubeSide.Up: text += "Up "; break;
                case CubeSide.Down: text += "Down "; break;
            }

            text += (Rotation == Rotation.CW) ? "CW" : "CCW";
            return text;
        }
    }
}

namespace RubiksCubeSolver.Rubiks
{
    /// <summary>
    /// Specifies the rotation direction
    /// </summary>
    public enum Rotation
    {
        /// <summary>
        /// Clockwise
        /// </summary>
        CW,
        /// <summary>
        /// Counter-clockwise
        /// </summary>
        CCW
    }

    /// <summary>
    /// Specifies the sides of a cube
    /// </summary>
    public enum CubeSide
    {
        None,
        /// <summary>
        /// The front of the cube
        /// </summary>
        Front,
        /// <summary>
        /// The back of the cube
        /// </summary>
        Back,
        /// <summary>
        /// The right of the cube
        /// </summary>
        Right,
        /// <summary>
        /// The left of the cube
        /// </summary>
        Left,
        /// <summary>
        /// The top of the cube
        /// </summary>
        Up,
        /// <summary>
        /// The bottom of the cube
        /// </summary>
        Down
    }

    /// <summary>
    /// Specifies a defect with a cube color configuration
    /// </summary>
    public enum ColorDefectType
    {
        /// <summary>
        /// There are too many instances of one color
        /// </summary>
        TooMany,
        /// <summary>
        /// There are too many distinct colors (7+)
        /// </summary>
        TooManyDistinct
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;

namespace RubiksCubeSolver.Rubiks
{
    /// <summary>
    /// Represents a 3x3 rubiks cube
    /// </summary>
    class RubiksCube : ICloneable
    {
        private readonly Color[][,] _origColors;

        private Color[][,] _allColors = new Color[6][,];
        /// <summary>
        /// Gets the color matrix for this cube. Face arrays are as follows:
        /// Front 0, Back 1, Right 2, left 3, Up 4, Down 5
        /// </summary>
        public Color[][,] AllColors
        {
            get { return _allColors; }
        }

        public Color[,] FrontColors
        {
            get { return _allColors[0]; }
            private set
            {
                if (_allColors[0] == value) return;
                _allColors[0] = value;
            }
        }

        public Color[,] BackColors
        {
            get { return _allColors[1]; }
            private set
            {
                if (_allColors[1] == value) return;
                _allColors[1] = value;
            }
        }

        public Color[,] RightColors
        {
            get { return _allColors[2]; }
            private set
            {
                if (_allColors[2] == value) return;
                _allColors[2] = value;
            }
        }

        public Color[,] LeftColors
        {
            get { return _allColors[3]; }
            private set
            {
                if (_allColors[3] == value) return;
                _allColors[3] = value;
            }
        }

        public Color[,] UpColors
        {
            get { return _allColors[4]; }
            private set
            {
                if (_allColors[4] == value) return;
                _allColors[4] = value;
            }
        }

        public Color[,] DownColors
        {
            get { return _allColors[5]; }
            private set
            {
                if (_allColors[5] == value) return;
                _allColors[5] = value;
            }
        }

        /// <summary>
        /// Gets whether the cube has valid, solvable color quantities.
        /// The cube needs 9 of 6 distinct colors
        /// </summary>
        public bool HasValidColorQuantities
        {
            get
            {
                var scheme = GetColorScheme();
                int frontCount = 0;
                int backCount = 0;
                int rightCount = 0;
                int leftCount = 0;
                int upCount = 0;
                int downCount = 0;
                var colors = GetColorsFlattened();

                foreach (Color color in colors)
                {
                    if (color.RgbEquals(scheme.FrontColor)) frontCount++;
                    else if (color.RgbEquals(scheme.BackColor)) backCount++;
                    else if (color.RgbEquals(scheme.RightColor)) rightCount++;
                    else if (color.RgbEquals(scheme.LeftColor)) leftCount++;
                    else if (color.RgbEquals(scheme.UpColor)) upCount++;
                    else if (color.RgbEquals(scheme.DownColor)) downCount++;
                }

                return (frontCount == 6 && backCount == 6 && rightCount == 6 &&
                        leftCount == 6 && upCount == 6 && downCount == 6);
            }
        }

        /// <summary>
        /// Gets whether this cube is solved
        /// </summary>
        public bool Solved
        {
            get
            {
                // Iterate all faces and check to see if they consist of one color
                foreach (var array in AllColors)
                {
                    Color lastColor = array[0, 0];

                    foreach (var color in array)
                    {
                        if (lastColor != color) return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Creates an instance of the RubiksCube
        /// </summary>
        /// <param name="colors">Face arrays are as follows:
        /// Front 0, Back 1, Right 2, left 3, Up 4, Down 5</param>
        public RubiksCube(Color[][,] colors)
        {
            _origColors = CloneColors(colors);
            // Clone so we dont modify the original matrix
            _allColors = colors;
        }

        /// <summary>
        /// Occurs when a single move has been made
        /// </summary>
        public event EventHandler<CubeMove> MoveMade;
        /// <summary>
        /// Raises the MoveMade event
        /// </summary>
        protected virtual void OnMoveMade(CubeMove move)
        {
            if (MoveMade != null)
                MoveMade(this, move);
        }

        /// <summary>
        /// Resets the colors to the cubes unmodified state
        /// </summary>
        public void Restore()
        {
            // Clone original so we do not modify it
            _allColors = CloneColors(_origColors);
        }

        /// <summary>
        /// Creates a solved cube from a specified color scheme
        /// </summary>
        public static RubiksCube Create(CubeColorScheme scheme)
        {
            var colors = new Color[6][,];
            colors[0] = CreateFace(scheme.FrontColor);
            colors[1] = CreateFace(scheme.BackColor);
            colors[2] = CreateFace(scheme.RightColor);
            colors[3] = CreateFace(scheme.LeftColor);
            colors[4] = CreateFace(scheme.UpColor);
            colors[5] = CreateFace(scheme.DownColor);
            return new RubiksCube(colors);
        }

        /// <summary>
        /// Gets the color scheme for this cube
        /// </summary>
        public CubeColorScheme GetColorScheme()
        {
            return new CubeColorScheme(
                FrontColors[1, 1],
                BackColors[1, 1],
                RightColors[1, 1],
                LeftColors[1, 1],
                UpColors[1, 1],
                DownColors[1, 1]);
        }

        public ColorDefect GetColorDefects()
        {
            var scheme = GetColorScheme();
            int frontCount = 0;
            int backCount = 0;
            int rightCount = 0;
            int leftCount = 0;
            int upCount = 0;
            int downCount = 0;
            var colors = GetColorsFlattened();

            foreach (Color color in colors)
            {
                if (color.RgbEquals(scheme.FrontColor)) frontCount++;
                else if (color.RgbEquals(scheme.BackColor)) backCount++;
                else if (color.RgbEquals(scheme.RightColor)) rightCount++;
                else if (color.RgbEquals(scheme.LeftColor)) leftCount++;
                else if (color.RgbEquals(scheme.UpColor)) upCount++;
                else if (color.RgbEquals(scheme.DownColor)) downCount++;
            }

            // Todo: your face!
            //   if (frontCount)

            return new ColorDefect(null, ColorDefectType.TooMany);
        }

        /// <summary>
        /// Reset the cube to its solved state
        /// </summary>
        public void CleanSlate()
        {
            var cube = Create(GetColorScheme());
            _allColors = cube._allColors;
        }

        /// <summary>
        /// Creates a 3x3 face with a solid color
        /// </summary>
        public static Color[,] CreateFace(Color faceColor)
        {
            return new[,]
            {
               {faceColor, faceColor, faceColor},
               {faceColor, faceColor, faceColor},
               {faceColor, faceColor, faceColor}
            };
        }
        /// <summary>
        /// Gets a cube face from the CubeSide specified
        /// </summary>
        /// <returns>Null, if CubeSide.None specified</returns>
        private Color[,] GetFaceColors(CubeSide side)
        {
            switch (side)
            {
                case CubeSide.Back: return BackColors;
                case CubeSide.Front: return FrontColors;
                case CubeSide.Left: return LeftColors;
                case CubeSide.Right: return RightColors;
                case CubeSide.Up: return UpColors;
                case CubeSide.Down: return DownColors;
                default: return null;
            }
        }

        /// <summary>
        /// Sets the colors for the cube side specified
        /// </summary>
        private void SetSide(CubeSide side, Color[,] value)
        {
            switch (side)
            {
                case CubeSide.Back: BackColors = value; break;
                case CubeSide.Front: FrontColors = value; break;
                case CubeSide.Left: LeftColors = value; break;
                case CubeSide.Right: RightColors = value; break;
                case CubeSide.Up: UpColors = value; break;
                case CubeSide.Down: DownColors = value; break;
            }
        }

        /// <summary>
        /// Rotates the colors only on the surface of the side
        /// </summary>
        private void RotateFace(CubeSide side, Rotation rotation)
        {
            var faceToRotate = GetFaceColors(side);
            var newFace = new Color[3, 3];

            if (rotation == Rotation.CW)
            {
                for (int i = 2; i >= 0; i--)
                    for (int i2 = 0; i2 < 3; i2++)
                        newFace[i2, 2 - i] = faceToRotate[i, i2];
            }
            else
            {
                for (int i = 2; i >= 0; i--)
                    for (int i2 = 0; i2 < 3; i2++)
                        newFace[2 - i, i2] = faceToRotate[i2, i];
            }

            SetSide(side, newFace);
        }

        /// <summary>
        /// Gets the color matrix colors as a flat color array
        /// </summary>
        public IEnumerable<Color> GetColorsFlattened()
        {
            var colorStack = new Stack<Color>();

            foreach (Color[,] array in _allColors)
            {
                for (int row = 0; row < array.GetLength(0); row++)
                {
                    for (int clm = 0; clm < array.GetLength(1); clm++)
                        colorStack.Push(array[row, clm]);
                }
            }

            return colorStack;
        }


        private static Color[][,] CloneColors(Color[][,] source)
        {
            var cloned = new Color[source.Length][,];

            // Iterate array
            for (int i = 0; i < source.Length; i++)
            {
                cloned[i] = new Color[3, 3];

                for (int row = 0; row < source[i].GetLength(0); row++)
                {
                    for (int clm = 0; clm < source[i].GetLength(1); clm++)
                    {
                        cloned[i][row, clm] = source[i][row, clm];
                    }
                }
            }

            return cloned;
        }

        public void MakeMove(CubeMove move)
        {
            MakeMove(move.Side, move.Rotation);
        }

        public void MakeMove(CubeSide side, Rotation rotation)
        {
            if (side == CubeSide.None) return;
            RotateFace(side, rotation);
            // Rotate non-face colors
            // No need to set middle colors as newColors is a full copy
            var newColors = CloneColors(_allColors);

            switch (side)
            {
                #region Front Shift
                case CubeSide.Front:

                    if (rotation == Rotation.CW)
                    {
                        // Shift Left to up
                        newColors[4][2, 0] = LeftColors[2, 2];
                        newColors[4][2, 1] = LeftColors[1, 2];
                        newColors[4][2, 2] = LeftColors[0, 2];
                        // Shift up to right
                        newColors[2][0, 0] = UpColors[2, 0];
                        newColors[2][1, 0] = UpColors[2, 1];
                        newColors[2][2, 0] = UpColors[2, 2];
                        // Shift right to down
                        newColors[5][0, 2] = RightColors[0, 0];
                        newColors[5][0, 1] = RightColors[1, 0];
                        newColors[5][0, 0] = RightColors[2, 0];
                        // Shift down to left
                        newColors[3][0, 2] = DownColors[0, 0];
                        newColors[3][1, 2] = DownColors[0, 1];
                        newColors[3][2, 2] = DownColors[0, 2];
                    }
                    else
                    {
                        // 0 Front, 1 back, 2 right, 3 left, 4 up, 5 down
                        // Shift up to left
                        newColors[3][2, 2] = UpColors[2, 0];
                        newColors[3][1, 2] = UpColors[2, 1];
                        newColors[3][0, 2] = UpColors[2, 2];
                        // Shift right to up
                        newColors[4][2, 0] = RightColors[0, 0];
                        newColors[4][2, 1] = RightColors[1, 0];
                        newColors[4][2, 2] = RightColors[2, 0];
                        // Shift down to right
                        newColors[2][0, 0] = DownColors[0, 2];
                        newColors[2][1, 0] = DownColors[0, 1];
                        newColors[2][2, 0] = DownColors[0, 0];
                        // Shift left to down
                        newColors[5][0, 0] = LeftColors[0, 2];
                        newColors[5][0, 1] = LeftColors[1, 2];
                        newColors[5][0, 2] = LeftColors[2, 2];
                    }

                    break;
                #endregion

                #region Back Shift
                case CubeSide.Back:

                    if (rotation == Rotation.CCW)
                    {
                        // Shift Left to up
                        newColors[4][0, 0] = LeftColors[2, 0];
                        newColors[4][0, 1] = LeftColors[1, 0];
                        newColors[4][0, 2] = LeftColors[0, 0];
                        // Shift up to right
                        newColors[2][0, 2] = UpColors[0, 0];
                        newColors[2][1, 2] = UpColors[0, 1];
                        newColors[2][2, 2] = UpColors[0, 2];
                        // Shift right to down
                        newColors[5][2, 2] = RightColors[0, 2];
                        newColors[5][2, 1] = RightColors[1, 2];
                        newColors[5][2, 0] = RightColors[2, 2];
                        // Shift down to left
                        newColors[3][0, 0] = DownColors[2, 0];
                        newColors[3][1, 0] = DownColors[2, 1];
                        newColors[3][2, 0] = DownColors[2, 2];
                    }
                    else
                    {
                        // 0 Front, 1 back, 2 right, 3 left, 4 up, 5 down
                        // Shift up to left
                        newColors[3][2, 0] = UpColors[0, 0];
                        newColors[3][1, 0] = UpColors[0, 1];
                        newColors[3][0, 0] = UpColors[0, 2];
                        // Shift right to up
                        newColors[4][0, 0] = RightColors[0, 2];
                        newColors[4][0, 1] = RightColors[1, 2];
                        newColors[4][0, 2] = RightColors[2, 2];
                        // Shift down to right
                        newColors[2][0, 2] = DownColors[2, 2];
                        newColors[2][1, 2] = DownColors[2, 1];
                        newColors[2][2, 2] = DownColors[2, 0];
                        // Shift left to down
                        newColors[5][2, 0] = LeftColors[0, 0];
                        newColors[5][2, 1] = LeftColors[1, 0];
                        newColors[5][2, 2] = LeftColors[2, 0];
                    }

                    break;
                #endregion

                #region Right Shift
                case CubeSide.Right:

                    if (rotation == Rotation.CW)
                    {
                        // 0 Front, 1 back, 2 right, 3 left, 4 up, 5 down
                        // Shift front to up
                        newColors[4][0, 2] = FrontColors[0, 2];
                        newColors[4][1, 2] = FrontColors[1, 2];
                        newColors[4][2, 2] = FrontColors[2, 2];
                        // Shift up to back
                        newColors[1][2, 0] = UpColors[0, 2];
                        newColors[1][1, 0] = UpColors[1, 2];
                        newColors[1][0, 0] = UpColors[2, 2];
                        // Shift back to down
                        newColors[5][0, 2] = BackColors[2, 0];
                        newColors[5][1, 2] = BackColors[1, 0];
                        newColors[5][2, 2] = BackColors[0, 0];
                        // Shift down to front
                        newColors[0][0, 2] = DownColors[0, 2];
                        newColors[0][1, 2] = DownColors[1, 2];
                        newColors[0][2, 2] = DownColors[2, 2];
                    }
                    else
                    {
                        // 0 Front, 1 back, 2 right, 3 left, 4 up, 5 down
                        // Shift up to front
                        newColors[0][0, 2] = UpColors[0, 2];
                        newColors[0][1, 2] = UpColors[1, 2];
                        newColors[0][2, 2] = UpColors[2, 2];
                        // Shift back to up
                        newColors[4][0, 2] = BackColors[2, 0];
                        newColors[4][1, 2] = BackColors[1, 0];
                        newColors[4][2, 2] = BackColors[0, 0];
                        // Shift down to back
                        newColors[1][0, 0] = DownColors[2, 2];
                        newColors[1][1, 0] = DownColors[1, 2];
                        newColors[1][2, 0] = DownColors[0, 2];
                        // Shift front to down
                        newColors[5][0, 2] = FrontColors[0, 2];
                        newColors[5][1, 2] = FrontColors[1, 2];
                        newColors[5][2, 2] = FrontColors[2, 2];
                    }

                    break;
                #endregion

                #region Left Shift
                case CubeSide.Left:

                    if (rotation == Rotation.CW)
                    {
                        // Shift up to front
                        newColors[0][0, 0] = UpColors[0, 0];
                        newColors[0][1, 0] = UpColors[1, 0];
                        newColors[0][2, 0] = UpColors[2, 0];
                        // Shift front to down
                        newColors[5][0, 0] = FrontColors[0, 0];
                        newColors[5][1, 0] = FrontColors[1, 0];
                        newColors[5][2, 0] = FrontColors[2, 0];
                        // Shift down to back
                        newColors[1][2, 2] = DownColors[0, 0];
                        newColors[1][1, 2] = DownColors[1, 0];
                        newColors[1][0, 2] = DownColors[2, 0];
                        // Shift back to up
                        newColors[4][2, 0] = BackColors[0, 2];
                        newColors[4][1, 0] = BackColors[1, 2];
                        newColors[4][0, 0] = BackColors[2, 2];
                    }
                    else
                    {
                        // 0 Front, 1 back, 2 right, 3 left, 4 up, 5 down
                        // Shift front to up
                        newColors[4][0, 0] = FrontColors[0, 0];
                        newColors[4][1, 0] = FrontColors[1, 0];
                        newColors[4][2, 0] = FrontColors[2, 0];
                        // Shift down to front
                        newColors[0][0, 0] = DownColors[0, 0];
                        newColors[0][1, 0] = DownColors[1, 0];
                        newColors[0][2, 0] = DownColors[2, 0];
                        // Shift back to down
                        newColors[5][0, 0] = BackColors[2, 2];
                        newColors[5][1, 0] = BackColors[1, 2];
                        newColors[5][2, 0] = BackColors[0, 2];
                        // Shift up to back
                        newColors[1][0, 2] = UpColors[2, 0];
                        newColors[1][1, 2] = UpColors[1, 0];
                        newColors[1][2, 2] = UpColors[0, 0];
                    }

                    break;
                #endregion

                #region Up Shift
                case CubeSide.Up:

                    // Rotate outercolors
                    if (rotation == Rotation.CW)
                    {
                        // No need to set middle colors as newColors is a full copy
                        // Shift right to front
                        newColors[0][0, 0] = RightColors[0, 0];
                        newColors[0][0, 1] = RightColors[0, 1];
                        newColors[0][0, 2] = RightColors[0, 2];
                        // Shift front to left
                        newColors[3][0, 0] = FrontColors[0, 0];
                        newColors[3][0, 1] = FrontColors[0, 1];
                        newColors[3][0, 2] = FrontColors[0, 2];
                        // Shift left to back
                        newColors[1][0, 0] = LeftColors[0, 0];
                        newColors[1][0, 1] = LeftColors[0, 1];
                        newColors[1][0, 2] = LeftColors[0, 2];
                        // Shift back to right
                        newColors[2][0, 0] = BackColors[0, 0];
                        newColors[2][0, 1] = BackColors[0, 1];
                        newColors[2][0, 2] = BackColors[0, 2];
                    }
                    else
                    {
                        //  0 Front, 1 back, 2 right, 3 left, 4 up, 5 down
                        // Shift front to right
                        newColors[2][0, 0] = FrontColors[0, 0];
                        newColors[2][0, 1] = FrontColors[0, 1];
                        newColors[2][0, 2] = FrontColors[0, 2];
                        // Shift left to front
                        newColors[0][0, 0] = LeftColors[0, 0];
                        newColors[0][0, 1] = LeftColors[0, 1];
                        newColors[0][0, 2] = LeftColors[0, 2];
                        // Shift back to left
                        newColors[3][0, 0] = BackColors[0, 0];
                        newColors[3][0, 1] = BackColors[0, 1];
                        newColors[3][0, 2] = BackColors[0, 2];
                        // Shift right to back
                        newColors[1][0, 0] = RightColors[0, 0];
                        newColors[1][0, 1] = RightColors[0, 1];
                        newColors[1][0, 2] = RightColors[0, 2];
                    }
                    break;
                #endregion

                #region Down Shift
                case CubeSide.Down:

                    // Rotate outercolors
                    if (rotation == Rotation.CCW)
                    {
                        //  0 Front, 1 back, 2 right, 3 left, 4 up, 5 down
                        // Shift right to front
                        newColors[0][2, 0] = RightColors[2, 0];
                        newColors[0][2, 1] = RightColors[2, 1];
                        newColors[0][2, 2] = RightColors[2, 2];
                        // Shift front to left
                        newColors[3][2, 0] = FrontColors[2, 0];
                        newColors[3][2, 1] = FrontColors[2, 1];
                        newColors[3][2, 2] = FrontColors[2, 2];
                        // Shift left to back
                        newColors[1][2, 0] = LeftColors[2, 0];
                        newColors[1][2, 1] = LeftColors[2, 1];
                        newColors[1][2, 2] = LeftColors[2, 2];
                        // Shift back to right
                        newColors[2][2, 0] = BackColors[2, 0];
                        newColors[2][2, 1] = BackColors[2, 1];
                        newColors[2][2, 2] = BackColors[2, 2];
                    }
                    else
                    {
                        // Shift front to right
                        newColors[2][2, 0] = FrontColors[2, 0];
                        newColors[2][2, 1] = FrontColors[2, 1];
                        newColors[2][2, 2] = FrontColors[2, 2];
                        // Shift left to front
                        newColors[0][2, 0] = LeftColors[2, 0];
                        newColors[0][2, 1] = LeftColors[2, 1];
                        newColors[0][2, 2] = LeftColors[2, 2];
                        // Shift back to left
                        newColors[3][2, 0] = BackColors[2, 0];
                        newColors[3][2, 1] = BackColors[2, 1];
                        newColors[3][2, 2] = BackColors[2, 2];
                        // Shift right to back
                        newColors[1][2, 0] = RightColors[2, 0];
                        newColors[1][2, 1] = RightColors[2, 1];
                        newColors[1][2, 2] = RightColors[2, 2];
                    }
                    break;
                #endregion
            }

            _allColors = newColors;
            OnMoveMade(new CubeMove(side, rotation));
        }

        /// <summary>
        /// Creates a new RubiksCube from this instance
        /// </summary>
        public object Clone()
        {
            var colors = CloneColors(_allColors);
            return new RubiksCube(colors);
        }
    }
}

namespace RubiksCubeSolver.Rubiks.Solver
{
    class RubiksBruteForcerParams
    {
        public int MaxMoves { get; private set; }

        public RubiksCube Cube { get; private set; }

        public RubiksBruteForcerParams(RubiksCube cube, int maxMoves)
        {
            Cube = cube;
            MaxMoves = maxMoves;
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCubeSolver.Rubiks.Solver
{
    partial class RubiksBruteForcer
    {
        private class BruteForceWorker : BackgroundWorker
        {
            protected override void OnDoWork(DoWorkEventArgs e)
            {
                base.OnDoWork(e);
                var p = (RubiksBruteForcerParams)e.Argument;


            }

            private void BruteForce(RubiksCube cube)
            {
                const int MAX_MOVE_DEPTH = 100;
                int depth = 1;

                while (!cube.Solved && depth < MAX_MOVE_DEPTH)
                {
                    var moveSet = new CubeMove[depth];

                    for (int i = 0; i < depth; i++)
                    {
                        for (int side = 1; side <= 6; side++)
                        {
                              
                        }
                    }

                    depth++;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RubiksCubeSolver.Views
{
    /// <summary>
    /// Represents predefined colors to be picked from
    /// </summary>
    [DefaultEvent("SelectedIndexChanged")]
    class ColorStrip : Panel
    {
        #region Properties
        private Color[] _colors;
        [Description("The array of colors to pick from")]
        [Category("Appearance")]
        [DefaultValue(null)]
        public Color[] Colors
        {
            get { return _colors; }
            set
            {
                _selectedIndex = -1; // Let the user do the reset selection logic
                _colors = value;
                this.Invalidate();
            }
        }

        private int _selectedIndex = -1;
        [Description("The index of the color selected")]
        [Category("Input")]
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex == value) return;

                if (_colors == null || _colors.Length == 0)
                    throw new InvalidOperationException("There are no colors to selected");

                if (value >= _colors.Length)
                    throw new ArgumentOutOfRangeException("Index must be less than Colors.Count", "value");

                _selectedIndex = value;
                this.Invalidate();
                SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the currently selected color
        /// </summary>
        [Browsable(false)]
        public Color SelectedColor
        {
            get
            {
                if (_selectedIndex == -1) return Color.Empty;
                return _colors[_selectedIndex];
            }
        }

        /// <summary>
        /// Gets whether a color is selected
        /// </summary>
        [Browsable(false)]
        public bool HasSelection
        {
            get { return _selectedIndex != -1; }
        }
        #endregion

        [Description("Occurs when the value of the SelectedIndex property has changed")]
        public event EventHandler SelectedIndexChanged = delegate { };

        public ColorStrip()
        {
            base.DoubleBuffered = true;
        }

        private RectangleF[] GetColorRects()
        {
            if (_colors == null) return new RectangleF[] { };
            float width = (float)Width / _colors.Length;

            var rects = new List<RectangleF>();

            for (int i = 0; i < _colors.Length; i++)
            {
                rects.Add(new RectangleF(width * i, 0, width, Height));
            }

            return rects.ToArray();
        }

        private RectangleF GetColorRect(int index)
        {
            float width = (float)Width / _colors.Length;
            return new RectangleF(index * width, 0, width, Height);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            var rects = GetColorRects();

            for (int i = 0; i < rects.Length; i++)
            {
                if (rects[i].Contains(e.Location))
                    SelectedIndex = i;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_colors == null) return;
           
            Color borderColor = Enabled ? Color.Black : Color.Gray;
            Pen borderPen = new Pen(borderColor, 1);

            // Draw colors
            for (int i = 0; i < _colors.Length; i++)
            {
                var rect = GetColorRect(i);
                var brush = new SolidBrush(_colors[i]);
                if (!Enabled) brush.Color = brush.Color.Desaturate();
                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.DrawRectangle(borderPen, rect.X, rect.Y - 1, rect.Width, rect.Height);
            }

            // Draw master border
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, borderColor, ButtonBorderStyle.Solid);

            // Draw selected color
            for (int i = 0; i < _colors.Length; i++)
            {
                if (i == SelectedIndex)
                {
                    var rect = GetColorRect(i);
                    const int RGBMAX = 255;
                    const int BORDER_WIDTH = 4;

                    var color = Color.FromArgb(RGBMAX - _colors[i].R,
                        RGBMAX - _colors[i].G, RGBMAX - _colors[i].B);

                    Pen pen = new Pen(color, BORDER_WIDTH);
                    e.Graphics.DrawRectangle(pen, rect.X + BORDER_WIDTH / 2,
                        rect.Y + BORDER_WIDTH / 2, rect.Width - BORDER_WIDTH, rect.Height - BORDER_WIDTH);

                    // Draw inner small border
                    e.Graphics.DrawRectangle(borderPen, rect.X + BORDER_WIDTH, rect.Y + BORDER_WIDTH,
                        rect.Width - BORDER_WIDTH * 2, rect.Height - BORDER_WIDTH * 2);

                    // Draw outer 
                    float width = rect.Width;
                    if (i == _colors.Length - 1) width = this.Width - rect.X - 1;
                    e.Graphics.DrawRectangle(borderPen, rect.X, rect.Y, width, rect.Height - 1);
                }
            }
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using RubiksCubeSolver.ColorGrid;
using RubiksCubeSolver.Rubiks;

namespace RubiksCubeSolver.Views
{
    /// <summary>
    /// Specifies a mode for adjusting the cube colors
    /// </summary>
    public enum ClickMode
    {
        /// <summary>
        /// No effect will happen
        /// </summary>
        None,
        /// <summary>
        /// The user will be able to rotate with the left and right mouse buttons
        /// </summary>
        Rotation,
        /// <summary>
        /// The user will be able to set colors with the right mouse buttons
        /// </summary>
        ColorSet
    }

    /// <summary>
    /// Represents a grid point mapper, to build point arrays on a grid
    /// </summary>
    [DefaultEvent("PointsChanged")]
    class CubeFaceDisplay : Control
    {
        private RectangleF _hoveredRect;

        #region Properties
        private ColorGridStyle Style
        {
            get
            {
                return new ColorGridStyle(GetDisplayColors(), 0.05f, RoundedRadius);
            }
        }

        private ClickMode _clickMode;
        [Category("Appearance")]
        [DefaultValue(ClickMode.None)]
        [Description("The operations to perform when clicking")]
        public ClickMode ClickMode
        {
            get { return _clickMode; }
            set
            {
                if (_clickMode == value) return;
                _clickMode = value;

                if (_clickMode == ClickMode.ColorSet)
                {
                    this.Cursor = Cursors.Hand;
                }
                else if (_clickMode == ClickMode.Rotation)
                {
                    this.Cursor = Cursors.NoMoveHoriz;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }

                this.Invalidate();
            }
        }

        private CubeSide _face = CubeSide.None;
        [Category("Appearance")]
        [DefaultValue(CubeSide.None)]
        [Description("The face or side of the cube to handle and display")]
        public CubeSide Face
        {
            get { return _face; }
            set
            {
                _face = value;
                this.Invalidate();
            }
        }

        private Color _newColor;
        [Description("Determines the color to be set when right-clicking a cell")]
        [Category("Behavior")]
        public Color NewColor
        {
            get { return _newColor; }
            set
            {
                if (_newColor == value) return;
                _newColor = value;
                this.Invalidate();
            }
        }

        private RubiksCube _rubiksCube;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RubiksCube RubiksCube
        {
            get { return _rubiksCube; }
            set
            {
                _rubiksCube = value;
                this.Invalidate();
            }
        }

        private Color[,] FaceColors
        {
            get
            {
                if (_rubiksCube == null) return null;

                switch (Face)
                {
                    case CubeSide.Front: return RubiksCube.FrontColors;
                    case CubeSide.Back: return RubiksCube.BackColors;
                    case CubeSide.Right: return RubiksCube.RightColors;
                    case CubeSide.Left: return RubiksCube.LeftColors;
                    case CubeSide.Up: return RubiksCube.UpColors;
                    case CubeSide.Down: return RubiksCube.DownColors;
                    default: return null;
                }
            }
        }

        private int _roundedRadius = 5;
        [Category("Appearance")]
        [DefaultValue(5)]
        [Description("The corner radius of the rounded rectangles used with this control")]
        public int RoundedRadius
        {
            get { return _roundedRadius; }
            set
            {
                _roundedRadius = value;
                this.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "Transparent")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        #endregion

        public CubeFaceDisplay()
        {
            ClickMode = ClickMode.None;

            this.SetStyle(ControlStyles.ResizeRedraw |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.SupportsTransparentBackColor, true);

            base.BackColor = Color.Transparent;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var rect = ColorGridRendering.GetCellRectFromPosition
                (Style, e.Location, this.ClientSize);

            if (rect != _hoveredRect)
            {
                _hoveredRect = rect;
                OnHoverCellChanged(_hoveredRect);

                if (ClickMode != ClickMode.None)
                    this.Invalidate();
            }
        }

        #region Overrides
        [Description("Occurs when a new cell has been hovered over by the mouse")]
        public event EventHandler<RectangleF> HoveredCellChanged;
        /// <summary>
        /// Raises the HoveredCellChanged event
        /// </summary>
        protected virtual void OnHoverCellChanged(RectangleF cellPos)
        {
            if (HoveredCellChanged != null)
            {
                HoveredCellChanged(this, cellPos);
            }
        }

        [Description("Occurs when a cell has been clicked by the mouse")]
        public event EventHandler<CellMouseClickedEventArgs> CellMouseClicked;
        /// <summary>
        /// Raises the CellMouseClicked event
        /// </summary>
        protected virtual void OnCellMouseClicked(Point pos, MouseButtons buttons)
        {
            if (CellMouseClicked != null)
            {
                var args = new CellMouseClickedEventArgs(pos, buttons);
                CellMouseClicked(this, args);
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Invalidate();
        }

        /// <summary>
        /// Gets the appropriate background color for painting
        /// </summary>
        private Color GetBackColor()
        {
            if (this.BackColor == Color.Transparent && this.Parent != null)
                return Parent.BackColor;
            else
                return BackColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.Clear(GetBackColor());
            ColorGridRendering.Draw(Style, e.Graphics, ClientSize, Enabled);

            if (ClickMode == ClickMode.ColorSet && _hoveredRect != Rectangle.Empty)
            {
                var path = RoundedRectangleF.Create(_hoveredRect, RoundedRadius);
                Pen pen = new Pen(Color.White, 3f);
                pen.Alignment = PenAlignment.Center;
                e.Graphics.DrawPath(pen, path);
            }
            else if (ClickMode == ClickMode.Rotation)
            {
                var rect = ColorGridRendering.GetMasterRectangle(Style, this.Size);

                if (rect.Contains(this.PointToClient(Cursor.Position)))
                {
                    //var path = RoundedRectangleF.Create(rect, RoundedRadius);
                    //Pen pen = new Pen(Color.Gray, 8f);
                    // e.Graphics.DrawPath(pen, path);
                    ControlPaint.DrawBorder3D(e.Graphics, rect.ToRect(),
                        Border3DStyle.Flat);
                }
            }
        }

        private Color[,] GetDisplayColors()
        {
            var face = FaceColors;
            // If face == null then the CubeSide enum has not been set
            if (_rubiksCube == null || face == null)
            {
                return RubiksCube.CreateFace(Color.White);
            }
            else return face;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoveredRect = Rectangle.Empty;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            var point = ColorGridRendering.GetGridPointFromPosition
              (Style, e.Location, ClientSize);

            if (point.X != -1 && ClickMode == ClickMode.ColorSet &&
                NewColor != Color.Empty)
            {
                FaceColors[point.X, point.Y] = NewColor;
                OnCellMouseClicked(point, e.Button);
            }
            else if (ClickMode == ClickMode.Rotation)
            {
                var r = (e.Button == MouseButtons.Left) ? Rotation.CCW : Rotation.CW;
                _rubiksCube.MakeMove(Face, r);
                InvalidateOtherCubeControls();
            }

            this.Invalidate();
        }

        // Rotation of a single face results in change of colors in othe faces
        // so show the changes right away
        private void InvalidateOtherCubeControls()
        {
            if (Parent == null) return;

            foreach (Control control in Parent.Controls)
            {
                var display = control as CubeFaceDisplay;
                // Equality with this because we are already invalidating elsewhere
                // and have the same rubik cube
                if (display != null && display != this &&
                    display.RubiksCube == this.RubiksCube) display.Invalidate();
            }
        }

        protected override Size DefaultSize
        {
            get { return new Size(300, 300); }
        }
        #endregion
    }


    /// <summary>
    /// Represents event arguments for the cref="CellMouseClicked" event
    /// </summary>
    class CellMouseClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the position of the cell
        /// </summary>
        public Point CellPosition { get; private set; }

        /// <summary>
        /// Gets the x position of the cell
        /// </summary>
        public int CellX
        {
            get { return CellPosition.X; }
        }

        /// <summary>
        /// Gets the y position of the cell
        /// </summary>
        public int CellY
        {
            get { return CellPosition.Y; }
        }

        public MouseButtons Buttons { get; private set; }

        public CellMouseClickedEventArgs(Point cellPos, MouseButtons buttons)
        {
            CellPosition = cellPos;
            Buttons = buttons;
        }
    }
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RubiksCubeSolver.Rubiks;
using System.Collections.Generic;
using System.Linq;

namespace RubiksCubeSolver.Views
{
    public partial class MainForm : Form
    {
        private RubiksCube _rubiksCube;

        /// <summary>
        /// Gets all of the cube displays on the form
        /// </summary>
        private IEnumerable<CubeFaceDisplay> CubeDisplays
        {
            get { return Controls.OfType<CubeFaceDisplay>(); }
        }

        public MainForm()
        {
            InitializeComponent();
            colorStrip.Colors = Settings.Instance.Palette;
            CreateRubiksCube();
            SetHoverEffect();
        }

        private void CreateRubiksCube()
        {
            //  else 
            //     _rubiksCube = new RubiksCube(Settings.Instance.CubeColors);

            _rubiksCube = CreateScrambledCube();
           // _rubiksCube = RubiksCube.Create(CubeColorScheme.DevsScheme);
            Debug.WriteLine(_rubiksCube.Solved);
            _rubiksCube.MoveMade += RubiksCubeMoveMade;
            foreach (var display in CubeDisplays)
                display.RubiksCube = _rubiksCube;
        }

        private void RubiksCubeMoveMade(object sender, CubeMove move)
        {
            if (_rubiksCube.Solved)
            {
                lblStatus.Text = "Cube Solved";
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                lblStatus.Text = "Last Move: " + move;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Settings.Instance.Palette = colorStrip.Colors;
            Settings.Instance.CubeColors = _rubiksCube.AllColors;
            //  Settings.Instance.Save();
        }

        private static RubiksCube CreateScrambledCube()
        {
            var colors = new[]
            {
            new[,] // Front face
            {
               {Color.White, Color.White, Color.White},
               {Color.White, Color.White, Color.White},
               {Color.White, Color.White, Color.White}
            },
             new[,] // Back face
            {
               {Color.Yellow, Color.Green, Color.Yellow},
               {Color.Red, Color.Yellow, Color.Blue},
               {Color.Red, Color.Yellow, Color.Red}
            },
            new[,] // Right face
            {
               {Color.Green, Color.Blue, Color.Orange},
               {Color.Green, Color.Blue, Color.Yellow},
               {Color.Green, Color.Green, Color.Green}
            },
            new[,] // Left face
            {
               {Color.Orange, Color.Orange, Color.Blue},
               {Color.Red, Color.Green, Color.Orange},
               {Color.Blue, Color.Green, Color.Blue}
            },

            new[,] // Up face
            {
               {Color.Blue, Color.Yellow, Color.Green},
               {Color.Blue, Color.Orange, Color.Yellow},
               {Color.Red, Color.Blue, Color.Red}
            },
            new[,] // Down face
            {
               {Color.Orange, Color.Red, Color.Orange},
               {Color.Red, Color.Red, Color.Orange},
               {Color.Yellow, Color.Orange, Color.Yellow}
            }
            };

            return new RubiksCube(colors);
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            _rubiksCube = CreateScrambledCube();
            SetDisplayedCube();
        }

        private void SetDisplayedCube()
        {
            foreach (var display in CubeDisplays)
            {
                display.RubiksCube = _rubiksCube;
            }
        }

        private void txtCommand_KeyDown(object sender, KeyEventArgs e)
        {
            txtCommand.BackColor = Color.FromKnownColor(KnownColor.Window);
            if (e.KeyCode != Keys.Enter) return;
            string lower = txtCommand.Text;

            if (lower == "clean slate")
            {
                _rubiksCube.CleanSlate();
            }

            string[] splitted = txtCommand.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string str in splitted)
            {
                try
                {
                    CubeMove move = new CubeMove(str);
                    _rubiksCube.MakeMove(move);
                }
                catch (ArgumentException)
                {
                    txtCommand.BackColor = Color.LightPink; 
                    return;
                }
            }

            e.SuppressKeyPress = true;
            txtCommand.Clear();
            this.Invalidate();
        }

        private void SetHoverEffect()
        {
            foreach (var display in CubeDisplays)
            {
                if (chkLockColors.Checked)
                {
                    display.ClickMode = ClickMode.Rotation;
                }
                else if (colorStrip.HasSelection)
                {
                    display.ClickMode = ClickMode.ColorSet;
                }
                else
                {
                    display.ClickMode = ClickMode.None;
                }
            }
        }

        private void chkLockColors_CheckedChanged(object sender, EventArgs e)
        {
            SetHoverEffect();
            colorStrip.Enabled = !chkLockColors.Checked;
        }

        private void colorStrip_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetHoverEffect();

            foreach (var display in CubeDisplays)
                display.NewColor = colorStrip.SelectedColor;
        }

        private void cubeDisplay_CellMouseClicked(object sender, CellMouseClickedEventArgs e)
        {
           var defects = _rubiksCube.GetColorDefects();

            // There are too many instances of certain colors
            if (defects.Type == ColorDefectType.TooMany)
            {
                StringBuilder SB = new StringBuilder();
                SB.Append("There is too much of: ");

                foreach (Color color in defects.Colors)
                {
                    SB.Append(color + ", ");
                }

                SB.Remove(SB.Length - 3, 2);
                lblErrorStatus.Text = SB.ToString();
            }
                // There are too many distinct colors
            else if (defects.Type == ColorDefectType.TooManyDistinct)
            {
                lblErrorStatus.Text = "There are too many distinct colors";
            }
        }

        private void cubeFront_CellMouseClicked(object sender, CellMouseClickedEventArgs e)
        {

        }
    }
}

namespace RubiksCubeSolver.Views
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnRevert = new System.Windows.Forms.Button();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblErrorStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.chkLockColors = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.colorStrip = new RubiksCubeSolver.Views.ColorStrip();
            this.cubeDown = new RubiksCubeSolver.Views.CubeFaceDisplay();
            this.cubeUp = new RubiksCubeSolver.Views.CubeFaceDisplay();
            this.cubeLeft = new RubiksCubeSolver.Views.CubeFaceDisplay();
            this.cubeRight = new RubiksCubeSolver.Views.CubeFaceDisplay();
            this.cubeBack = new RubiksCubeSolver.Views.CubeFaceDisplay();
            this.cubeFront = new RubiksCubeSolver.Views.CubeFaceDisplay();
            this.btnSolve = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(87, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 29);
            this.label1.TabIndex = 2;
            this.label1.Text = "Front";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(87, 299);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 29);
            this.label2.TabIndex = 4;
            this.label2.Text = "Back";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(311, 299);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 29);
            this.label3.TabIndex = 8;
            this.label3.Text = "Left";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(311, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 29);
            this.label4.TabIndex = 6;
            this.label4.Text = "Right";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(534, 299);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 29);
            this.label5.TabIndex = 12;
            this.label5.Text = "Down";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(544, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 29);
            this.label6.TabIndex = 10;
            this.label6.Text = "Up";
            // 
            // btnRevert
            // 
            this.btnRevert.Location = new System.Drawing.Point(441, 12);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(112, 37);
            this.btnRevert.TabIndex = 16;
            this.btnRevert.Text = "Reset";
            this.btnRevert.UseVisualStyleBackColor = true;
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // txtCommand
            // 
            this.txtCommand.AcceptsReturn = true;
            this.txtCommand.Location = new System.Drawing.Point(103, 12);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(243, 26);
            this.txtCommand.TabIndex = 17;
            this.txtCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCommand_KeyDown);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 20);
            this.label7.TabIndex = 18;
            this.label7.Text = "Command";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblErrorStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 616);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(683, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 19;
            this.statusStrip.Text = "statusStrip";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // lblErrorStatus
            // 
            this.lblErrorStatus.Name = "lblErrorStatus";
            this.lblErrorStatus.Size = new System.Drawing.Size(668, 17);
            this.lblErrorStatus.Spring = true;
            this.lblErrorStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkLockColors
            // 
            this.chkLockColors.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkLockColors.BackgroundImage = global::RubiksCubeSolver.Properties.Resources.Lock;
            this.chkLockColors.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.chkLockColors.Location = new System.Drawing.Point(577, 541);
            this.chkLockColors.Name = "chkLockColors";
            this.chkLockColors.Size = new System.Drawing.Size(84, 48);
            this.chkLockColors.TabIndex = 20;
            this.chkLockColors.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLockColors.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip.SetToolTip(this.chkLockColors, "When checked, colors can not be configured\r\nand rotation can be done with the lef" +
        "t and right mouse");
            this.chkLockColors.UseVisualStyleBackColor = true;
            this.chkLockColors.CheckedChanged += new System.EventHandler(this.chkLockColors_CheckedChanged);
            // 
            // colorStrip
            // 
            this.colorStrip.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))))};
            this.colorStrip.Location = new System.Drawing.Point(19, 541);
            this.colorStrip.Name = "colorStrip";
            this.colorStrip.Size = new System.Drawing.Size(552, 48);
            this.colorStrip.TabIndex = 15;
            this.colorStrip.Text = "colorStrip1";
            this.colorStrip.SelectedIndexChanged += new System.EventHandler(this.colorStrip_SelectedIndexChanged);
            // 
            // cubeDown
            // 
            this.cubeDown.Face = RubiksCubeSolver.Rubiks.CubeSide.Down;
            this.cubeDown.Location = new System.Drawing.Point(460, 331);
            this.cubeDown.Name = "cubeDown";
            this.cubeDown.NewColor = System.Drawing.Color.Empty;
            this.cubeDown.Size = new System.Drawing.Size(218, 190);
            this.cubeDown.TabIndex = 11;
            this.cubeDown.Text = "colorGridDisplay1";
            this.cubeDown.CellMouseClicked += new System.EventHandler<RubiksCubeSolver.Views.CellMouseClickedEventArgs>(this.cubeDisplay_CellMouseClicked);
            // 
            // cubeUp
            // 
            this.cubeUp.Face = RubiksCubeSolver.Rubiks.CubeSide.Up;
            this.cubeUp.Location = new System.Drawing.Point(460, 102);
            this.cubeUp.Name = "cubeUp";
            this.cubeUp.NewColor = System.Drawing.Color.Empty;
            this.cubeUp.Size = new System.Drawing.Size(218, 190);
            this.cubeUp.TabIndex = 9;
            this.cubeUp.Text = "colorGridDisplay1";
            this.cubeUp.CellMouseClicked += new System.EventHandler<RubiksCubeSolver.Views.CellMouseClickedEventArgs>(this.cubeDisplay_CellMouseClicked);
            // 
            // cubeLeft
            // 
            this.cubeLeft.Face = RubiksCubeSolver.Rubiks.CubeSide.Left;
            this.cubeLeft.Location = new System.Drawing.Point(236, 333);
            this.cubeLeft.Name = "cubeLeft";
            this.cubeLeft.NewColor = System.Drawing.Color.Empty;
            this.cubeLeft.Size = new System.Drawing.Size(218, 190);
            this.cubeLeft.TabIndex = 7;
            this.cubeLeft.Text = "colorGridDisplay1";
            this.cubeLeft.CellMouseClicked += new System.EventHandler<RubiksCubeSolver.Views.CellMouseClickedEventArgs>(this.cubeDisplay_CellMouseClicked);
            // 
            // cubeRight
            // 
            this.cubeRight.Face = RubiksCubeSolver.Rubiks.CubeSide.Right;
            this.cubeRight.Location = new System.Drawing.Point(236, 100);
            this.cubeRight.Name = "cubeRight";
            this.cubeRight.NewColor = System.Drawing.Color.Empty;
            this.cubeRight.Size = new System.Drawing.Size(218, 190);
            this.cubeRight.TabIndex = 5;
            this.cubeRight.Text = "colorGridDisplay1";
            this.cubeRight.CellMouseClicked += new System.EventHandler<RubiksCubeSolver.Views.CellMouseClickedEventArgs>(this.cubeDisplay_CellMouseClicked);
            // 
            // cubeBack
            // 
            this.cubeBack.Face = RubiksCubeSolver.Rubiks.CubeSide.Back;
            this.cubeBack.Location = new System.Drawing.Point(12, 333);
            this.cubeBack.Name = "cubeBack";
            this.cubeBack.NewColor = System.Drawing.Color.Empty;
            this.cubeBack.Size = new System.Drawing.Size(218, 190);
            this.cubeBack.TabIndex = 3;
            this.cubeBack.Text = "colorGridDisplay1";
            this.cubeBack.CellMouseClicked += new System.EventHandler<RubiksCubeSolver.Views.CellMouseClickedEventArgs>(this.cubeDisplay_CellMouseClicked);
            // 
            // cubeFront
            // 
            this.cubeFront.Face = RubiksCubeSolver.Rubiks.CubeSide.Front;
            this.cubeFront.Location = new System.Drawing.Point(12, 102);
            this.cubeFront.Name = "cubeFront";
            this.cubeFront.NewColor = System.Drawing.Color.Empty;
            this.cubeFront.Size = new System.Drawing.Size(218, 190);
            this.cubeFront.TabIndex = 0;
            this.cubeFront.Text = "colorGridDisplay1";
            this.cubeFront.CellMouseClicked += new System.EventHandler<RubiksCubeSolver.Views.CellMouseClickedEventArgs>(this.cubeDisplay_CellMouseClicked);
            // 
            // btnSolve
            // 
            this.btnSolve.Location = new System.Drawing.Point(559, 12);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(112, 37);
            this.btnSolve.TabIndex = 21;
            this.btnSolve.Text = "Solve...";
            this.btnSolve.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 638);
            this.Controls.Add(this.btnSolve);
            this.Controls.Add(this.chkLockColors);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtCommand);
            this.Controls.Add(this.btnRevert);
            this.Controls.Add(this.colorStrip);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cubeDown);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cubeUp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cubeLeft);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cubeRight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cubeBack);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cubeFront);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "Rubiks Cube Solver";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CubeFaceDisplay cubeFront;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private CubeFaceDisplay cubeBack;
        private System.Windows.Forms.Label label3;
        private CubeFaceDisplay cubeLeft;
        private System.Windows.Forms.Label label4;
        private CubeFaceDisplay cubeRight;
        private System.Windows.Forms.Label label5;
        private CubeFaceDisplay cubeDown;
        private System.Windows.Forms.Label label6;
        private CubeFaceDisplay cubeUp;
        private ColorStrip colorStrip;
        private System.Windows.Forms.Button btnRevert;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblErrorStatus;
        private System.Windows.Forms.CheckBox chkLockColors;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnSolve;



    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RubiksCubeSolver.Views
{
    public partial class SolveForm : Form
    {
        public SolveForm()
        {
            InitializeComponent();
        }
    }
}

namespace RubiksCubeSolver.Views
{
    partial class SolveForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox = new System.Windows.Forms.ListBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 405);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(879, 30);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(200, 24);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(41, 25);
            this.lblStatus.Text = "Idle";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Moves to solve:";
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.IntegralHeight = false;
            this.listBox.ItemHeight = 20;
            this.listBox.Location = new System.Drawing.Point(12, 52);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(855, 350);
            this.listBox.TabIndex = 3;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(792, 9);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 37);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(711, 9);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 37);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // SolveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 435);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip);
            this.Name = "SolveForm";
            this.Text = "Brute Force Solve";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
    }
}
