using System.Collections.ObjectModel;

using ReactiveUI;


namespace PhotoCatalogistApp.Desktop.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    public ObservableCollection<string>? ErrorMessages { get; } = [];
}
