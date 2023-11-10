using BattleshipMp;
using BattleshipMpServer.Constants;
using BattleshipMpServer.Factory.Ship;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BattleshipMpServer.Strategy
{
    public class DiagonalRadarStrategy : IRadarStrategy
    {
        public (bool shipFound, string message) ScanGrid(Button button)
        {
            var cellsToCheck = GetDiagonalCells(button.Name);
            bool shipFound = DetectWithRadarIfShipFoundDiagonally(Form2_PreparatoryScreen.shipList, cellsToCheck)
                             || DetectWithRadarIfShipFoundDiagonally(Form2_PreparatoryScreen.specialShipList, cellsToCheck);

            string message = shipFound
                ? $"Diagonally from position '{button.Name}' radar found ship"
                : $"Diagonally from position '{button.Name}' radar did not find ship";

            return (shipFound, message);
        }

        private List<string> GetDiagonalCells(string selectedCell)
        {
            List<string> diagonalCells = new List<string>();
            char column = selectedCell[0];
            int row = int.Parse(selectedCell.Substring(1));

            AddDiagonalCell(diagonalCells, (char)(column - 1), row - 1);
            AddDiagonalCell(diagonalCells, (char)(column + 1), row + 1);

            AddDiagonalCell(diagonalCells, (char)(column + 1), row - 1);
            AddDiagonalCell(diagonalCells, (char)(column - 1), row + 1);

            return diagonalCells;
        }

        private void AddDiagonalCell(List<string> cells, char column, int row)
        {
            if (column >= 'A' && column <= BoardSize.WIDTH_LETTER && row >= 0 && row < BoardSize.HEIGHT)
            {
                cells.Add($"{column}{row}");
            }
        }

        private bool DetectWithRadarIfShipFoundDiagonally(List<IShip> ships, List<string> cellsToCheck)
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

        private bool DetectWithRadarIfShipFoundDiagonally(List<ISpecialShip> ships, List<string> cellsToCheck)
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
            return "Select button to scan diagonally for enemy ships";
        }
    }
}