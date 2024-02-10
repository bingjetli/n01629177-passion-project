using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace n01629177_passion_project.Models {
  public class Shop {

    [Key]
    public int ShopId { get; set; }

    public long OverpassId { get; set; }

    [Required]
    [Range(-90.0, 90.0, ErrorMessage = "Only values from [-90, 90] are valid.")]
    public float Latitude { get; set; }

    [Required]
    [Range(-180.0, 180.0, ErrorMessage = "Only values from [-180, 180] are valid.")]
    public float Longitude { get; set; }

    [Required]
    [StringLength(128, MinimumLength = 6, ErrorMessage = "Shop names must be between 6 to 128 characters in length.")]
    public string Name { get; set; }

    [StringLength(128, ErrorMessage = "Shop addresses can be a maximum of 128 characters in length.")]
    public string Address { get; set; }

    //A Shop can have many Prices associated with it but a Price can only be
    //associated with 1 Shop.
    public virtual ICollection<Price> Prices { get; set; }


    public ShopSerializable ToSerializable() {
      return new ShopSerializable {
        ShopId = this.ShopId,
        OverpassId = this.OverpassId,
        Latitude = this.Latitude,
        Longitude = this.Longitude,
        Name = this.Name,
        Address = this.Address
      };
    }
  }


  public class ShopSerializable {
    public int ShopId { get; set; }
    public long? OverpassId { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }


    public override string ToString() {
      return new JavaScriptSerializer().Serialize(this);
    }


    public HttpContent ToHttpContent() {
      return new StringContent(this.ToString(), Encoding.UTF8, "application/json");
    }
  }
}
