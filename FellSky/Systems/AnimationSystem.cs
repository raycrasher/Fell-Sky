using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    class AnimationSystem
    {
        /*
            EVENT eventname
	            ROTATE partid degrees speed
	            TRANSLATE partid x y
	            START MOTOR partid
                STOP MOTOR partid
	            RESET POSITION partid speed
	            RESET ROTATION partid speed
	            COLOR partid speed r g b a
	            COLOR partid speed r g b
	            COLOR partid speed a
	            FIRE eventname
        */

        enum CommandType
        {
            Rotate, Translate, Lock, ResetPosition, ResetRotation, ColorRgba, ColorA, ColorRgb, FireEvent
        }

        class Script
        {
            public string EventId;
            public List<ScriptCommand> Commands = new List<ScriptCommand>();

            public Script(string evt)
            {
                EventId = evt;
            }
        }

        class ScriptCommand
        {
            public ScriptCommand(CommandType type, params object[] parameters)
            {
                Command = type;
                Parameters = parameters;
            }
            public CommandType Command;
            public object[] Parameters;
        }

        void Parse(string script)
        {
            var lines = script.Split('\n');

            Script currentScript = null;
            List<Script> scripts = new List<Script>();

            foreach (var line in lines)
            {
                var items = line.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                if (items.Length <= 0) continue;


                if (items[0].Equals("event", StringComparison.InvariantCultureIgnoreCase))
                {
                    currentScript = new Script(items[1]);
                    scripts.Add(currentScript);
                }
                else if (currentScript != null)
                {                
                    if (items[0].Equals("rotate", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentScript.Commands.Add(new ScriptCommand(CommandType.Rotate, items[1], float.Parse(items[2]), float.Parse(items[3])));
                    }
                    else if (items[0].Equals("translate", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentScript.Commands.Add(new ScriptCommand(CommandType.Translate, items[1], float.Parse(items[2]), float.Parse(items[3])));
                    }
                    else if (items[0].Equals("lock", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentScript.Commands.Add(new ScriptCommand(CommandType.Translate, items[1], float.Parse(items[2]), float.Parse(items[3])));
                    }
                    else if (items[0].Equals("reset", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentScript.Commands.Add(new ScriptCommand(CommandType.Translate, items[1], float.Parse(items[2]), float.Parse(items[3])));
                    }
                    else if (items[0].Equals("color", StringComparison.InvariantCultureIgnoreCase))
                    {
                        switch (items.Length)
                        {
                            case 4:
                                break;
                            case 6:
                                break;
                            case 7:
                                break;
                        }

                    }
                    else if (items[0].Equals("fire", StringComparison.InvariantCultureIgnoreCase))
                    {

                    }
                }
            }
        }
        
    }
}
