using BattleshipMpClient.Constants;
using BattleshipMpClient.Factory.Ship;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BattleshipMpClient.Strategy
{
    public class VerticalHorizontalRadarStrategy : IRadarStrategy
    {
        public (bool shipFound, string message) ScanGrid(Button button)
        {
            var verticalCells = GetVerticalCells(button.Name);
            var horizontalCells = GetHorizontalCells(button.Name);

            var cellsToCheck = new List<string>(verticalCells);
            cellsToCheck.AddRange(horizontalCells);

            bool shipFound = DetectWithRadarIfShipFound(Form2_PreparatoryScreen.shipList, cellsToCheck)
                             || DetectWithRadarIfShipFound(Form2_PreparatoryScreen.specialShipList, cellsToCheck);

            string message = shipFound
                ? $"Vertically and horizontally from position '{button.Name}' radar found ship"
                : $"Vertically and horizontally from position '{button.Name}' radar did not find ship";

            return (shipFound, message);
        }

        private List<string> GetVerticalCells(string selectedCell)
        {
            List<string> verticalCells = new List<string>();
            char column = selectedCell[0];

            for (int row = 0; row < BoardSize.HEIGHT; row++)
            {
                verticalCells.Add($"{column}{row}");
            }

            return verticalCells;
        }

        private List<string> GetHorizontalCells(string selectedCell)
        {
            List<string> horizontalCells = new List<string>();
            int row = int.Parse(selectedCell.Substring(1));

            for (char column = 'A'; column <= BoardSize.WIDTH_LETTER; column++)
            {
                horizontalCells.Add($"{column}{row}");
            }

            return horizontalCells;
        }

        private bool DetectWithRadarIfShipFound(List<IShip> ships, List<string> cellsToCheck)
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

        private bool DetectWithRadarIfShipFound(List<ISpecialShip> ships, List<string> cellsToCheck)
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
            return "Select button to scan vertically and horizontally for enemy ships";
        }
    }
}