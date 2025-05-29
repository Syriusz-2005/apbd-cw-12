namespace apbd_cw_12.DTOS;

public record TripsGetDto()
{
    public required int PageNum { get; init; }
    public required int PageSize { get; init; }
    public required int AllPages { get; init; }
    public required ICollection<TripGetDto> Trips { get; init; }   
}