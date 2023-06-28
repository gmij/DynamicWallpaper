using DynamicWallpaper.TreasureChest;

namespace DynamicWallpaper.Plus
{
    internal class AutoOpen
    {

        public AutoOpen()
        {
            EventBus.Subscribe(EventName.BoxReady, OpenTreasureChest);
        }

        private static void RegisterEvent()
        {
            EventBus.Register(EventName.BoxAutoOpen);
        }

        private void OpenTreasureChest(CustomEventArgs args)
        {
            EventBus.Publish(EventName.BoxAutoOpen, args);
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
