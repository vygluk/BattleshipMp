namespace BattleshipMpServer.ChainOfResponsibility
{
    public class Windy : IWeatherState
    {
        public float GetModifier() => 1.25f;

        public BoostType GetModifierType() => BoostType.ExtraRound;
    }
}
