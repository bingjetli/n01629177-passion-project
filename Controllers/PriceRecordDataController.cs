using n01629177_passion_project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace n01629177_passion_project.Controllers {
  public class PriceRecordDataController : ApiController {
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: api/PriceRecordData
    public IQueryable<Price> GetBasePriceRecords() {
      return db.Prices;
    }

    // GET: api/PriceRecordData/5
    [ResponseType(typeof(Price))]
    public async Task<IHttpActionResult> GetBasePriceRecord(int id) {
      Price basePriceRecord = await db.Prices.FindAsync(id);
      if (basePriceRecord == null) {
        return NotFound();
      }

      return Ok(basePriceRecord);
    }

    // PUT: api/PriceRecordData/5
    [ResponseType(typeof(void))]
    public async Task<IHttpActionResult> PutBasePriceRecord(int id, Price basePriceRecord) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      if (id != basePriceRecord.PriceId) {
        return BadRequest();
      }

      db.Entry(basePriceRecord).State = EntityState.Modified;

      try {
        await db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) {
        if (!BasePriceRecordExists(id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }

      return StatusCode(HttpStatusCode.NoContent);
    }


    // POST: api/PriceRecordData
    [ResponseType(typeof(BasePriceRecord))]
    public IHttpActionResult PostBasePriceRecord(BasePriceRecordPostPayload payload) {

      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }


      //First obtain the Navigational Properties.
      ApplicationUser user = db.Users.Find(payload.UserId);
      Item item = db.Items.Find(payload.ItemId);
      Shop shop = db.Shops.Find(payload.ShopId);


      //Then create the BasePriceRecord.
      Price price_record = new Price {
        CreationDate = payload.CreationDate,
        LastAttestationDate = payload.LastAttestationDate,
        Value = payload.BasePrice,
        Item = item,
        ItemId = payload.ItemId,
        Shop = shop,
        ShopId = shop.ShopId,
        Users = new List<ApplicationUser>()
      };


      //Add the specified to the price record.
      price_record.Users.Add(user);


      //Then add the price record to the table.
      db.Prices.Add(price_record);


      //And save the changes.
      db.SaveChanges();


      return Ok(price_record);
    }


    // DELETE: api/PriceRecordData/5
    [ResponseType(typeof(BasePriceRecord))]
    public async Task<IHttpActionResult> DeleteBasePriceRecord(int id) {
      Price basePriceRecord = await db.Prices.FindAsync(id);
      if (basePriceRecord == null) {
        return NotFound();
      }

      db.Prices.Remove(basePriceRecord);
      await db.SaveChangesAsync();

      return Ok(basePriceRecord);
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool BasePriceRecordExists(int id) {
      return db.Prices.Count(e => e.PriceId == id) > 0;
    }
  }
}