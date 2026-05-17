    namespace project
    {
        public partial class App : Application
        {
            public App()
            {
                InitializeComponent();

                SupabaseService.Initialize().Wait();
            }

            protected override Window CreateWindow(IActivationState? activationState)
            {
                return new Window(new AppShell());
            }
        }
    }