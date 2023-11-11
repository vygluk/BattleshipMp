namespace BattleshipMpClient.Observer
{
    public abstract class ExtraRoundSubscriber
    {
        protected float ExtraRoundChance = 2.0f;

        protected int RoundsInARow = 0;

        protected abstract float ExtraRoundChanceMultiplier { get; }

        public void UpdateRoundsInARow()
        {
            RoundsInARow += 1;
        }

        public float GetExtraRoundChancePercentages() 
        {
            var chance = ExtraRoundChance + (RoundsInARow * ExtraRoundChanceMultiplier);
            return chance > 100.0f ? 100.0f : chance;
        }
    }
}