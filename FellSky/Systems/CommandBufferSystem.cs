using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public enum ClientToServerMessageType: byte
    {
        MoveForward = 1,
        MoveBack = 2,
        MoveLeft = 3,
        MoveRight = 4,
        TurnLeft = 5,
        TurnRight = 6,
        ActivateSubsystem = 7,  // params: subsystem index, mouse world position
        Fire = 8,               // params: mouse world target position, weapon group index
        Chat = 9,               // params: chat string (utf-8)
        LogOn = 10,      
        LogOff = 11,                 // params: disconnect type
        SetTarget = 12,              // params: target id
        SendShipData = 13,           // params: ship data stream
    }

    public enum ServerToClientMessageType : byte
    {
        CreateEntity,           // object template id
        DestroyEntity,          // object id
        OtherClientCommand,     // other client send command (for prediction)
        UpdateTransform,        // pos, rot, linearVel, angularVel, linearAcceleration, angularAcceleration
    }

    public class CommandBufferSystem : Artemis.System.ProcessingSystem
    {
        public override void ProcessSystem()
        {
            
        }
    }
}
