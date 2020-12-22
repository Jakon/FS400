namespace SimWinO.FlightSimulator
{
    /// <summary>
    /// Liste des variables qu'on inscrit à récupérer dans Flight Simulator
    /// Elles sont utilisées telles quelles dans la méthode d'enregistrement des variables
    /// </summary>
    public static class SimVariables
    {
        public static readonly SimVariable[] Variables =
        {
            new SimVariable("GENERAL ENG STARTER:1", "Bool"), // 001 INDEX PAR MOTEUR
            new SimVariable("ELECTRICAL MASTER BATTERY", "Bool"), // 002
            new SimVariable("GENERAL ENG MASTER ALTERNATOR:1", "Bool"), // 003 /!\ Y'A UN INDEX PAR MOTEUR
            // 004
            new SimVariable("LIGHT TAXI", "Bool"), // 005
            new SimVariable("LIGHT LANDING", "Bool"), // 006
            new SimVariable("LIGHT STROBE", "Bool"), // 007
            new SimVariable("LIGHT NAV", "Bool"), // 008
            new SimVariable("PITOT HEAT", "Bool"), // 009
            new SimVariable("GENERAL ENG FUEL PUMP SWITCH:1", "Bool"), // 010 /!\ Y'A UN INDEX PAR MOTEUR
            new SimVariable("GENERAL ENG ANTI ICE POSITION:1", "Bool"), // 011 /!\ Y'A UN INDEX PAR MOTEUR
            new SimVariable("RECIP ENG FUEL TANK SELECTOR:1", "Enum"), // 012 /!\ Y'A UN INDEX CHELOU (lol)
            new SimVariable("GENERAL ENG MIXTURE LEVER POSITION:1", "Percent"), //013 /!\ INDEX PAR MOTEUR
            new SimVariable("RECIP ENG RIGHT MAGNETO:1", "Bool"), // 014 (Magneto droite) /!\ Y'A UN INDEX PAR MOTEUR
            new SimVariable("RECIP ENG LEFT MAGNETO:1", "Bool"), // 015 (Magneto gauche) /!\ Y'A UN INDEX PAR MOTEUR
            
            new SimVariable("PLANE LATITUDE", "Degrees"), // Latitude de l'avion
            new SimVariable("PLANE LONGITUDE", "Degrees"), // Longitude de l'avion
            new SimVariable("GROUND VELOCITY", "knots"), // Longitude de l'avion
        };
    }

    public class SimVariable
    {
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }

        public SimVariable(string name, string type)
        {
            PropertyName = name;
            PropertyType = type;
        }
    }
}
