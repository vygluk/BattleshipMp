namespace BattleshipMpClient.ChainOfResponsibility
{
    public interface IWeatherState
    {
        float GetModifier();

        BoostType GetModifierType();
    }
}