using System.Windows.Forms;

namespace BattleshipMpClient.Strategy
{
    public interface IRadarStrategy
    {
        (bool shipFound, string message) ScanGrid(Button button);

        string InformationAboutRadarType();
    }
}
