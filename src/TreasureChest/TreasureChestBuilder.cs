namespace DynamicWallpaper.TreasureChest
{

    /// <summary>
    /// 参数已经被使用
    /// </summary>
    class AlreadyUseParamException: Exception
    {

    }

    /// <summary>
    /// 宝箱构建器
    /// </summary>
    internal class TreasureChestBuilder
    {
        private TreasureChestPanel _panel;
        private IBoxOptions _boxBehavior;
        private INetworkProvider _provider;

        public TreasureChestBuilder AddPanel(TreasureChestPanel panel)
        {
            if (_panel == null)
                _panel = panel;
            else
                throw new AlreadyUseParamException();
            return this;
        }

        public TreasureChestBuilder AddBoxBehavior(IBoxOptions boxBehavior) {
            if (_boxBehavior == null)
                _boxBehavior = boxBehavior;
            else
                throw new AlreadyUseParamException();
            return this;
        }

        public TreasureChestBuilder AddProvider(INetworkProvider provider) {
            if (_provider == null)
                _provider = provider;
            else
                throw new AlreadyUseParamException();
            return this;
        }

        public ITreasureChest Build()
        {

            _panel.InitializeComponent(_boxBehavior);
            _panel.OpenHandler += (sender, opt) =>
            {
                _provider.DownLoadWallPaper(opt);
            };
            return Build(_panel, _boxBehavior, _provider);
        }

        private ITreasureChest Build(Control panel, IBoxOptions boxBehavior, INetworkProvider provider)
        {
            return new Impl.TreasureChest(panel, boxBehavior, provider);
        }

    }

    class TreasureChestBuilderFactory
    {
        public static ITreasureChest CreateTreasureChest4Bing()
        {
            var builder = new TreasureChestBuilder();
            builder.AddPanel(new TreasureChestPanel());
            builder.AddBoxBehavior(new Impl.BingBoxOptions());
            builder.AddProvider(new Impl.BingProvider());
            return builder.Build();
        }

        public static ITreasureChest CreateTreasureChest4Pixabay()
        {
            var builder = new TreasureChestBuilder();
            builder.AddPanel(new TreasureChestPanel());
            builder.AddBoxBehavior(new Impl.PixabayBoxOptions());
            builder.AddProvider(new Impl.PixabayProvider());
            return builder.Build();
        }

        public static ITreasureChest CreateTreasureChest4Wallhaven()
        {
            var builder = new TreasureChestBuilder();
            builder.AddPanel(new TreasureChestPanel());
            builder.AddBoxBehavior(new Impl.WallhavenBoxOptions());
            builder.AddProvider(new Impl.WallhavenProvider());
            return builder.Build();
        }
    }
}
