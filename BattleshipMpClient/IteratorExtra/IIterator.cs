namespace BattleshipMpClient.IteratorExtra
{
    public interface IIterator<T>
    {
        bool HasNext();

        T Next();
    }
}
