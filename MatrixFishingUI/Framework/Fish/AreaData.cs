using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MatrixFishingUI.Framework.Fish;

public class AreaData : INotifyPropertyChanged
{
    public FishInfo Fish { get; set; } = null!;
    public string HeaderText { get; set; } = "";
    public string AreaName { get; set; } = "";
    
    public static AreaData GetArea(string areaName, FishInfo fish)
    {
        return new AreaData
        {
            HeaderText = fish.Name,
            Fish = fish,
            AreaName = areaName
        };
    }

    #region Property Changes

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}