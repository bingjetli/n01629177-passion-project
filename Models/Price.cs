using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace n01629177_passion_project.Models {
  public class Price {
    [Key]
    public int PriceId { get; set; }


    [Required]
    public DateTime CreationDate { get; set; }


    [Required]
    public DateTime LastAttestationDate { get; set; }


    //Prices are stored as unit prices for the item that they are associated with.
    //So depending on the item's price type, this value should get calculated to
    //its appropriate value. See `devlog/handling-prices-item-weight.md`
    [Required]
    public float Value { get; set; }


    //A BasePriceRecord must have 1 item; But an item can have many BasePriceRecords.
    public int ItemId { get; set; }
    public virtual Item Item { get; set; }


    //A BasePriceRecord must have 1 shop; but a shop can have many BasePriceRecords.
    public int ShopId { get; set; }
    public virtual Shop Shop { get; set; }


    //A BasePriceRecord must can have many Users; and a User can have many BasePriceRecords.
    //Each time a user is added, the `LastAttestationDate` should be updated.
    public virtual ICollection<ApplicationUser> Users { get; set; }




    public PriceSerializable ToSerializable(
        bool is_deep = false,
        bool include_item = false,
        bool include_shop = false,
        bool include_users = false
      ) {

      if (is_deep == true) {
        return new PriceSerializable {
          PriceId = this.PriceId,
          CreationDate = this.CreationDate,
          LastAttestationDate = this.LastAttestationDate,
          Value = this.Value,
          ItemId = this.ItemId,
          ShopId = this.ShopId,

          IsDeep = is_deep,
          Attestations = include_users ? this.Users.Count : -1,
          Item = include_item ? this.Item.ToSerializable() : null,
          Shop = include_shop ? this.Shop.ToSerializable() : null,
        };
      }

      return new PriceSerializable {
        PriceId = this.PriceId,
        CreationDate = this.CreationDate,
        LastAttestationDate = this.LastAttestationDate,
        Value = this.Value,
        ItemId = this.ItemId,
        ShopId = this.ShopId,
      };
    }

  }


  public class PriceSerializable {
    public int PriceId { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime LastAttestationDate { get; set; } = DateTime.MinValue;
    public float? Value { get; set; }
    public int? ItemId { get; set; }
    public int? ShopId { get; set; }


    //TO-SELF: This is here so we can pass the UserId in the POST request to make
    //a new record.
    public string UserId { get; set; }

    //SELF: A boolean flag for PUT requests, when enabled, it indicates that we
    //want to update a user's attestation to this price record. This means removing
    //this user's existing attestations to a price record of the same itemId &
    //shopId, before adding their attestation to this price record.
    public bool Reattest { get; set; } = false;


    public bool IsDeep { get; set; } = false;
    public ItemSerializable Item { get; set; } = null;
    public ShopSerializable Shop { get; set; } = null;

    //TO-SELF: Why not just return a list of UserIDs?
    //Probably not a good idea to make the user-ids public to the end users. Call it intuition.
    public int Attestations { get; set; } = -1;




    public override string ToString() {
      return new JavaScriptSerializer().Serialize(this);
    }


    public HttpContent ToHttpContent() {
      return new StringContent(this.ToString(), Encoding.UTF8, "application/json");
    }
  }
}