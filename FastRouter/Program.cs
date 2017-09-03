namespace FastRouter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Hash exact match
            MeasureActionRoute<HashDictionaryRouter>("");
            MeasureActionRouteModulo<HashDictionaryRouter>();

            // Action match
            System.Diagnostics.Trace.WriteLine($"Using lookup");
            ActionDictionary.UseLookup = true;
            MeasureActionRoute<ActionDictionaryRouter>("?test");
            MeasureActionRouteModulo<ActionDictionaryRouter>();
            MeasureActionRoute<ActionDictionaryRouter>("");

            System.Diagnostics.Trace.WriteLine($"Not using lookup");
            ActionDictionary.UseLookup = false;
            MeasureActionRoute<ActionDictionaryRouter>("?test");
            MeasureActionRouteModulo<ActionDictionaryRouter>();
            MeasureActionRoute<ActionDictionaryRouter>("");
        }

        private static void MeasureActionRoute<TRouter>(string actionSuffix)
            where TRouter: BaseRouter, new()
        {
            System.Diagnostics.Trace.WriteLine($"MeasureActionRoute for {typeof(TRouter).Name} with suffix '{actionSuffix}'");
            var router = new TRouter();
            router.Init();
            foreach (var testAction in router.TestActions)
            {
                var action = testAction.route + actionSuffix;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                for (var i = 0; i < 5000000; i++)
                {
                    router.Route(action);
                }
                watch.Stop();
                System.Diagnostics.Trace.WriteLine($"# {testAction.route}: {watch.ElapsedMilliseconds}");
            }
        }

        private static void MeasureActionRouteModulo<TRouter>()
            where TRouter : BaseRouter, new()
        {
            System.Diagnostics.Trace.WriteLine($"MeasureActionRouteModulo for {typeof(TRouter).Name}");
            var router = new TRouter();
            router.Init();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var count = router.TestActions.Count;
            for (var i = 0; i < 5000000; i++)
            {
                var action = router.TestActions[i % count].route;
                router.Route(action);
            }
            watch.Stop();
            System.Diagnostics.Trace.WriteLine($"# total: {watch.ElapsedMilliseconds}");
        }
    }
}
