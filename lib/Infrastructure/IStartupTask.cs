namespace LIB.Infrastructure
{
    public interface IStartupTask 
    {
        void Execute();

        int Order { get; }
    }
}
