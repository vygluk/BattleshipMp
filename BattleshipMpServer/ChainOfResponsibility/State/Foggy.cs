namespace BattleshipMpServer.ChainOfResponsibility
{
    public class Foggy : IWeatherState
    {
        public float GetModifier() => 1.5f;

        public BoostType GetModifierType() => BoostType.ExtraRound;
    }
}