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
        }

        private void LogInfo(CustomEventArgs arg, Func<INetworkPaperProvider, string> msgAction)
        {
            var data = arg.GetData<INetworkPaperProvider>();
            if (data != null)
            {
                _logger.LogInformation(msgAction(data));
            }
        }

        private void OnBoxReady(CustomEventArgs arg) {
            LogInfo(arg, p => $"听说{p.ProviderName}家有{p.DefaultBox.Num}个宝箱，我们去寻找吧 ~~~");
        }

        private void OnBoxOpen(CustomEventArgs arg) {
            LogInfo(arg, p => $"出发去{p.ProviderName}家喽 ~~~");
        }

        private void OnBoxLoad(CustomEventArgs arg) {
            LogInfo(arg, p => $"在{p.ProviderName}家发现1个宝箱，搬回家喽 ~~~");
        }

        private void OnBoxFail(CustomEventArgs arg)
        {
            LogInfo(arg, p => $"e.. 去往{p.ProviderName}家的路怎么被人挖断了 ~~~");
        }

        private void OnBoxLost(CustomEventArgs arg)
        {
            LogInfo(arg, p => $"遇到江湖大盗，从{p.ProviderName}家挖出来的宝箱丢了 ~~~");
        }

        private void OnBoxExists(CustomEventArgs arg)
        {
            LogInfo(arg, p => $"哇，{p.ProviderName}家宝箱挖出1个宝贝，可是我已经有了 ~~~");
        }

        private void OnBoxSuccess(CustomEventArgs arg)
        {
            LogInfo(arg, p => $"哇，{p.ProviderName}家宝箱挖出1个宝贝，收藏了 ~~~");
        }
    }
}