using BattleshipMpClient.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMpClient.Adapter
{
    public class IcebergShipInteractionAdapter : IIcebergShipInteractionAdapter
    {
        public void ProcessIcebergShipCollision(Iceberg iceberg, List<(string, Color)> shipButtons, Form4_GameScreen screen, out bool isIceberg)
        {
            isIceberg = false;
            foreach (var icebergTile in iceberg.obsticlePerButton)
            {
                foreach (var shipButton in shipButtons)
                {
                    if (IsCollision(icebergTile, shipButton.Item1))
                    {
                        HandleCollision(icebergTile, shipButton.Item1, screen, out isIceberg);
                    }
                }

                if (isIceberg)
                {
                    break;
                }
            }
        }

        private bool IsCollision(Control icebergTile, string shipButton)
        {
            return icebergTile.Name == shipButton;
        }

        private void HandleCollision(Control icebergTile, string shipButton, Form4_GameScreen screen, out bool isIceberg)
        {
            isIceberg = true;
            var nameNumber = shipButton[1];
            var nameToSend = $"{shipButton}{nameNumber}";

            screen.AttackFromEnemy(nameToSend);
        }
    }
}
