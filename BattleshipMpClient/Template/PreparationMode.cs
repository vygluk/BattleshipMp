using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Template
{
    public abstract class PreparationMode
    {
        protected Form2_PreparatoryScreen _form;

        public PreparationMode(Form2_PreparatoryScreen form)
        {
            _form = form;
        }

        // Template method
        public void PrepareBoard()
        {
            SetObstacles();
            CreateShipList();
            RemainingShips();
            AdditionalPreparation();
        }

        protected abstract void AdditionalPreparation();

        private void SetObstacles()
        {
            _form.SetObsticlesUp();
        }

        private void CreateShipList()
        {
            _form.CreateShipList();
        }

        private void RemainingShips()
        {
            _form.RemainingShips();
        }
    }
}
