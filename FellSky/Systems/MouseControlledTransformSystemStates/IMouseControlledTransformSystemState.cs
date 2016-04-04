using Artemis;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems.MouseControlledTransformSystemStates
{
    public interface IMouseControlledTransformSystemState
    {
        void Transform(Vector2 worldMousePosition);
        void Apply();
        void Cancel();
        void Start(IEnumerable<Entity> entities, Vector2 worldMousePosition);

        float SnapAmount { get; set; }
        bool IsSnapEnabled { get; set; }
        Axis Constraint { get; set; }

    }
}
