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
    public IQueryable<BasePriceRecord> GetBasePriceRecords() {
      return db.BasePriceRecords;
    }

    // GET: api/PriceRecordData/5
    [ResponseType(typeof(BasePriceRecord))]
    public async Task<IHttpActionResult> GetBasePriceRecord(int id) {
      BasePriceRecord basePriceRecord = await db.BasePriceRecords.FindAsync(id);
      if (basePriceRecord == null) {
        return NotFound();
      }

      return Ok(basePriceRecord);
    }

    // PUT: api/PriceRecordData/5
    [ResponseType(typeof(void))]
    public async Task<IHttpActionResult> PutBasePriceRecord(int id, BasePriceRecord basePriceRecord) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      if (id != basePriceRecord.BasePriceRecordId) {
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
      BasePriceRecord price_record = new BasePriceRecord {
        BasePriceRecordCreationDate = payload.CreationDate,
        BasePriceRecordLastAttestationDate = payload.LastAttestationDate,
        BasePrice = payload.BasePrice,
        Item = item,
        ItemId = payload.ItemId,
        Shop = shop,
        ShopId = shop.ShopId,
        Users = new List<ApplicationUser>()
      };


      //Add the specified to the price record.
      price_record.Users.Add(user);


      //Then add the price record to the table.
      db.BasePriceRecords.Add(price_record);


      //And save the changes.
      db.SaveChanges();


      return Ok(price_record);
    }


    // DELETE: api/PriceRecordData/5
    [ResponseType(typeof(BasePriceRecord))]
    public async Task<IHttpActionResult> DeleteBasePriceRecord(int id) {
      BasePriceRecord basePriceRecord = await db.BasePriceRecords.FindAsync(id);
      if (basePriceRecord == null) {
        return NotFound();
      }

      db.BasePriceRecords.Remove(basePriceRecord);
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
      return db.BasePriceRecords.Count(e => e.BasePriceRecordId == id) > 0;
    }
  }
}