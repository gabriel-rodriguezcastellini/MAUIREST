using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PartsClient.Data;
using System.Collections.ObjectModel;

namespace PartsClient.ViewModels;

public partial class PartsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Part> _parts;


    [ObservableProperty]
    private bool _isRefreshing = false;


    [ObservableProperty]
    private bool _isBusy = false;


    [ObservableProperty]
    private Part _selectedPart;

    public PartsViewModel()
    {
        _parts = [];

        WeakReferenceMessenger.Default.Register<RefreshMessage>(this, async (r, m) =>
        {
            await LoadData();
        });

        _ = Task.Run(LoadData);
    }

    [RelayCommand]
    private async Task PartSelected()
    {
        if (SelectedPart == null)
        {
            return;
        }

        Dictionary<string, object> navigationParameter = new()
        {
            { "part", SelectedPart }
        };

        await Shell.Current.GoToAsync("addpart", navigationParameter);

        MainThread.BeginInvokeOnMainThread(() => SelectedPart = null);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsRefreshing = true;
            IsBusy = true;

            IEnumerable<Part> partsCollection = await PartsManager.GetAll();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Parts.Clear();

                foreach (Part part in partsCollection)
                {
                    Parts.Add(part);
                }
            });
        }
        finally
        {
            IsRefreshing = false;
            IsBusy = false;
        }
    }

    [RelayCommand]
    private static async Task AddNewPart()
    {
        await Shell.Current.GoToAsync("addpart");
    }

}
