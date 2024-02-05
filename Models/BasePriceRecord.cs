using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace n01629177_passion_project.Models {
  public class BasePriceRecord {
    [Key]
    public int BasePriceRecordId { get; set; }
    public DateTime BasePriceRecordCreationDate { get; set; }
    public DateTime BasePriceRecordLastAttestationDate { get; set; }
    public DateTime BasePriceRecordPrice { get; set; }

    [ForeignKey("Item")]
    public int ItemId { get; set; }
    public virtual Item Item { get; set; }

    [ForeignKey("Shop")]
    public int ShopId { get; set; }
    public virtual Shop Shop { get; set; }

    //A BasePrice Record can have many users. Each time
    //a user is added, the `LastAttestationDate` should
    //be updated.
    //TODO : add ICollection<Users>
  }
}