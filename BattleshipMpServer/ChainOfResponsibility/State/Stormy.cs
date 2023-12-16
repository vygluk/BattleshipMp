namespace BattleshipMpServer.ChainOfResponsibility
{
    public class Stormy : IWeatherState
    {
        public float GetModifier() => 2;

        public BoostType GetModifierType() => BoostType.Damage;
    }
}