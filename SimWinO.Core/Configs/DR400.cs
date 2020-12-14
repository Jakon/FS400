using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SimWinO.Core.Configs
{
    /// <summary>
    /// Structure des données qu'on récupère pour le DR 400
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DR400Struct : ISimStruct
    {
        public double StarterFSValue { get; set; }

        public double ElectricalMasterBatteryFSValue { get; set; }

        public double AlternatorFSValue { get; set; }

        public double LightTaxiFSValue { get; set; }

        public double LandingLightFSValue { get; set; }

        public double StrobeLightFSValue { get; set; }

        public double NavLightFSValue { get; set; }

        public double PitotHeatFSValue { get; set; }

        public double FuelPumpFSValue { get; set; }

        public double AntiIceFSValue { get; set; }

        public double FuelTankSelectorFSValue { get; set; }

        public double MixtureLevelFSValue { get; set; }

        public double RightMagnetoFSValue { get; set; }

        public double LeftMagnetoFSValue { get; set; }


        // Ici on référence les valeurs mieux typées. CE sera plus simple à gérer
        public bool IsStarterOn => Convert.ToBoolean(StarterFSValue);
        public bool IsElectricalMasterBatteryOn => Convert.ToBoolean(ElectricalMasterBatteryFSValue);
        public bool IsAlternatorOn => Convert.ToBoolean(AlternatorFSValue);
        public bool IsLightTaxiOn => Convert.ToBoolean(LightTaxiFSValue);
        public bool IsLandingLightOn => Convert.ToBoolean(LandingLightFSValue);
        public bool IsStrobeLightOn => Convert.ToBoolean(StrobeLightFSValue);
        public bool IsNavLightOn => Convert.ToBoolean(NavLightFSValue);
        public bool IsPitotHeatOn => Convert.ToBoolean(PitotHeatFSValue);
        public bool IsFuelPumpOn => Convert.ToBoolean(FuelPumpFSValue);
        public bool IsAntiIceOn => Convert.ToBoolean(AntiIceFSValue);
        public bool FuelTankSelector => Convert.ToBoolean(FuelTankSelectorFSValue);
        public decimal MixtureLevel => Convert.ToDecimal(MixtureLevelFSValue) * 100;
        public bool IsRightMagnetoOn => Convert.ToBoolean(RightMagnetoFSValue);
        public bool IsLeftMagnetoOn => Convert.ToBoolean(LeftMagnetoFSValue);
        public bool IsOneMagnetoOn => IsLeftMagnetoOn || IsRightMagnetoOn;
    }

    public class DR400Parser : ArduinoParser<DR400Struct>
    {
        protected override Dictionary<string, Action<ActionArgs<DR400Struct>>> Commands { get; } = new Dictionary<string, Action<ActionArgs<DR400Struct>>>
        {
            // Magneto Off
            { "000", args => args.FSHelper.EngineCommands.SetMagnetoOff() },
            // Starter
            { "001", args => StartEngine(args.Value,
                args.SimState.IsElectricalMasterBatteryOn,
                args.SimState.IsOneMagnetoOn,
                args.SimState.FuelTankSelector,
                args.SimState.MixtureLevel > 0,
                () => args.FSHelper.EngineCommands.SetAutoStart(),
                () => args.FSHelper.EngineCommands.SetStarter1()) },
            // Battery Switch
            { "002", args => Toggle(args.SimState.IsElectricalMasterBatteryOn, args.Value, () => args.FSHelper.MiscSystemsCommands.ChangeBatteryState())},
            // Alternator Switch
            { "003", args => Toggle(args.SimState.IsAlternatorOn, args.Value, () => args.FSHelper.MiscSystemsCommands.ChangeAlternatorState())},
            // Magneto Both
            { "004", args => args.FSHelper.EngineCommands.SetMagnetoBoth() },
            // Taxi Light
            { "005", args => Toggle(args.SimState.IsLightTaxiOn, args.Value, () => args.FSHelper.LightsCommands.ChangeTaxiLightState())},
            // Landing Light
            { "006", args => Toggle(args.SimState.IsLandingLightOn, args.Value, () => args.FSHelper.LightsCommands.ChangeLandingLightState())},
            // Strobes Light
            { "007", args => Toggle(args.SimState.IsStrobeLightOn, args.Value, () => args.FSHelper.LightsCommands.ChangeStrobesLightState())},
            // Nav Light
            { "008", args => Toggle(args.SimState.IsNavLightOn, args.Value, () => args.FSHelper.LightsCommands.ChangeNavLightState())},
            // Pitot Heat
            { "009", args => Toggle(args.SimState.IsPitotHeatOn, args.Value, () => args.FSHelper.MiscSystemsCommands.ChangePitotHeatState())},
            // Fuel Pump
            { "010", args => Toggle(args.SimState.IsFuelPumpOn, args.Value, () => args.FSHelper.EngineCommands.ChangeFuelPumpState())},
            // Magneto Right
            { "011", args => args.FSHelper.EngineCommands.SetAntiIce1(IsActive(args.Value)) },
            // Fuel Selector
            { "012", args => Toggle(args.SimState.FuelTankSelector, args.Value, () => args.FSHelper.FuelSystemsCommands.SetFuel1SelectorOff(), () => args.FSHelper.FuelSystemsCommands.SetFuel1SelectorAll())},
            // Mixture Level
            { "013", args => args.FSHelper.EngineCommands.SetMixture1(Percentage(Convert.ToInt32(args.Value), 255, 16383)) },
            // Magneto Right
            { "014", args => args.FSHelper.EngineCommands.SetMagnetoRight() },
            // Magneto Left
            { "015", args => args.FSHelper.EngineCommands.SetMagnetoLeft() },
        };
    }
}
