using System;
using System.Runtime.InteropServices;

namespace FS400.Utils
{
    /// <summary>
    /// Structure des données qu'on récupère pour le DR 400
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DR400Struc
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
}
