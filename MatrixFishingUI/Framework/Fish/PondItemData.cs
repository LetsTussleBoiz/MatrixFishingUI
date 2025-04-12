using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace MatrixFishingUI.Framework.Fish;

public partial class PondItemData : INotifyPropertyChanged
{
    public List<PondInfoModel> ProducedItems { get; set; } = [];

    public static PondItemData GetPondItems(PondInfo? pond, Item fish)
    {
        var list = new List<PondInfoModel>();
        if (pond is null)
            return new PondItemData
            {
                ProducedItems = list
            };
        foreach (var reward in pond.FishPondRewards)
        {
            var formattedQuantity = "";
            if (reward.MinStack < reward.MaxStack)
            {
                formattedQuantity = $"{reward.MinStack} - {reward.MaxStack}";
            } else if (reward.MinStack == reward.MaxStack && reward.MinStack > -1)
            {
                formattedQuantity = $"{reward.MinStack}";
            }

            if (ItemRegistry.GetData(reward.ItemId).QualifiedItemId is "(O)812")
            {
                var roe = (ColoredObject)FishHelper.GetRoeForFish(new SObject(fish.ItemId, 1));
                list.Add(new PondInfoModel(
                    ItemRegistry.GetData(reward.ItemId),
                    GetSalePrice(ItemRegistry.Create(reward.ItemId), fish),
                    $"[{I18n.Ui_Fishipedia_Ponds_PopRequired()} {reward.RequiredPopulation}]", 
                    $"{reward.MinStack}",
                    $"{reward.MaxStack}",
                    formattedQuantity,
                    $"[{Math.Round(reward.Chance * 100, 1).ToString(CultureInfo.InvariantCulture)}%]",
                    true,
                    roe,
                    roe.color.Value));
            }
            else
            {
                list.Add(new PondInfoModel(
                    ItemRegistry.GetData(reward.ItemId),
                    GetSalePrice(ItemRegistry.Create(reward.ItemId), fish),
                    $"[{I18n.Ui_Fishipedia_Ponds_PopRequired()} {reward.RequiredPopulation}]", 
                    $"{reward.MinStack}",
                    $"{reward.MaxStack}",
                    formattedQuantity,
                    $"[{Math.Round(reward.Chance * 100, 1).ToString(CultureInfo.InvariantCulture)}%]",
                    false));
            }
        }

        return new PondItemData
        {
            ProducedItems = list
        };
    }

    private static string GetSalePrice(Item item, Item fish)
    {
        if(item.QualifiedItemId is not "(O)812") return $"[{I18n.Ui_Fishipedia_Ponds_SalePrice()} {item.salePrice()/2}g] ";
        var obj = new SObject(fish.ItemId, 1);
        return $"[{I18n.Ui_Fishipedia_Ponds_SalePrice()} {FishHelper.GetRoeForFish(obj).salePrice()/2}g] ";
        // ItemQueryContext itemQueryContext = new();
        //
        // var result = ItemQueryResolver.DefaultResolvers.FLAVORED_ITEM(
        //     string.Empty,
        //     $"{SObject.PreserveType.Roe.ToString()} {fish.QualifiedItemId}",
        //     itemQueryContext,
        //     avoidRepeat: true,
        //     avoidItemIds: [],
        //     logError: (_, _) => { })
        //     .FirstOrDefault();
        //
        // return result is null ? string.Empty : $"[{I18n.Ui_Fishipedia_Ponds_SalePrice()} {result.Item.salePrice()}g] ";
    }

    #region PropertyChanges

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

public record PondInfoModel(
    ParsedItemData Item,
    string SalesPrice,
    string PopulationRequired,
    string MinQuantity,
    string MaxQuantity,
    string QuantityString,
    string Chance,
    bool IsRoe,
    ColoredObject? FlavoredRoe = null,
    Color? RoeColor = null);
