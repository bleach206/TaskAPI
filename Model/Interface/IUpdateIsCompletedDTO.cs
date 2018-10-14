namespace Model.Interface
{
    public interface IUpdateIsCompletedDTO
    {
        int UserId { get; set; }
        bool IsCompleted { get; set; }
    }
}
