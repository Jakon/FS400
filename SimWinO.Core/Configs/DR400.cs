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
        public int FuelTankSelector => Convert.ToInt32(FuelTankSelectorFSValue);
        public decimal MixtureLevel => Convert.ToDecimal(MixtureLevelFSValue) * 100;
        public bool IsRightMagnetoOn => Convert.ToBoolean(RightMagnetoFSValue);
        public bool IsLeftMagnetoOn => Convert.ToBoolean(LeftMagnetoFSValue);
    }

    public class DR400Parser : ArduinoParser<DR400Struct>
    {
        protected override Dictionary<string, Action<ActionArgs<DR400Struct>>> Commands { get; } = new Dictionary<string, Action<ActionArgs<DR400Struct>>>
        {
            // Magneto Off
            { "000", args => args.FSHelper.SetMagnetoOff() },
            // { "001", args => ??? },
            // Battery Switch
            { "002", args => Toggle(args.SimState.IsElectricalMasterBatteryOn, args.Value, () => args.FSHelper.ChangeBatteryState())},
            // Alternator Switch
            { "003", args => Toggle(args.SimState.IsAlternatorOn, args.Value, () => args.FSHelper.ChangeAlternatorState())},
            // Magneto Both
            { "004", args => args.FSHelper.SetMagnetoBoth() },
            // Magneto Right
            { "014", args => args.FSHelper.SetMagnetoRight() },
            // Magneto Left
            { "015", args => args.FSHelper.SetMagnetoLeft() },
        };
    }
}
