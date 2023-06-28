namespace DynamicWallpaper.TreasureChest.Impl
{
    internal class DefaultProvider : NetworkProviderBase
    {
        public override string ProviderName => "Gmij";

        private string[]? listUrl;

        public override async Task<bool> DownLoadWallPaper(IBoxOptions opt)
        {
            var num = new Random().Next(1, opt.RandomHarvest);

            EventBus.Publish(EventName.BoxRandom, new CustomEventArgs(num));

            var prevTime = opt.LastOpenTime;
            if (!prevTime.HasValue || (prevTime.Value - DateTime.Now)> TimeSpan.FromMinutes(120) || listUrl == null)
            {
                listUrl = await LoadUrl<string[]>(opt.ListUrl);
            }
            

            if (listUrl == null ||  !listUrl.Any())
            {
                throw new Exception("No images found.");
            }
            else
            {
                var topImage = RandomUrlList(num) ?? throw new Exception("No images found.");

                Parallel.ForEach(topImage, async x => await SaveToCache($"{opt.ItemBaseUrl}{x.Replace("preview/", "wallpaper/")}", x[(x.LastIndexOf('/') + 1)..]));

                Task.WaitAll();
                opt.LastOpenTime = DateTime.Now;
                EventBus.Publish(EventName.BoxFinish, new CustomEventArgs(this));
                return true;
            }
        }


        private string[]? RandomUrlList(int count)
        {
            if (listUrl != null)
            {
                var list = new string[count];
                var random = new Random();
                for (int i = 0; i < count; i++)
                {
                    list[i] = listUrl[random.Next(0, listUrl.Length)];
                }
                return list;
            }
            return null;
        }

    }
}
