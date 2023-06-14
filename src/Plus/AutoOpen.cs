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
            if (ops == null)
                return; 
            if (!ops.AutoOpen)
                return;
            var chest = ops?.TreasureChest;
            if (chest != null )
                chest?.Open();
        }
    }
}
