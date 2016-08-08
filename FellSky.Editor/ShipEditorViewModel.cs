using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Microsoft.Xna.Framework.Content;
using Artemis;
using XnaColor = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework;
using FellSky.Framework;
using System.Windows;
using FellSky.Components;
using FellSky.Systems;
using FellSky.Services;
using FellSky.EntityFactories;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FellSky.Editor.Systems;
using FellSky.Systems.SceneGraphRenderers;
using FellSky.Game.Ships;

namespace FellSky.Editor
{
    public enum ColorPaletteSelection
    {
        Hull = -1, Base = -2, Trim = -3,
        Color0 = 0,
        Color1 = 1,
        Color2 = 2,
        Color3 = 3,
        Color4 = 4,
        Color5 = 5,
        Color6 = 6,
        Color7 = 7,
        Color8 = 8,
        Color9 = 9,
        Color10 = 10,
        Color11 = 11,
        Color12 = 12,
        Color13 = 13,
        Color14 = 14,
        Color15 = 15,
    }

    [PropertyChanged.ImplementPropertyChanged]
    public class ShipEditorViewModel: INotifyPropertyChanged
    {
        private D3D11Host _host;

        [PropertyChanged.ImplementPropertyChanged]
        public class SpriteSheet
        {
            public Framework.SpriteSheet SpriteDefinitions { get; set; }
            public BitmapImage Image { get; set; }
        }

        public Camera Camera { get; set; }

        public Dictionary<string, Sprite> Sprites { get; set; }
        public SpriteSheet CurrentSpriteSheet { get; set; }

        public Dictionary<string, List<Sprite>> HullSprites { get; set; }

        public XnaColor BackgroundColor { get; set; } = new XnaColor(5, 10, 20);
        public XnaColor GridColor { get; set; } = new XnaColor(30, 40, 50, 50);
        public System.Windows.Media.Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                ShipEditorService.Instance.SetPartColor(new XnaColor(value.R, value.G, value.B, value.A));
            }
        }

        public ColorPaletteSelection ColorPaletteSelection { get; set; }
        public ObservableCollection<XnaColor> ColorPalette { get; } = GetDefaultPalette();
        
        public ShipEditorService EditorService { get; private set; }

        public ContentManager Content { get; set; }
        public GameServiceContainer Services { get; set; }
        public EntityWorld World { get; set; }

        private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        //public List<Entity> SelectedPartEntities { get; set; }
        
        public SpriteBatch SpriteBatch { get; private set; }
        public bool IsSnap { get; set; }
        public bool IsGridVisible { get; set; }
        public int GridSize {
            get { return ((int?) GridEntity?.GetComponent<GridComponent>()?.GridSize.X) ?? 10;  }
            set
            {
                EditorService.SnapAmount = value;
                var grid = GridEntity?.GetComponent<GridComponent>();
                if (grid == null) return;
                grid.GridSize = new Vector2(value, value);
            }
        }
        
        public Entity CameraEntity { get; private set; }
        public Entity GridEntity { get; private set; }
        public int SelectedTabIndex { get; set; }

        private MouseService _mouse;

        private MouseControlledTransformSystem _transformSystem;
        private KeyboardService _keyboard;
        private System.Windows.Media.Color _selectedColor;

        public string ShipFileFilter = "Ship Model JSON files(*.json)|*.json|All files(*.*)|*.*";
        private TimerService _timer;

        public string WindowTitle
        {
            get
            {
                if (CurrentFilename != null)
                    return "ShipEditor: " + CurrentFilename;
                else
                    return "Ship Editor";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private List<Action> ActionsNextFrame { get; } = new List<Action>();

        internal void Initialize(D3D11Host host)
        {
            Services = new GameServiceContainer();

            ServiceLocator.Initialize(Services);
            _host = host;

            Artemis.System.EntitySystem.BlackBoard.SetEntry("GraphicsDevice", _host.GraphicsDevice);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("ServiceProvider", Services);

            Environment.CurrentDirectory = Path.GetFullPath(Properties.Settings.Default.DataFolder);

            Services.AddService<IGraphicsDeviceService>(host);
            Services.AddService(host.GraphicsDevice);

            _mouse = new MouseService(host);
            Services.AddService<IMouseService>(_mouse);
            _mouse.ButtonDown += OnMouseButtonDown;
            _keyboard = new KeyboardService(host);
            Services.AddService<IKeyboardService>(_keyboard);
            _timer = new TimerService();
            Services.AddService<ITimerService>(_timer);

            _timer.LastFrameUpdateTime = _gameTime;
            _timer.LastFrameRenderTime = _gameTime;

            SpriteBatch = new SpriteBatch(host.GraphicsDevice);
            Services.AddService(SpriteBatch);

            Content = new ContentManager(Services);
            Content.RootDirectory = Environment.CurrentDirectory;

            SpriteManager = new SpriteManagerService(Content);
            Services.AddService<ISpriteManagerService>(SpriteManager);

            LoadHullSprites("textures/hulls.json");

            Artemis.System.EntitySystem.BlackBoard.SetEntry("ContentManager", Content);

            World = new EntityWorld(false, false, false);

            World.SystemManager.SetSystem(new GridRendererSystem(), Artemis.Manager.GameLoopType.Draw, 1);
            //World.SystemManager.SetSystem(new ShipRendererSystem(), Artemis.Manager.GameLoopType.Draw, 2);
            World.SystemManager.SetSystem(new SceneGraphRendererSystem<StandardShipModelRenderer>(new StandardShipModelRenderer()), Artemis.Manager.GameLoopType.Draw, 3);
            World.SystemManager.SetSystem(new BoundingBoxRendererSystem(), Artemis.Manager.GameLoopType.Draw, 4);
            World.SystemManager.SetSystem(new GenericDrawableRendererSystem(), Artemis.Manager.GameLoopType.Draw, 5);
            var arcRendererSystem = new HardpointRendererSystem();
            World.SystemManager.SetSystem(arcRendererSystem, Artemis.Manager.GameLoopType.Draw, 6);

            World.SystemManager.SetSystem(new CameraControlSystem(), Artemis.Manager.GameLoopType.Update, 1);
            _transformSystem = new MouseControlledTransformSystem();
            World.SystemManager.SetSystem(_transformSystem, Artemis.Manager.GameLoopType.Update, 2);
            World.SystemManager.SetSystem(new ShipUpdateSystem(), Artemis.Manager.GameLoopType.Update, 3);
            World.SystemManager.SetSystem(new BoundingBoxSelectionSystem(), Artemis.Manager.GameLoopType.Update, 4);
            World.SystemManager.SetSystem(new ShowThrusterTrailsOverrideSystem(), Artemis.Manager.GameLoopType.Update, 5);

            World.InitializeAll();

            CameraEntity = World.CreateCamera(Constants.ActiveCameraTag, _host.GraphicsDevice);
            CameraEntity.Tag = Constants.ActiveCameraTag;
            Camera = CameraEntity.GetComponent<Camera>();

            GridEntity = World.CreateGrid(new Vector2(50, 50), GridColor);

            World.CreateCircle(Vector2.Zero, 10, 8, XnaColor.Red * 0.4f);

            host.PreviewKeyDown += HandleKeyboardInput;

            EditorService = new ShipEditorService(_mouse, World);
            CreateNewShipModelCommand.Execute(null);
            EditorService.SelectedPartEntities.CollectionChanged += (o, e) =>
            {
                if (e.NewItems != null && e.NewItems.Count > 0) {
                    var color = e.NewItems.Cast<Entity>().First().Components.OfType<IShipPartComponent>().First().Part.Color;
                    _selectedColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedColor)));
                }
            };

            PropertyChanged += (o, e) => {
                if (e.PropertyName == nameof(SelectedTabIndex))
                {
                    arcRendererSystem.IsEnabled = SelectedTabIndex == 2;
                }
            };
            arcRendererSystem.IsEnabled = false;

            _mouse.WheelChanged += OnWheelChanged;
        }

        private void OnWheelChanged(int delta)
        {
            if (_keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                EditorService.ChangePartDepth(Math.Sign(-delta));
            }
        }

        private void HandleKeyboardInput(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.Key)
            {
                case Key.R:
                    EditorService.RotateParts();
                    break;
                case Key.T:
                    EditorService.TranslateParts();
                    break;
                case Key.S:
                    EditorService.ScaleParts();
                    break;
                case Key.Delete:
                    EditorService.DeleteParts();
                    break;
                case Key.X:
                    if (EditorService.AxisConstraint != Axis.X)
                        EditorService.AxisConstraint = Axis.X;
                    else
                        EditorService.AxisConstraint = Axis.None;
                    break;
                case Key.Y:
                    if (EditorService.AxisConstraint != Axis.Y)
                        EditorService.AxisConstraint = Axis.Y;
                    else
                        EditorService.AxisConstraint = Axis.None;
                    break;
                case Key.F:
                    if(_keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                        EditorService.FlipLocal(SpriteEffects.FlipHorizontally);
                    else
                        EditorService.FlipGroup(SpriteEffects.FlipHorizontally);
                    break;
                case Key.V:
                    if (_keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                        EditorService.FlipLocal(SpriteEffects.FlipVertically);
                    else
                        EditorService.FlipGroup(SpriteEffects.FlipVertically);
                    break;
                case Key.C:
                    EditorService.CloneParts();
                    break;
                case Key.Up:
                    EditorService.OffsetParts(new Vector2(0, Keyboard.IsKeyDown(Key.LeftShift) ? -1 : -10));
                    break;
                case Key.Down:
                    EditorService.OffsetParts(new Vector2(0, Keyboard.IsKeyDown(Key.LeftShift) ? 1 : 10));
                    break;
                case Key.Left:
                    EditorService.OffsetParts(new Vector2(Keyboard.IsKeyDown(Key.LeftShift) ? -1 : -10, 0));
                    break;
                case Key.Right:
                    EditorService.OffsetParts(new Vector2(Keyboard.IsKeyDown(Key.LeftShift) ? 1 : 10, 0));
                    break;
                case Key.M:
                    EditorService.MirrorSelectedLaterally();
                    break;
                case Key.H:
                    EditorService.ToggleHardpointOnSelected();
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }
        
        private void OnMouseButtonDown(Microsoft.Xna.Framework.Point pos, int button)
        {
            //ActionsNextFrame.Add(EditorService.ClearSelection);
            if (button == 2) _transformSystem.CancelTransform();
            else if (button == 0) _transformSystem.ApplyTransform();
        }

        GameTime _gameTime = new GameTime();

        internal void Render(TimeSpan timespan)
        {
            _gameTime.ElapsedGameTime = timespan - _gameTime.TotalGameTime;
            _gameTime.TotalGameTime = timespan;
            _mouse.Update();

            foreach (var a in ActionsNextFrame) a();
            ActionsNextFrame.Clear();

            Camera.ScreenSize = new Vector2((float)_host.ActualWidth, (float)_host.ActualHeight);
            _host.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            _host.GraphicsDevice.Clear(BackgroundColor);


            
            _stopwatch.Start();
            World.Update();
            World.Draw();
            _stopwatch.Reset();
        }
              
        private void LoadHullSprites(string sheetfile)
        {
            var sheet = new SpriteSheet();
            sheet.SpriteDefinitions = SpriteManager.LoadSpriteSheet(sheetfile);
            CurrentSpriteSheet = sheet;
            HullSprites = CurrentSpriteSheet.SpriteDefinitions.Sprites
                .Where(s => s.Type == "hull")
                .GroupBy(s => s.Subtype)
                .OrderBy(s => s.Key)
                .ToDictionary(s => s.Key, s => s.ToList());
            sheet.Image = TextureToImage(Content.Load<Texture2D>(sheet.SpriteDefinitions.Texture));
            ThrusterSprites = CurrentSpriteSheet.SpriteDefinitions.Sprites
                .Where(s => s.Subtype == "thruster")
                .ToList();
        }

        private BitmapImage TextureToImage(Texture2D tex)
        {
            var image = new BitmapImage();
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

        private static ObservableCollection<XnaColor> GetDefaultPalette()
        {
            var items = Properties.Settings.Default.Palette.Split(',').Select(s => System.Drawing.ColorTranslator.FromHtml(s).ToXnaColor()).ToList();
            return new ObservableCollection<XnaColor>(items);
        }


        public ICommand CreateNewShipModelCommand => new DelegateCommand(o =>
        {
            EditorService.CreateNewModel();
            CurrentFilename = null;
        });

        public ICommand Quit => new DelegateCommand(o => Application.Current.Shutdown());
        public ICommand AddHull => new DelegateCommand(o =>
        {
            ActionsNextFrame.Add(() => {
                var pos = _host.PointToScreen(new System.Windows.Point(_host.ActualWidth / 2, _host.ActualHeight / 2));
                _mouse.ScreenPosition = new Vector2((float)pos.X, (float)pos.Y);
                System.Threading.Thread.Sleep(10);
                EditorService.AddHull((Sprite)o);
            });
        });

        public ICommand AddThruster => new DelegateCommand(o =>
        {
            ActionsNextFrame.Add(() =>
            {
                var pos = _host.PointToScreen(new System.Windows.Point(_host.ActualWidth / 2, _host.ActualHeight / 2));
                _mouse.ScreenPosition = new Vector2((float)pos.X, (float)pos.Y);
                System.Threading.Thread.Sleep(10);
                EditorService.AddThruster((Sprite)o);
            });
        });

        public ICommand AddDummyPart => new DelegateCommand(o =>
        {
            ActionsNextFrame.Add(() =>
            {
                var pos = _host.PointToScreen(new System.Windows.Point(_host.ActualWidth / 2, _host.ActualHeight / 2));
                _mouse.ScreenPosition = new Vector2((float)pos.X, (float)pos.Y);
                System.Threading.Thread.Sleep(10);
                EditorService.AddDummyPart();
            });
        });

        public ICommand DeletePartsCommand => new DelegateCommand(o => EditorService.DeleteParts());
        public ICommand MirrorLateralCommand => new DelegateCommand(o => EditorService.MirrorSelectedLaterally());
        public ICommand RotatePartsCommand => new DelegateCommand(o => EditorService.RotateParts());
        public ICommand SaveShipModelCommand => new DelegateCommand(o => {
            ActionsNextFrame.Add(() =>
            {
                var startDir = o?.ToString();
                if (startDir == null)
                {
                    startDir = Path.Combine(Content.RootDirectory, "ShipModels");
                    if (!Directory.Exists(startDir)) startDir = Content.RootDirectory;

                    var dialog = new Microsoft.Win32.SaveFileDialog
                    {
                        InitialDirectory = startDir,
                        AddExtension = true,
                        Filter = ShipFileFilter,
                        FileName = CurrentFilename,
                        DefaultExt = ".json"
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        EditorService.SaveShip(dialog.FileName);
                        CurrentFilename = dialog.FileName;
                    }
                        
                } else
                {
                    EditorService.SaveShip(CurrentFilename);
                }
            });
        });
        public ICommand LoadShipModelCommand => new DelegateCommand(o => {
            ActionsNextFrame.Add(() =>
            {
                var startDir = Path.Combine(Content.RootDirectory, o?.ToString() ?? "ShipModels");
                if (!Directory.Exists(startDir)) startDir = Content.RootDirectory;

                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    InitialDirectory = startDir,
                    AddExtension = true,
                    Filter = ShipFileFilter,
                    DefaultExt = ".json"
                };


                if (dialog.ShowDialog() == true)
                {
                    EditorService.LoadShipModel(dialog.FileName);
                    CurrentFilename = dialog.FileName;
                }
            });
        });

        public ICommand AddColorToPalette => new DelegateCommand(o => ColorPalette.Add(SelectedColor.ToXnaColor()));

        public ICommand DisassembleSpritesheet => new DelegateCommand(o => {

            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                foreach (var sprite in CurrentSpriteSheet.SpriteDefinitions.Sprites)
                {
                    var source = CurrentSpriteSheet.Image;
                    var bytesPerPixel = (source.Format.BitsPerPixel + 7) / 8;
                    var stride = sprite.W * bytesPerPixel;

                    var pixels = new byte[stride * sprite.H];
                    source.CopyPixels(new Int32Rect(sprite.X, sprite.Y, sprite.W, sprite.H), pixels, stride, 0);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(BitmapSource.Create(sprite.W, sprite.H, source.DpiX, source.DpiY, source.Format, source.Palette, pixels, stride)));
                    using (var outStream = new FileStream(Path.Combine(dialog.SelectedPath, $"{sprite.Id}.png"), FileMode.Create))
                        encoder.Save(outStream);
                }
            }
            if(MessageBox.Show($"Sprites exported to {dialog.SelectedPath}. Open folder?", "Export Spritesheet", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start(dialog.SelectedPath);
                }
                catch (Exception) { } // gulp
            }
        });

        public SpriteManagerService SpriteManager { get; private set; }
        public List<Sprite> ThrusterSprites { get; private set; }

        [PropertyChanged.AlsoNotifyFor(nameof(WindowTitle))]
        public string CurrentFilename { get; set; }
    }
}
