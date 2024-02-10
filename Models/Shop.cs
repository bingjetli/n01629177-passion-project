using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace n01629177_passion_project.Models {
  public class Shop {

    [Key]
    public int ShopId { get; set; }

    public long ShopOverpassId { get; set; }

    [Required]
    [Range(-90.0, 90.0, ErrorMessage = "Only values from [-90, 90] are valid.")]
    public float ShopLatitude { get; set; }

    [Required]
    [Range(-180.0, 180.0, ErrorMessage = "Only values from [-180, 180] are valid.")]
    public float ShopLongitude { get; set; }

    [Required]
    [StringLength(128, MinimumLength = 6, ErrorMessage = "Shop names must be between 6 to 128 characters in length.")]
    public string ShopName { get; set; }

    [StringLength(128, ErrorMessage = "Shop addresses can be a maximum of 128 characters in length.")]
    public string ShopAddress { get; set; }


    //A shop can have many BasePriceRecords; but a BasePriceRecord can only be associated with 1 Shop.
    //public virtual ICollection<BasePriceRecord> PriceRecords { get; set; }


    //A Shop can have many Prices associated with it but a Price can only be
    //associated with 1 Shop.
    public virtual ICollection<Price> Prices { get; set; }





    public ShopDto ToDto() {
      return new ShopDto {
        ShopId = ShopId,
        ShopOverpassId = ShopOverpassId,
        ShopLatitude = ShopLatitude,
        ShopLongitude = ShopLongitude,
        ShopName = ShopName,
        ShopAddress = ShopAddress
      };
    }
  }


  public class ShopPostPayload {
    public long? ShopOverpassId { get; set; }

    [Required]
    public float ShopLatitude { get; set; }

    [Required]
    public float ShopLongitude { get; set; }

    [Required]
    public string ShopName { get; set; }

    public string ShopAddress { get; set; }
  }




  //Duplicated to avoid breaking existing functionality while testing.
  //TODO : merge this with `ShopPostPayload`.
  public class ShopDto {
    public int? ShopId { get; set; }

    public long? ShopOverpassId { get; set; }

    [Required]
    public float ShopLatitude { get; set; }

    [Required]
    public float ShopLongitude { get; set; }

    [Required]
    public string ShopName { get; set; }

    public string ShopAddress { get; set; }
  }


}