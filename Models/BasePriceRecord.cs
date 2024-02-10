using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace n01629177_passion_project.Models {
  public class BasePriceRecord {
    [Key]
    public int BasePriceRecordId { get; set; }


    [Required]
    public DateTime BasePriceRecordCreationDate { get; set; }


    [Required]
    public DateTime BasePriceRecordLastAttestationDate { get; set; }


    [Required]
    public float BasePrice { get; set; }


    //A BasePriceRecord must have 1 item; But an item can have many BasePriceRecords.
    public int ItemId { get; set; }
    public virtual Item Item { get; set; }


    //A BasePriceRecord must have 1 shop; but a shop can have many BasePriceRecords.
    public int ShopId { get; set; }
    public virtual Shop Shop { get; set; }


    //A BasePriceRecord must can have many Users; and a User can have many BasePriceRecords.
    //Each time a user is added, the `LastAttestationDate` should be updated.
    public virtual ICollection<ApplicationUser> Users { get; set; }

  }


  public class BasePriceRecordPostPayload {
    public DateTime CreationDate { get; set; }
    public DateTime LastAttestationDate { get; set; }
    public float BasePrice { get; set; }
    public int ItemId { get; set; }
    public int ShopId { get; set; }
    public string UserId { get; set; }


    public override string ToString() {
      return new JavaScriptSerializer().Serialize(this);
    }


    public HttpContent ToHttpContent() {
      return new StringContent(this.ToString(), Encoding.UTF8, "application/json");
    }
  }
}