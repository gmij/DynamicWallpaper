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
            _boxBehavior.TreasureChest = this;
            _provider = provider;
        }

        public IBoxOptions Box => _boxBehavior;

        public INetworkProvider Provider => _provider;

        public Control Panel => _panel;

        public void Open()
        {
            var chest = _panel as TreasureChestPanel;
            if (chest != null)
            {
                chest.Open();
            }
        }
    }
}
