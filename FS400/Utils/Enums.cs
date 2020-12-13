namespace FS400.Utils
{
    public enum SimEvents
    {
        TOGGLE_STARTER1, // Starter Engine 1
        MAGNETO1_START,
        TOGGLE_MASTER_IGNITION_SWITCH,
        TOGGLE_FUEL_VALVE_ENG1,
        TOGGLE_ALL_STARTERS, // Starter All Engines
        ENGINE_AUTO_START,
        TOGGLE_TAXI_LIGHTS, // Allume/Eteint les phares de roulage

        MAGNETO1_SET,
        TOGGLE_MASTER_BATTERY,
        TOGGLE_MASTER_ALTERNATOR
    }

    public enum GROUP
    {
        GROUP1
    }

    public enum DATA_REQUESTS
    {
        DR400Request,
        TaxiLightRequest,
        LandingLightRequest,
        EngineRequest
    }

    public enum DEFINITIONS
    {
        DR400Struct = 0
    }
}
