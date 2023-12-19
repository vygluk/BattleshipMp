using BattleshipMpClient.IteratorExtra;

namespace BattleshipMp.IteratorExtra
{
    public interface IAggregate<T>
    {
        IIterator<T> CreateIterator();
    }
}