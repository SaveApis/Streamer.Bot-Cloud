using Hangfire.Storage;
using Newtonsoft.Json;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Events;

namespace Utils.Hangfire.Domain.Dtos;


public class RecurringEventSummaryDto
{
    public RecurringEventSummaryDto(string id) : this(id, false, string.Empty, HangfireQueue.Default, null, false, string.Empty, HangfireQueue.Default)
    {
    }

    [JsonConstructor]
    private RecurringEventSummaryDto(string id,
        bool isInCode,
        string codeCron,
        HangfireQueue codeQueue,
        IRecurringHangfireEvent? codeInstance,
        bool isInHangfire,
        string hangfireCron,
        HangfireQueue hangfireQueue)
    {
        Id = id;
        IsInCode = isInCode;
        CodeCron = codeCron;
        CodeQueue = codeQueue;
        CodeInstance = codeInstance;
        IsInHangfire = isInHangfire;
        HangfireCron = hangfireCron;
        HangfireQueue = hangfireQueue;
    }

    public string Id { get; }

    public bool IsInCode { get; private set; }
    public string CodeCron { get; private set; }
    public HangfireQueue CodeQueue { get; private set; }
    public IRecurringHangfireEvent? CodeInstance { get; private set; }

    public bool IsInHangfire { get; private set; }
    public string HangfireCron { get; private set; }
    public HangfireQueue HangfireQueue { get; private set; }

    public bool HasChanges => CodeCron != HangfireCron || CodeQueue != HangfireQueue;

    public void EnrichCode(KeyValuePair<IRecurringHangfireEvent, HangfireRecurringEventAttribute> pair)
    {
        IsInCode = true;
        CodeCron = pair.Value.Cron;
        CodeQueue = pair.Value.Queue;
        CodeInstance = pair.Key;
    }

    public void EnrichHangfire(RecurringJobDto dto)
    {
        IsInHangfire = true;
        HangfireCron = dto.Cron;
        HangfireQueue = Enum.TryParse<HangfireQueue>(dto.Queue, out var queue)
            ? queue
            : HangfireQueue.Default;
    }
}
