using System.Windows.Forms;

namespace BattleshipMpServer.Strategy
{
    public interface IRadarStrategy
    {
        (bool shipFound, string message) ScanGrid(Button button);

        string InformationAboutRadarType();
    }
}