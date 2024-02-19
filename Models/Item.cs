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

    [Required]
    [StringLength(16, ErrorMessage = "Item price types may only have a maximum of 16 characters.")]
    public string PriceType { get; set; }


    [Required]
    public int DefaultQuantity { get; set; }


    //An Item can be associated with many Prices, but a Price can only correspond to 1 Item.
    public virtual ICollection<Price> Prices { get; set; }


    public ItemSerializable ToSerializable(bool is_deep = false) {
      if (is_deep == true) {


        //Serialize the item data as well as it's associated price collection data.
        PriceSerializable most_attested_price = null;
        int attestations = -1;
        var item_prices = new List<PriceSerializable>();
        foreach (var p in Prices) {
          item_prices.Add(p.ToSerializable());


          //Try to get the top price for this item.
          if (p.Users.Count > attestations) {

            attestations = p.Users.Count;
            most_attested_price = p.ToSerializable();
          }
        }


        return new ItemSerializable {
          ItemId = this.ItemId,
          Brand = this.Brand,
          Name = this.Name,
          Variant = this.Variant,
          PriceType = this.PriceType,
          DefaultQuantity = this.DefaultQuantity,

          IsDeep = is_deep,
          Prices = item_prices,
          MostAttestedPrice = most_attested_price,
          AttestationCount = attestations,
        };
      }


      return new ItemSerializable {
        ItemId = this.ItemId,
        Brand = this.Brand,
        Name = this.Name,
        Variant = this.Variant,
        PriceType = this.PriceType,
        DefaultQuantity = this.DefaultQuantity,

        IsDeep = is_deep,
        Prices = null,
        MostAttestedPrice = null,
        AttestationCount = -1,
      };
    }

  }




  public class ItemSerializable {
    public int ItemId { get; set; }
    public string Brand { get; set; }
    public string Name { get; set; }
    public string Variant { get; set; }

    //An item's price type can either be by "weight" or by individual "item"s.
    //This value determines how the price values are interpreted when it's time
    //to input and output the value.
    public string PriceType { get; set; }
    public int DefaultQuantity { get; set; }


    //Boolean property indicating whether or not this object is serialized with 
    //or without iterating the Prices array. If `IsDeep` is true, then this
    //object includes data from the `Prices` collection.
    public bool IsDeep { get; set; }
    public IEnumerable<PriceSerializable> Prices { get; set; }

    public PriceSerializable MostAttestedPrice { get; set; }
    public int AttestationCount { get; set; }


    public override string ToString() {
      return new JavaScriptSerializer().Serialize(this);
    }


    public HttpContent ToHttpContent() {
      return new StringContent(this.ToString(), Encoding.UTF8, "application/json");
    }
  }
}