using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace n01629177_passion_project.Models {
  public class Item {
    [Key]
    public int ItemId { get; set; }

    [Required]
    [StringLength(128, ErrorMessage = "Item brand names may only have a maximum of 128 characters.")]
    public string Brand { get; set; }

    [Required]
    [StringLength(128, ErrorMessage = "Item names may only have a maximum of 128 characters.")]
    public string Name { get; set; }

    [Required]
    [StringLength(128, ErrorMessage = "Item variant names may only have a maximum of 128 characters.")]
    public string Variant { get; set; }

    //An Item can be associated with many Prices, but a Price can only correspond to 1 Item.
    public virtual ICollection<Price> Prices { get; set; }


    public ItemSerializable ToSerializable() {
      return new ItemSerializable {
        ItemId = this.ItemId,
        Brand = this.Brand,
        Name = this.Name,
        Variant = this.Variant
      };
    }


    //Methods
    //public ItemDto ToDto() {
    //  return new ItemDto {
    //    ItemVariant = ItemVariant,
    //    ItemBrand = ItemBrand,
    //    ItemName = ItemName,
    //    ItemId = ItemId,
    //  };
    //}
  }


  //public class ItemDto {
  //  public string ItemBrand { get; set; }
  //  public string ItemName { get; set; }
  //  public string ItemVariant { get; set; }
  //  public int ItemId { get; set; }
  //}


  public class ItemSerializable {
    public int ItemId { get; set; }
    public string Brand { get; set; }
    public string Name { get; set; }
    public string Variant { get; set; }


    public override string ToString() {
      return new JavaScriptSerializer().Serialize(this);
    }


    public HttpContent ToHttpContent() {
      return new StringContent(this.ToString(), Encoding.UTF8, "application/json");
    }
  }
}