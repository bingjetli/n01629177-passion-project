using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace n01629177_passion_project.Models {
  public class Item {
    [Key]
    public int ItemId { get; set; }

    [Required]
    [StringLength(128, ErrorMessage = "Item brand names may only have a maximum of 128 characters.")]
    public string ItemBrand { get; set; }

    [Required]
    [StringLength(128, ErrorMessage = "Item names may only have a maximum of 128 characters.")]
    public string ItemName { get; set; }

    [Required]
    [StringLength(128, ErrorMessage = "Item variant names may only have a maximum of 128 characters.")]
    public string ItemVariant { get; set; }

    //An item can have many price records; but a price record can only correspond to 1 item.
    //public virtual ICollection<BasePriceRecord> ItemPriceRecords { get; set; }


    //An Item can be associated with many Prices, but a Price can only correspond to 1 Item.
    public virtual ICollection<Price> Prices { get; set; }


    //Methods
    public ItemDto ToDto() {
      return new ItemDto {
        ItemVariant = ItemVariant,
        ItemBrand = ItemBrand,
        ItemName = ItemName,
        ItemId = ItemId,
      };
    }
  }


  public class ItemDto {
    public string ItemBrand { get; set; }
    public string ItemName { get; set; }
    public string ItemVariant { get; set; }
    public int ItemId { get; set; }
  }
}