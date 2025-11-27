using System.Text.Json.Serialization;

namespace SharedService.SharedKernel;

public record PaginationEnvelope<T>
{
    public T[] Items { get; init; } = [];

    public long TotalCount { get; init; }

    [JsonConstructor]
    private PaginationEnvelope() { }

    public PaginationEnvelope(IEnumerable<T> items, long totalCount)
    {
        Items = [.. items];
        TotalCount = totalCount;
    }
}