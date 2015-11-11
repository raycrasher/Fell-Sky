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
using WpfColor = System.Windows.Media.Color;
using Microsoft.Xna.Framework;
using FellSky.EntityComponents;
using System.Runtime.InteropServices;
using FellSky.Systems;

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
            public JsonSpriteSheet SpriteDefinitions { get; set; }
            public System.Windows.Media.Imaging.BitmapImage Image { get; set; }
        }

        public Camera2D Camera { get; set; }
        
        public Dictionary<string, Graphics.Sprite> Sprites { get; set; }
        public SpriteSheet CurrentSpriteSheet { get; set; }

        public Dictionary<string, List<JsonSprite>> HullSprites { get; set; }

        public XnaColor DefaultColor { get; set; } = XnaColor.White;
        public XnaColor TrimColor { get; set; } = XnaColor.CornflowerBlue;
        public XnaColor BaseColor { get; set; } = XnaColor.Gold;
        public XnaColor BackgroundColor { get; set; } = new XnaColor(5,10,20);
        public XnaColor GridColor { get; set; } = new XnaColor(30, 40, 50);

        public Ship Ship { get; set; }
        public ContentManager Content { get; set; }
        public Microsoft.Xna.Framework.GameServiceContainer Services { get; set; }
        public EntityWorld World { get; set; }

        public List<Entity> SelectedPartEntities { get; set; }
        public SpriteBatch SpriteBatch { get; private set; }

        private MouseService _mouse;
        private MouseControlledTransformSystem _transformSystem;

        internal void Initialize(D3D11Host host)
        {
            SelectedPartEntities = new List<Entity>();
            Services = new Microsoft.Xna.Framework.GameServiceContainer();
            _host = host;
            _mouse = new MouseService(host);

            _mouse.ButtonUp += OnMouseButtonUp;

            Artemis.System.EntitySystem.BlackBoard.SetEntry("GraphicsDevice", _host.GraphicsDevice);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("ServiceProvider", Services);
           
            Environment.CurrentDirectory = Path.GetFullPath(Properties.Settings.Default.DataFolder);
            
            Services.AddService<IGraphicsDeviceService>(host);
            Services.AddService(host.GraphicsDevice);
            Services.AddService<Framework.IMouseService>(_mouse);
            SpriteBatch = new SpriteBatch(host.GraphicsDevice);
            Services.AddService(SpriteBatch);

            Content = new ContentManager(Services);
            Content.RootDirectory = Environment.CurrentDirectory;
            LoadHullSprites("textures/hulls.json");

            Artemis.System.EntitySystem.BlackBoard.SetEntry("ContentManager", Content);

            World = new EntityWorld(false, true, false);
            World.InitializeAll(typeof(FellSky.Game).Assembly, GetType().Assembly);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("World", World);

            CameraEntity = World.CreateEntityFromTemplate("Camera");
            Camera = CameraEntity.GetComponent<Camera2D>();

            Artemis.System.EntitySystem.BlackBoard.SetEntry(Camera2D.PlayerCameraName, Camera);
            GridEntity = World.CreateEntityFromTemplate("Grid", new Vector2(50,50), GridColor);

            World.CreateEntityFromTemplate("GenericDrawable", EntityTemplates.GenericDrawableTemplate.Circle(Vector2.Zero, 10, 10, XnaColor.Red));

            CreateNewShip();            

            _transformSystem = World.SystemManager.GetSystem<Systems.MouseControlledTransformSystem>();
            host.KeyUp += HandleKeyboardInput;
        }

        private void HandleKeyboardInput(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.R:
                    _transformSystem.Mode = MouseControlledTransformMode.Rotate;
                    break;
                case Key.T:
                    _transformSystem.Mode = MouseControlledTransformMode.Translate;
                    break;
                case Key.S:
                    _transformSystem.Mode = MouseControlledTransformMode.Scale;
                    break;
                case Key.Delete:
                    DeleteSelectedParts();
                    break;
                case Key.Up:
                    TranslateSelectedParts(new Vector2(0, Keyboard.IsKeyDown(Key.LeftShift) ? -1 : -10));
                    break;
                case Key.Down:
                    TranslateSelectedParts(new Vector2(0, Keyboard.IsKeyDown(Key.LeftShift) ? 1 : 10));
                    break;
                case Key.Left:
                    TranslateSelectedParts(new Vector2(Keyboard.IsKeyDown(Key.LeftShift) ? -1 : -10, 0));
                    break;
                case Key.Right:
                    TranslateSelectedParts(new Vector2(Keyboard.IsKeyDown(Key.LeftShift) ? 1 : 10, 0));
                    break;
            }
        }

        private void TranslateSelectedParts(Vector2 offset)
        {
            foreach (var xform in SelectedPartEntities.Select(e => e.GetComponent<Transform>()).Where(t => t != null))
            {
                xform.Position += offset;
            }
        }

        private void DeleteSelectedParts()
        {
            foreach(var part in SelectedPartEntities.Select(e => e.GetComponent<ShipPart>()))
            {
                Ship.RemovePart(part);
            }
        }

        private void OnMouseButtonUp(Microsoft.Xna.Framework.Point arg1, int arg2)
        {
            ClearControlledEntities();
            _transformSystem.Mode = null;
        }

        internal void Render(TimeSpan timespan)
        {
            Camera.ScreenSize = new Vector2((float)_host.ActualWidth, (float)_host.ActualHeight);
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
            HullSprites = CurrentSpriteSheet.SpriteDefinitions.Sprites
                .Where(s => s.Type == "hull")
                .GroupBy(s => s.Subtype)
                .ToDictionary(s => s.Key, s => s.ToList());
            sheet.Image = TextureToImage(Content.Load<Texture2D>(sheet.SpriteDefinitions.Texture));            
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

        public ICommand AddHull { get { return new DelegateCommand(o => AddHullToShip((JsonSprite)o)); } }

        public Entity ShipEntity { get; private set; }
        public Entity CameraEntity { get; private set; }
        public Entity GridEntity { get; private set; }

        private void AddHullToShip(JsonSprite o)
        {
            var pos = _host.PointToScreen(new System.Windows.Point(_host.ActualWidth / 2, _host.ActualHeight / 2));
            _mouse.ScreenPosition = new Vector2((float)pos.X, (float)pos.Y);
            _transformSystem.Mode = MouseControlledTransformMode.Translate;
            _transformSystem.Origin = Vector2.Zero;
            var hull = new Hull(o.Id, Vector2.Zero, 0, Vector2.One, new Vector2(o.OriginX ?? o.W/2, o.OriginY ?? o.H/2), XnaColor.White);
            Ship.Hulls.Add(hull);
            ClearControlledEntities();
            var entity = World.CreateEntity();
            entity.AddComponent(hull);
            entity.AddComponent(hull.Transform);
            entity.AddComponent(new MouseControlledTransformComponent());
            entity.AddComponent(new ChildEntityComponent(ShipEntity));
            entity.AddComponent(new DrawBoundingBoxComponent(hull.BoundingBox));
            entity.Refresh();
            SelectedPartEntities.Add(entity);
        }

        private void ClearControlledEntities()
        {
            foreach (var entity in SelectedPartEntities)
                entity.Delete();
            SelectedPartEntities.Clear();
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


    }

    public class SpriteToIntRectConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sprite = value as JsonSprite;
            if (sprite == null) return null;
            return new System.Windows.Int32Rect(sprite.X, sprite.Y, sprite.W, sprite.H);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class XnaColorToBrushConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is XnaColor)) return null;
            var color = (XnaColor)value;
            return new System.Windows.Media.SolidColorBrush(WpfColor.FromArgb(color.A,color.R,color.G,color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as System.Windows.Media.SolidColorBrush;
            if (brush == null) return null;
            return new XnaColor(brush.Color.R,brush.Color.G,brush.Color.B,brush.Color.A);
        }
    }
}
