using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace n01629177_passion_project.Models {
  public class PromoRecord {
    [Key]
    public int PromoRecordId { get; set; }
    public DateTime PromoRecordCreationDate { get; set; }
    public DateTime PromoRecordLastAttestationDate { get; set; }
    public float PromoRecordPrice { get; set; }

    [ForeignKey("PromoType")]
    public int PromoTypeId { get; set; }
    public virtual PromoType PromoType { get; set; }

    [ForeignKey("Item")]
    public int ItemId { get; set; }
    public virtual Item Item { get; set; }

    [ForeignKey("Shop")]
    public int ShopId { get; set; }
    public virtual Shop Shop { get; set; }

    //A PromoRecord can have many users. Each time a
    //user is added, the `lastAttestationDate` should
    //be updated.
    //TODO: add Icollection<Users>
  }
}