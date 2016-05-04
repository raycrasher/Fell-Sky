using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibRocketNet;
using Artemis.Interface;

namespace FellSky.Components
{
    public class GuiComponent: IComponent
    {
        public GuiComponent(Element element)
        {
            Element = element;
        }
        public readonly Element Element;
    }
}
