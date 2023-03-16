using App.Pages;

namespace App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        Routing.RegisterRoute("TodoEditor", typeof(TodoEditor));

        MainPage = new AppShell();
    }
}
