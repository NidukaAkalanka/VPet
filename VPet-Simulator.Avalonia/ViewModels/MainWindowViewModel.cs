using ReactiveUI;

namespace VPet_Simulator.Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public string Title { get; } = "VPet Simulator - Linux";
}