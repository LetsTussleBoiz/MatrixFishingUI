using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MatrixFishingUI.Framework.Fish;

public class SpecialConditionData : INotifyPropertyChanged
{
    
    public FishInfo Fish { get; set; } = null!;
    public string HeaderText { get; set; } = "";
    public List<string> Conditions { get; set; } = [];
    
    public static SpecialConditionData GetSpecialConditions(List<string> conditions, FishInfo fish)
    {
        return new SpecialConditionData
        {
            HeaderText = fish.Name,
            Fish = fish,
            Conditions = conditions
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