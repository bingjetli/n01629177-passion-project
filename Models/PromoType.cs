using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace n01629177_passion_project.Models {
  public class PromoType {
    [Key]
    public int PromoTypeId { get; set; }
    public string PromoTypeName { get; set; }
  }
}