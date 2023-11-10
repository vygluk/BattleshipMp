using BattleshipMpClient.Constants;
using BattleshipMpClient.Factory.Ship;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BattleshipMpClient.Strategy
{
    public class HorizontalRadarStrategy : IRadarStrategy
    {
        public (bool shipFound, string message) ScanGrid(Button button)
        {
            var cellsToCheck = GetHorizontalCells(button.Name);
            bool shipFound = DetectWithRadarIfShipFoundHorizontally(Form2_PreparatoryScreen.shipList, cellsToCheck)
                             || DetectWithRadarIfShipFoundHorizontally(Form2_PreparatoryScreen.specialShipList, cellsToCheck);

            string message = shipFound
                ? $"Horizontally from position '{button.Name}' radar found ship"
                : $"Horizontally from position '{button.Name}' radar did not find ship";

            return (shipFound, message);
        }

        private List<string> GetHorizontalCells(string selectedCell)
        {
            List<string> horizontalCells = new List<string>();
            int row = int.Parse(selectedCell.Substring(1));

            for (char column = 'A'; column <= BoardSize.WIDTH_LETTER; column++)
            {
                string cell = $"{column}{row}";
                horizontalCells.Add(cell);
            }

            return horizontalCells;
        }

        private bool DetectWithRadarIfShipFoundHorizontally(List<IShip> ships, List<string> cellsToCheck)
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

        private bool DetectWithRadarIfShipFoundHorizontally(List<ISpecialShip> ships, List<string> cellsToCheck)
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
            return "Select button to scan horizontally for enemy ships";
        }
    }
}