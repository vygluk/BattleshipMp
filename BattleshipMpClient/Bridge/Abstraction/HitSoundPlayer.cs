using System;
using System.IO;
using System.Windows.Forms;
using BattleshipMpClient.Bridge.Concrete;

namespace BattleshipMpClient.Bridge.Abstraction {
    public class HitSoundPlayer : SoundPlayerBridge
    {
        public HitSoundPlayer(ISoundImplementation soundImplementation) : base(soundImplementation)
        {
        }

        public override void Play()
        {
            soundImplementation.PlaySound();
        }

        public override void Stop()
        {
            soundImplementation.StopSound();
        }
    }
}