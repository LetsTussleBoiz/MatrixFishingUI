using System.ComponentModel;
using System.Runtime.CompilerServices;
using StardewValley;
using StardewValley.GameData.FishPonds;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.Fish;

public partial class PondItemData : INotifyPropertyChanged
{
    public List<PondInfoModel> ProducedItems { get; set; } = [];

    public static PondItemData GetPondItems(PondInfo? pond, Item fish)
    {
        var list = new List<PondInfoModel>();
        if (pond is not null)
        {
            list.AddRange(pond.Value.FishPondRewards
                .Select(item => new PondInfoModel(
                    ItemRegistry.GetData(item.ItemId), 
                    GetSalePrice(ItemRegistry.Create(item.ItemId), fish), 
                    $"[Population Required: {item.RequiredPopulation}]")));
        }
        
        return new PondItemData
        {
            ProducedItems = list
        };
    }

    private static string GetSalePrice(Item item, Item fish)
    {
        if(item.QualifiedItemId is not "(O)812") return $"[Sale Price: {item.salePrice()}g] ";
        ItemQueryContext itemQueryContext = new();

        var result = ItemQueryResolver.DefaultResolvers.FLAVORED_ITEM(
            string.Empty,
            $"{SObject.PreserveType.Roe.ToString()} {fish.QualifiedItemId}",
            itemQueryContext,
            avoidRepeat: true,
            avoidItemIds: [],
            logError: (_, _) => { })
            .FirstOrDefault();

        return result is null ? string.Empty : $"[Sale Price: {result.Item.salePrice()}g] ";
    }
    
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
}

public record PondInfoModel(
    ParsedItemData Item,
    string SalesPrice,
    string PopulationRequired);
