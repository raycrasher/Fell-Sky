using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace FellSky.Editor
{
    public class ShipEditorViewModel
    {
        public ShipEditorRenderer Renderer { get; set; }
        public Thread RendererThread { get; private set; }

        public void InitializeRenderer(System.Windows.Forms.Control host)
        {
            Renderer = new ShipEditorRenderer(host);
            RendererThread = new Thread(Renderer.Run);
            RendererThread.Start();
        }
    }
}
