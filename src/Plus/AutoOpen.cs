using DynamicWallpaper.TreasureChest;

namespace DynamicWallpaper.Plus
{
    internal class AutoOpen
    {

        public AutoOpen()
        {
            EventBus.Subscribe("Box.Ready", OpenTreasureChest);
            
        }

        private static void RegisterEvent()
        {
            EventBus.Register("Box.AutoOpen");
        }

        private void OpenTreasureChest(CustomEventArgs args)
        {
            EventBus.Publish("Box.AutoOpen", args);
            var ops = args.GetData<IBoxOptions>();
            var chest = ops?.TreasureChest;
            chest?.Open();
        }
    }
}
