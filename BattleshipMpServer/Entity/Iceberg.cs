using BattleshipMpServer.Factory.Ship;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BattleshipMpServer.Entity
{
    public class Iceberg : Obsticle
    {
        public Iceberg()
        {
            this.obsticlePerButton = new List<Control>();
        }

        public Iceberg(List<Control> obsticleButtons)
        {
            this.obsticlePerButton = obsticleButtons;
        }

        public void ReplaceTiles(List<ShipButtons> obsticleButtons)
        {
            this.obsticlePerButton = obsticlePerButton;
        }

        public void AddTiles(Control obsticleButtons)
        {
            this.obsticlePerButton.Add(obsticleButtons);
        }

        public override Obsticle ShallowCopy()
        {
            return (Obsticle)this.MemberwiseClone();
        }

        public override Obsticle DeepCopy()
        {
            Iceberg newIceberg = (Iceberg)this.MemberwiseClone();
            newIceberg.obsticlePerButton = new List<Control>(this.obsticlePerButton);
            return newIceberg;
        }
    }
}
