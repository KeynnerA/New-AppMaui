namespace MauiApp1.Services;

public static class ViewModelLocator
{
    private static MauiApp1.Models.NotasViewModel? _notasViewModel;

    public static MauiApp1.Models.NotasViewModel NotasViewModel
    {
        get
        {
            _notasViewModel ??= new MauiApp1.Models.NotasViewModel();
            return _notasViewModel;
        }
    }

    public static void Reset()
    {
        _notasViewModel = null;
    }
}
