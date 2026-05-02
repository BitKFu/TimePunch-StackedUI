namespace TimePunch_Uno_StackedUI_Demo;

public class Program
{
    private static App? _app;

    public static int Main(string[] args)
    {
        Console.WriteLine("[TPUNO] Program.Main start");
        Microsoft.UI.Xaml.Application.Start(_ => _app = new App());
        Console.WriteLine("[TPUNO] Program.Main after Application.Start");

        return 0;
    }
}
