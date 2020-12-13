using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimWinO.FlightSimulator;

namespace SimWinO.Core.Configs
{
    public class ActionArgs<T>
    {
        public T SimState { get; set; }
        public FlightSimulatorHelper FSHelper { get; set; }
        public string Type { get; set; }
        public string Command { get; set; }
        public string Value { get; set; }
    }

    public interface IArduinoParser
    {
        Action<ISimStruct, FlightSimulatorHelper> Parse(string input);
    }

    public abstract class ArduinoParser<T> : IArduinoParser where T : ISimStruct
    {
        protected abstract Dictionary<string, Action<ActionArgs<T>>> Commands { get; }

        protected static void Toggle(bool condition, string value, Action action)
        {
            if (condition && value == "0")
                action();
            else if (!condition && value == "1")
                action();
        }

        public Action<ISimStruct, FlightSimulatorHelper> Parse(string input)
        {
            var type = input.Substring(0, 1); // On a une lettre
            var command = input.Substring(1, 3);
            var value = input.Substring(4).Replace("\r","");

            if (!Commands.ContainsKey(command))
            {
                Debug.WriteLine($"Unknown Arduino command : {command} (full input : {input})");
                return null;
            }
                
            var action = Commands[command];

            return (state, fs) => action.Invoke(new ActionArgs<T>
            {
                SimState = (T) state,
                FSHelper = fs,
                Type = type,
                Command = command,
                Value = value,
            });
        }
    }
}
