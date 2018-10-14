namespace Model.Interface
{
    public interface ITaskDTO : ICacheType
    {
        int Id { get; set; }
        string Name { get; set; }
        bool IsCompleted { get; set; }
    }
}
