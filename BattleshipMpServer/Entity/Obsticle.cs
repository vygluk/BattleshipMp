using System.Collections.Generic;
using System.Windows.Forms;

namespace BattleshipMpServer.Entity
{
    public abstract class Obsticle
    {
        protected List<Control> obsticlePerButton { get; set; }

        public abstract Obsticle ShallowCopy();
        public abstract Obsticle DeepCopy();
    }
}
