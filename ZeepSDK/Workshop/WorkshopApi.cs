using JetBrains.Annotations;
using Steamworks.Ugc;
using ZeepSDK.External.Cysharp.Threading.Tasks;
using ZeepSDK.External.FluentResults;

namespace ZeepSDK.Workshop;

/// <summary>
/// An API for interacting with the workshop
/// </summary>
[PublicAPI]
public static class WorkshopApi
{
    /// <summary>
    /// Attempts to subscribe to a workshop item
    /// </summary>
    /// <param name="workshopId">The id of the workshop item to subscribe to</param>
    /// <returns>Ok if successfully subscribed, Fail for multiple failure reasons</returns>
    public static async UniTask<Result> SubscribeAsync(ulong workshopId)
    {
        Item? item = await Item.GetAsync(workshopId);
        if (!item.HasValue)
            return Result.Fail("Unable to get workshop item");

        if (item.Value.IsSubscribed)
            return Result.Ok();

        bool subscribed = await item.Value.Subscribe();

        if (!subscribed)
            return Result.Fail("Unable to subscribe to workshop item");

        return Result.OkIf(
            await WorkshopManager.Instance.DownloadWorkshopLevel(item.Value.Id),
            "Unable to download workshop level");
    }
}
