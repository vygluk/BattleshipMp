using BattleshipMp;
using BattleshipMpServer.Constants;
using BattleshipMpServer.Factory.Ship;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BattleshipMpServer.Strategy
{
    public class VerticalRadarStrategy : IRadarStrategy
    {
        public (bool shipFound, string message) ScanGrid(Button button)
        {
            var cellsToCheck = GetVerticalCells(button.Name);
            bool shipFound = DetectWithRadarIfShipFoundVertically(Form2_PreparatoryScreen.shipList, cellsToCheck)
                             || DetectWithRadarIfShipFoundVertically(Form2_PreparatoryScreen.specialShipList, cellsToCheck);

            string message = shipFound
                ? $"Vertically from position '{button.Name}' radar found ship"
                : $"Vertically from position '{button.Name}' radar did not find ship";

            return (shipFound, message);
        }

        private List<string> GetVerticalCells(string selectedCell)
        {
            List<string> verticalCells = new List<string>();
            char column = selectedCell[0];

            for (int row = 0; row < BoardSize.HEIGHT; row++)
            {
                string cell = $"{column}{row}";
                verticalCells.Add(cell);
            }

            return verticalCells;
        }

        private bool DetectWithRadarIfShipFoundVertically(List<IShip> ships, List<string> cellsToCheck)
        {
            foreach (var ship in ships)
            {
                foreach (var certainTypeShip in ship.shipPerButton)
                {
                    if (certainTypeShip.buttonNames.Exists(cellsToCheck.Contains))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool DetectWithRadarIfShipFoundVertically(List<ISpecialShip> ships, List<string> cellsToCheck)
        {
            foreach (var ship in ships)
            {
                foreach (var certainTypeShip in ship.shipPerButton)
                {
                    if (certainTypeShip.buttonNames.Exists(cellsToCheck.Contains))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public string InformationAboutRadarType()
        {
            return "Select button to scan vertically for enemy ships";
        }
    }
}
