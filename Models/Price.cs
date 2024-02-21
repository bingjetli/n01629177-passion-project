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


    //Deprecated.
    //Store the unit price instead of the value. Then allow the user to choose
    //weather or not to see the price by weight or price by unit. If the price
    //is by weight, allow the user to set the weight and unit.
    //public float UnitPrice { get; set; }

    //Deprecated.
    //Determines whether this Price record is for "weight" or and individual "item".
    //public string Type { get; set; }


    //A BasePriceRecord must have 1 item; But an item can have many BasePriceRecords.
    public int ItemId { get; set; }
    public virtual Item Item { get; set; }


    //A BasePriceRecord must have 1 shop; but a shop can have many BasePriceRecords.
    public int ShopId { get; set; }
    public virtual Shop Shop { get; set; }


    //A BasePriceRecord must can have many Users; and a User can have many BasePriceRecords.
    //Each time a user is added, the `LastAttestationDate` should be updated.
    public virtual ICollection<ApplicationUser> Users { get; set; }




    public PriceSerializable ToSerializable() {
      return new PriceSerializable {
        PriceId = this.PriceId,
        CreationDate = this.CreationDate,
        LastAttestationDate = this.LastAttestationDate,
        Value = this.Value,
        //UnitPrice = this.UnitPrice,
        //Type = this.Type,
        ItemId = this.ItemId,
        ShopId = this.ShopId,
        Attestations = this.Users.Count,
      };
    }

  }


  public class PriceSerializable {
    public int PriceId { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime LastAttestationDate { get; set; } = DateTime.MinValue;
    public float? Value { get; set; }
    //public float? UnitPrice { get; set; }
    //public string Type { get; set; }
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


    //TO-SELF: Why not just return a list of UserIDs?
    //Probably not a good idea to make the user-ids public to the end users. Call it intuition.
    public int Attestations { get; set; }




    public override string ToString() {
      return new JavaScriptSerializer().Serialize(this);
    }


    public HttpContent ToHttpContent() {
      return new StringContent(this.ToString(), Encoding.UTF8, "application/json");
    }
  }
}