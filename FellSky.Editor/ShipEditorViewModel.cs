using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using FellSky.Graphics;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Input;
using FellSky.Mechanics.Ships;
using Microsoft.Xna.Framework.Content;
using Artemis;
//using Microsoft.Xna.Framework;
using XnaColor = Microsoft.Xna.Framework.Color;
using WpfColor = System.Drawing.Color;
using Microsoft.Xna.Framework;
using FellSky.EntityComponents;
using System.Runtime.InteropServices;

namespace FellSky.Editor
{
    public enum EditMode
    {
        Move, Rotate
    }

    [PropertyChanged.ImplementPropertyChanged]
    public class ShipEditorViewModel
    {
        private D3D11Host _host;

        [PropertyChanged.ImplementPropertyChanged]
        public class SpriteSheet
        {
            public SpriteManager.JsonSpriteSheet SpriteDefinitions { get; set; }
            public System.Windows.Media.Imaging.BitmapImage Image { get; set; }
        }



        public Camera2D Camera { get; set; } = new Camera2D();
        
        public Dictionary<string, Graphics.Sprite> Sprites { get; set; }
        public SpriteSheet CurrentSpriteSheet { get; set; }

        public Dictionary<string, SpriteManager.JsonSprite[]> HullSprites { get; set; }

        public WpfColor DefaultColor { get; set; } = WpfColor.White;
        public WpfColor TrimColor { get; set; } = WpfColor.CornflowerBlue;
        public WpfColor BaseColor { get; set; } = WpfColor.Gold;
        public XnaColor BackgroundColor { get; set; } = new XnaColor(0,20,40);

        public Ship Ship { get; set; }
        public ContentManager Content { get; set; }
        public Microsoft.Xna.Framework.GameServiceContainer Services { get; } = new Microsoft.Xna.Framework.GameServiceContainer();
        public EntityWorld World { get; set; }

        public List<Entity> ControlledEntities { get; } = new List<Entity>();

        MouseService _mouse;

        internal void Initialize(D3D11Host host)
        {
            _host = host;
            _mouse = new MouseService(host);

            _mouse.ButtonUp += OnMouseButtonUp;

            Artemis.System.EntitySystem.BlackBoard.SetEntry("GraphicsDevice", _host.GraphicsDevice);
            Artemis.System.EntitySystem.BlackBoard.SetEntry(Camera2D.PlayerCameraName, Camera);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("ServiceProvider", Services);
           
            Environment.CurrentDirectory = Path.GetFullPath(Properties.Settings.Default.DataFolder);
            
            Services.AddService<IGraphicsDeviceService>(host);
            Services.AddService(host.GraphicsDevice);
            Services.AddService<Framework.IMouseService>(_mouse);

            Content = new ContentManager(Services);
            Content.RootDirectory = Environment.CurrentDirectory;
            LoadHullSprites("textures/hulls.json");

            Artemis.System.EntitySystem.BlackBoard.SetEntry("ContentManager", Content);

            World = new EntityWorld(false, true, false);
            World.InitializeAll(typeof(FellSky.Game).Assembly, GetType().Assembly);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("World", World);
            CameraEntity = World.CreateEntity();
            CameraEntity.AddComponent(Camera);
            CameraEntity.AddComponent(Camera.Transform);
            CameraEntity.Refresh();
            Camera.ScreenSize = new Vector2(_host.GraphicsDevice.DisplayMode.Width, _host.GraphicsDevice.DisplayMode.Height);
            CreateNewShip();
            
        }

        private void OnMouseButtonUp(Microsoft.Xna.Framework.Point arg1, int arg2)
        {
            ClearControlledEntities();
        }

        internal void Render(TimeSpan timespan)
        {
            _host.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            _host.GraphicsDevice.Clear(BackgroundColor);
            World.Update();
            World.Draw();
        }
              
        private void LoadHullSprites(string sheetfile)
        {
            var sheet = new SpriteSheet();
            sheet.SpriteDefinitions = Graphics.SpriteManager.AddSpriteSheetFromFile(Content, sheetfile);
            CurrentSpriteSheet = sheet;
            HullSprites = CurrentSpriteSheet.SpriteDefinitions.sprites
                .Where(s => s.type == "hull")
                .GroupBy(s => s.subtype)
                .ToDictionary(s => s.Key, s => s.ToArray());
            sheet.Image = TextureToImage(Content.Load<Texture2D>(sheet.SpriteDefinitions.texture));            
        }

        private BitmapImage TextureToImage(Texture2D tex)
        {
            var image = new System.Windows.Media.Imaging.BitmapImage();
            using(var stream=new MemoryStream())
            {
                tex.SaveAsPng(stream, tex.Width, tex.Height);
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
            }
            
            return image;
        }

        public ICommand AddHull { get { return new DelegateCommand(o => AddHullToShip((SpriteManager.JsonSprite)o)); } }

        public Entity ShipEntity { get; private set; }
        public Entity CameraEntity { get; private set; }

        private void AddHullToShip(SpriteManager.JsonSprite o)
        {
            var hull = new Hull(o.id, Vector2.Zero, 0, Vector2.One, new Vector2(o.origin_x, o.origin_y), XnaColor.White);
            Ship.Hulls.Add(hull);
            ClearControlledEntities();
            var entity = World.CreateEntity();
            entity.AddComponent(hull.Transform);
            entity.AddComponent(new MouseControlledTransformComponent());
            entity.Refresh();

            var pos = _host.PointToScreen(new System.Windows.Point(_host.ActualWidth / 2, _host.ActualHeight / 2));
            SetCursorPos((int)pos.X, (int)pos.Y);
            ControlledEntities.Add(entity);
        }

        private void ClearControlledEntities()
        {
            foreach (var entity in ControlledEntities)
                entity.Delete();
            ControlledEntities.Clear();
        }

        public void CreateNewShip()
        {
            Ship = new Ship();
            ShipEntity = World.CreateEntity();
            ShipEntity.AddComponent(Ship);
            ShipEntity.AddComponent(new FellSky.EntityComponents.ShipSpriteComponent(Ship));
            ShipEntity.AddComponent(new Transform());
            ShipEntity.Refresh();
            Artemis.System.EntitySystem.BlackBoard.SetEntry("PlayerShip", Ship);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("PlayerShipEntity", ShipEntity);

        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetCursorPos(int X, int Y);
    }

    public class SpriteToIntRectConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sprite = value as SpriteManager.JsonSprite;
            if (sprite == null) return null;
            return new System.Windows.Int32Rect(sprite.x, sprite.y, sprite.w, sprite.h);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
