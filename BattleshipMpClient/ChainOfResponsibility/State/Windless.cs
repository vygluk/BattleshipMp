namespace BattleshipMpClient.ChainOfResponsibility
{
    public class Windless : IWeatherState
    {
        public float GetModifier() => 1;

        public BoostType GetModifierType() => BoostType.None;
    }
}