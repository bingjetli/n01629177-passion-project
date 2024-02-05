using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace n01629177_passion_project.Models {
  public class Item {
    [Key]
    public int ItemId { get; set; }
    public string ItemBrand { get; set; }
    public string ItemName { get; set; }
    public string ItemVariant { get; set; }
    public int ItemWeightGrams { get; set; }
  }
}