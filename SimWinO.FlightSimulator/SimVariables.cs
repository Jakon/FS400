namespace SimWinO.FlightSimulator
{
    /// <summary>
    /// Liste des variables qu'on inscrit à récupérer dans Flight Simulator
    /// Elles sont utilisées telles quelles dans la méthode d'enregistrement des variables
    /// </summary>
    public static class SimVariables
    {
        public static readonly string[] Names =
        {
            "GENERAL ENG STARTER:1", // 001 INDEX PAR MOTEUR
            "ELECTRICAL MASTER BATTERY", // 002
            "GENERAL ENG MASTER ALTERNATOR:1", // 003 /!\ Y'A UN INDEX PAR MOTEUR
            // 004
            "LIGHT TAXI", // 005
            "LIGHT LANDING", // 006
            "LIGHT STROBE", // 007
            "LIGHT NAV", // 008
            "PITOT HEAT", // 009
            "GENERAL ENG FUEL PUMP SWITCH:1", // 010 /!\ Y'A UN INDEX PAR MOTEUR
            "GENERAL ENG ANTI ICE POSITION:1", // 011 /!\ Y'A UN INDEX PAR MOTEUR
            "RECIP ENG FUEL TANK SELECTOR:1", // 012 /!\ Y'A UN INDEX CHELOU (lol)
            "GENERAL ENG MIXTURE LEVER POSITION:1", //013 /!\ INDEX PAR MOTEUR
            "RECIP ENG RIGHT MAGNETO:1", // 014 (Magneto droite) /!\ Y'A UN INDEX PAR MOTEUR
            "RECIP ENG LEFT MAGNETO:1", // 015 (Magneto gauche) /!\ Y'A UN INDEX PAR MOTEUR
        };
    }
}
