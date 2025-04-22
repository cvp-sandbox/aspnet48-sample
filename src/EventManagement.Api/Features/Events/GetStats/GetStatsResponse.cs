namespace EventManagement.Api.Features.Events.GetStats;

public record StatItem(string StatLabel, int StatValue);

public record GetStatsResponse(IEnumerable<StatItem> Stats);
