using DynamicWallpaper.TreasureChest;
using Microsoft.Extensions.Logging;


namespace DynamicWallpaper
{


    internal class ProgressLog
    {
        private readonly ILogger<ProgressLog> _logger;

        public ProgressLog(ILogger<ProgressLog> logger) {
            _logger = logger;
            EventBus.Subscribe("Box.Ready", OnBoxReady);
            EventBus.Subscribe("Box.Open", OnBoxOpen);
            EventBus.Subscribe("Box.Load", OnBoxLoad);
            EventBus.Subscribe("Box.Fail", OnBoxFail);
            EventBus.Subscribe("Box.Lost", OnBoxLost);
            EventBus.Subscribe("Box.Exists", OnBoxExists);
            EventBus.Subscribe("Box.Success", OnBoxSuccess);
            EventBus.Subscribe("Box.Finish", OnBoxFinish);
            EventBus.Subscribe("Box.Random", OnBoxRandom);

            EventBus.Subscribe("SetLockScreenImageFailed", OnSetScreenFail);

            EventBus.Subscribe("Box.AutoOpen", OnBoxAutoOpen);

            EventBus.Subscribe("AutoRefresh", OnTimerRun);

            EventBus.Subscribe("DeleteOldFile", OnDeleteOldFiles);

        }

        private void OnSetScreenFail(CustomEventArgs args)
        {
            _logger.LogInformation("设置锁屏壁纸需要管理员权限！~~");
        }

        private void OnBoxAutoOpen(CustomEventArgs args)
        {
            _logger.LogInformation($"    {args.GetData<IBoxOptions>()?.TreasureChest?.Provider.ProviderName}家的宝箱快要被人抢走啦！~~~冲啊~~");
        }

        private void OnBoxRandom(CustomEventArgs arg)
        {
            _logger.LogInformation($"    带上({arg.GetData<int>()})个小伙伴，冲啊~~~");
        }

        private void LogInfo(CustomEventArgs arg, Func<INetworkProvider, string> msgAction)
        {
            var data = arg.GetData<INetworkProvider>();
            if (data != null)
            {
                _logger.LogInformation(msgAction(data));
            }
        }

        private void OnBoxFinish(CustomEventArgs arg)
        {
            LogInfo(arg, p => $"从{p.ProviderName}家里挖宝结束...");
        }

        private void OnDeleteOldFiles(CustomEventArgs obj)
        {
            _logger.LogInformation($"    家里的宝贝太多啦({obj.GetData<string>()})，清仓大甩卖啦~~~~");
        }

        private void OnTimerRun(CustomEventArgs arg)
        {
            _logger.LogInformation($"又到了更换新壁纸的时间啦!~~~~");
        }

        private void OnBoxReady(CustomEventArgs arg) {
            _logger.LogInformation($"听说{arg.GetData<IBoxOptions>()?.TreasureChest?.Provider.ProviderName}家有宝箱，我们去寻找吧 ~~~");
        }

        private void OnBoxOpen(CustomEventArgs arg) {
            LogInfo(arg, p => $"出发去{p.ProviderName}家喽~~~");
        }

        private void OnBoxLoad(CustomEventArgs arg) {
            LogInfo(arg, p => $"    在{p.ProviderName}家发现1个宝箱，搬回家喽 ~~~");
        }

        private void OnBoxFail(CustomEventArgs arg)
        {
            LogInfo(arg, p => $"    e.. 去往{p.ProviderName}家的路怎么被人挖断了 ~~~");
        }

        private void OnBoxLost(CustomEventArgs arg)
        {
            LogInfo(arg, p => $"    遇到江湖大盗，从{p.ProviderName}家挖出来的宝箱丢了 ~~~");
        }

        private void OnBoxExists(CustomEventArgs arg)
        {
            LogInfo(arg, p => $"    哇，{p.ProviderName}家宝箱挖出的宝贝，可惜我已经有了 ~~~");
        }

        private void OnBoxSuccess(CustomEventArgs arg)
        {
            LogInfo(arg, p => $"    哇，{p.ProviderName}家宝箱挖出1个宝贝，收藏了 ~~~");
        }
    }
}