namespace PhotoCatalogistApp.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public PageViewModelBase? CurrentPage { get; set; } =
        new MainPageViewModel();
}
