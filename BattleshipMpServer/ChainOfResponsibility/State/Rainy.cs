namespace BattleshipMpServer.ChainOfResponsibility
{
    public class Rainy : IWeatherState
    {
        public float GetModifier() => 2;

        public BoostType GetModifierType() => BoostType.ExtraRound;
    }
}
