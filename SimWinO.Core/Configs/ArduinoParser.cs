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

        protected static void Toggle(bool condition, string value, Action action1, Action action2)
        {
            if (condition && value == "0")
                action1();
            else if (!condition && value == "1")
                action2();
        }

        protected static uint Percentage(int value, int arduinoMaxValue, int fsMaxValue)
        {
            var newValue = value * (decimal)fsMaxValue / arduinoMaxValue;
            return Convert.ToUInt32(newValue);
        }

        public static uint IsActive(string value)
        {
            return value == "0" ? (uint)0 : (uint)1;
        }

        public static void StartEngine(string value, bool isBatteryOn, bool isMagnetoNotOff, bool isFuelSelectorOn, bool isMixture, Action action1, Action action2)
        {
            if (value == "1")
            {
                if (isBatteryOn && isMagnetoNotOff && isFuelSelectorOn && isMixture)
                    action1();
                else
                    action2();
            }
        }

        public Action<ISimStruct, FlightSimulatorHelper> Parse(string input)
        {
            var type = input.Substring(0, 1); // On a une lettre
            var command = input.Substring(1, 3);
            var value = input.Substring(4).Replace("\r", "");

            if (!Commands.ContainsKey(command))
            {
                Debug.WriteLine($"Unknown Arduino command : {command} (full input : {input})");
                return null;
            }

            var action = Commands[command];

            return (state, fs) => action.Invoke(new ActionArgs<T>
            {
                SimState = (T)state,
                FSHelper = fs,
                Type = type,
                Command = command,
                Value = value,
            });
        }
    }
}
