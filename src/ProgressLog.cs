using Microsoft.Extensions.Logging;

namespace DynamicWallpaper
{
    internal class ProgressLog
    {

        public ProgressLog(ILogger<ProgressLog> logger) {

            EventBus.Subscribe("Box.Open", arg => {
                var data = arg.GetData<INetworkPaperProvider>();
                if (data != null)
                {
                    logger.LogInformation($"听说{data.ProviderName}家有{data.DefaultBox.Num}个宝箱，我们去寻找吧 ~~~");
                }
            });

            EventBus.Subscribe("Box.Load", arg => {
                var data = arg.GetData<INetworkPaperProvider>();
                if (data != null)
                {
                    logger.LogInformation($"在{data.ProviderName}家发现{data.DefaultBox.Num}个宝箱，搬回家喽 ~~~");
                }
            });

            EventBus.Subscribe("Box.Fail", arg => {
                var data = arg.GetData<INetworkPaperProvider>();
                if (data != null)
                {
                    logger.LogInformation($"e.. 去往{data.ProviderName}家的路怎么被人挖断了 ~~~");
                }
            });

            EventBus.Subscribe("Box.Lost", arg => {
                var data = arg.GetData<INetworkPaperProvider>();
                if (data != null)
                {
                    logger.LogInformation($"遇到江湖大盗，从{data.ProviderName}家挖出来的宝箱丢了 ~~~");
                }
            });

            EventBus.Subscribe("Box.Exists", arg => {
                var data = arg.GetData<INetworkPaperProvider>();
                if (data != null)
                {
                    logger.LogInformation($"哇，{data.ProviderName}家宝箱挖出{data.DefaultBox.Num}个宝贝，可是我已经有了 ~~~");
                }
            });

            EventBus.Subscribe("Box.Success", arg => {
                var data = arg.GetData<INetworkPaperProvider>();
                if (data != null)
                {
                    logger.LogInformation($"哇，{data.ProviderName}家宝箱挖出{data.DefaultBox.Num}个宝贝，收藏了 ~~~");
                }
            });

        }



    }
}
