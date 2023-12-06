using IconCreator.Services;
using IconCreator.Views;
using OpenView;
using OpenView.Interfaces;
using OpenView.Services;

namespace IconCreator;

sealed class App : AppBase
{
    static readonly string _data = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".oneo", "iconCreator");

    public override string Name => "IconCreator";

    protected override IWindowModel CreateMain()
    {
        return new Main_Model();
    }

    protected override Task OnRunAsync(RunArgs args)
    {
        var configService = Service.Add(new ConfigService(_data));

        args.OnExit = () =>
        {
            configService.Save();
        };

        return Task.CompletedTask;
    }
}
