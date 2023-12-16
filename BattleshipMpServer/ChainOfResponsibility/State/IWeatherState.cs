namespace BattleshipMpServer.ChainOfResponsibility
{
    public interface IWeatherState
    {
        float GetModifier();

        BoostType GetModifierType();
    }
}