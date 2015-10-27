using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace FellSky.Editor
{
    public class ShipEditorRenderer: Microsoft.Xna.Framework.Game
    {
        private IntPtr _deviceWindowHandle;

        public GraphicsDeviceManager GraphicsManager { get; private set; }
        public Point Size { get; set; }
        public volatile bool QuitFlag = false;
        private bool _hidePhantomWindow=false;
        private System.Windows.Forms.Control _host;
        private volatile bool _hasResized=false;

        public ShipEditorRenderer(System.Windows.Forms.Control host)
        {
            _host = host;
            GraphicsManager = new GraphicsDeviceManager(this);
            _deviceWindowHandle = host.Handle;
            GraphicsManager.PreparingDeviceSettings += PrepareDeviceSettings;
            GraphicsManager.IsFullScreen = false;
            GraphicsManager.PreferredDepthStencilFormat = Microsoft.Xna.Framework.Graphics.DepthFormat.None;
            ApplyGraphicsChanges();
            this.IsMouseVisible = true;
            host.Resize += Host_Resize;
        }

        private void Host_Resize(object sender, EventArgs e)
        {
            _hasResized = true;
        }

        public void ApplyGraphicsChanges()
        {
            Size = new Point(_host.Width, _host.Height);
            GraphicsManager.PreferredBackBufferWidth = Size.X;
            GraphicsManager.PreferredBackBufferHeight = Size.Y;
            GraphicsManager.ApplyChanges();
            _hasResized = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if(!_hidePhantomWindow) HidePhantomWindow();
            if (_hasResized) ApplyGraphicsChanges();

            if (QuitFlag) Exit();
            base.Update(gameTime);
        }

        private void HidePhantomWindow()
        {
            var form = (System.Windows.Forms.Form) System.Windows.Forms.Form.FromHandle(Window.Handle);
            form.Opacity = 0;
            form.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            form.ShowInTaskbar = false;
            _hidePhantomWindow = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        private void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = _deviceWindowHandle;
        }
    }
}
