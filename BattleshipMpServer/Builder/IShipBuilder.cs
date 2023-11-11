using BattleshipMpServer.Factory.Ship;

namespace BattleshipMp.Builder
{
    public interface IShipBuilder
    {
        IShip CreateSubmarine();

        IShip CreateDestroyer();

        IShip CreateCruiser();

        IShip CreateBattleship();

        ISpecialShip CreateSpecialSubmarine();
    }
}
