namespace MovieWatch.Data.Common;

public interface IRecordable
{
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}