namespace DynamicWallpaper.TreasureChest.Impl
{



    internal class TreasureChest : ITreasureChest
    {
        private readonly Control _panel;
        private readonly IBoxOptions _boxBehavior;
        private readonly INetworkProvider _provider;

        public TreasureChest(Control panel, IBoxOptions boxBehavior, INetworkProvider provider)
        {
            _panel = panel;
            _boxBehavior = boxBehavior;
            _provider = provider;
        }

        public IBoxOptions Box => _boxBehavior;

        public INetworkProvider Provider => _provider;

        public Control Panel => _panel;

        // Implement the ITreasureChest interface methods here.
    }
}
