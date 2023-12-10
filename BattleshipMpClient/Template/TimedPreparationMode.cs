using System;
using System.Windows.Forms;

namespace BattleshipMpClient.Template
{
    public class TimedPreparationMode : PreparationMode
    {
        private System.Windows.Forms.Timer _timer;
        private int _timeLimitInSeconds;

        public TimedPreparationMode(Form2_PreparatoryScreen form, int timeLimitInSeconds) : base(form)
        {
            _timeLimitInSeconds = timeLimitInSeconds;
            AdditionalPreparation();
        }

        protected override void AdditionalPreparation()
        {
            // checks if the timer is already created and running
            if (_timer == null)
            {
                _timer = new System.Windows.Forms.Timer();
                _timer.Interval = 1000;
                _timer.Tick += OnTimeElapsed;
                _timer.Start();
            }
            else if (!_timer.Enabled)
            {
                // if the timer exists but is not running it it starts
                _timer.Start();
            }
        }

        private void OnTimeElapsed(object sender, EventArgs e)
        {
            _timeLimitInSeconds--;

            if (_timeLimitInSeconds <= 0)
            {
                _timer.Stop();
                _form.EndPreparation();
            }
            else
            {
                _form.UpdateCountdownDisplay(_timeLimitInSeconds);
            }
        }
    }
}
