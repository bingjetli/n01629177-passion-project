using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace n01629177_passion_project.Models.ViewModels {
  public class ItemDetails {
    public ItemSerializable item { get; set; }
    public IEnumerable<PriceSerializable> prices { get; set; }
  }


  //TODO : DELETE
}