namespace Model.Interface
{
    public interface IGetTasksDTO
    {
        int Id { get; set; }
        int Skip { get; set; }
        int Limit { get; set; }
    }
}
