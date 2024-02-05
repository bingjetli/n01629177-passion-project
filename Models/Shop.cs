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

    public float ShopLatitude { get; set; }
    public float ShopLongitude { get; set; }
    public string ShopTitle { get; set; }
    public string ShopVariant { get; set; }
  }
}