using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PartsClient.Data;

namespace PartsClient.ViewModels;

public partial class AddPartViewModel : ObservableObject
{
    [ObservableProperty]
    private string _partID;

    [ObservableProperty]
    private string _partName;

    [ObservableProperty]
    private string _suppliers;

    [ObservableProperty]
    private string _partType;

    public AddPartViewModel()
    {
    }

    [RelayCommand]
    private async Task SaveData()
    {
        if (string.IsNullOrWhiteSpace(PartID))
        {
            await InsertPart();
        }
        else
        {
            await UpdatePart();
        }
    }


    [RelayCommand]
    private async Task InsertPart()
    {
        _ = await PartsManager.Add(PartName, Suppliers, PartType);

        _ = WeakReferenceMessenger.Default.Send(new RefreshMessage(true));

        await Shell.Current.GoToAsync("..");
    }


    [RelayCommand]
    private async Task UpdatePart()
    {
        Part partToSave = new()
        {
            PartID = PartID,
            PartName = PartName,
            PartType = PartType,
            Suppliers = [.. Suppliers.Split(",")]
        };

        await PartsManager.Update(partToSave);

        _ = WeakReferenceMessenger.Default.Send(new RefreshMessage(true));

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeletePart()
    {
        if (string.IsNullOrWhiteSpace(PartID))
        {
            return;
        }

        await PartsManager.Delete(PartID);

        _ = WeakReferenceMessenger.Default.Send(new RefreshMessage(true));

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private static async Task DoneEditing()
    {
        await Shell.Current.GoToAsync("..");
    }
}
