using n01629177_passion_project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace n01629177_passion_project.Controllers {
  public class PriceDataController : ApiController {
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: api/PriceData
    /// <summary>
    /// Returns a Generic Collection of all the `Price` objects stored inside
    /// the database.
    /// </summary>
    /// <returns>
    /// Always returns HTTP 200 : OK, with an ICollection of `Price` objects.
    /// </returns>
    [ResponseType(typeof(ICollection<Price>))]
    public IHttpActionResult GetAllPrices() {
      return Ok(db.Prices.Select(p => p.ToSerializable()).AsEnumerable());
    }




    // GET: api/PriceData?id={PRICE_ID}
    /// <summary>
    /// Returns the specified `PriceSerializable`.
    /// </summary>
    /// <param name="id">Integer primary key id of the `Price` object.</param>
    /// <returns>
    /// HTTP 200 OK with the `PriceSerializable` object if it finds the specified
    /// `Price` object. HTTP 404 Not Found otherwise.
    /// </returns>
    [ResponseType(typeof(PriceSerializable))]
    public IHttpActionResult GetPrice([FromUri] int id) {
      Price price = db.Prices.Find(id);
      if (price == null) {
        return NotFound();
      }

      return Ok(price.ToSerializable());
    }




    // PUT: api/PriceData/5
    /// <summary>
    /// Updates the specified Price object in the database. The PriceId of the 
    /// target Price object must be included in the PriceSerializable parameter.
    /// 
    /// Only the following fields can be updated : LastAttestationDate, Value,
    /// ItemId, and ShopId.
    /// </summary>
    /// <param name="updated">
    /// A PriceSerializable object containing the id of the Price to update
    /// as well as the fields to update.
    /// </param>
    /// <returns>
    /// HTTP 204 : No Content if successful, or HTTP 400 : Bad Request and
    /// HTTP 404 : Not Found if errors occurred.
    /// </returns>
    [ResponseType(typeof(void))]
    public IHttpActionResult PutPrice(PriceSerializable updated) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }


      //Fetch the corresponding entry to be updated.
      Price price = db.Prices.Find(updated.PriceId);

      if (updated.LastAttestationDate != null) {
        price.LastAttestationDate = updated.LastAttestationDate;
      }

      if (updated.Value != null) {
        price.Value = (float)updated.Value;
      }

      if (updated.ItemId != null) {

        //First check to see if this ItemId is a valid Id in the database.
        Item item = db.Items.Find(updated.ItemId);


        if (item == null) {
          return BadRequest($"Specified itemId {updated.ItemId} does not exist.");
        }


        //Then remove the previous reference to this Price object from the old
        //associated Item.
        price.Item.Prices.Remove(price);


        //Add this Price into the new Item's price list and update the relationship.
        item.Prices.Add(price);
        price.Item = item;
        price.ItemId = item.ItemId;
      }


      if (updated.ShopId != null) {

        //First check to see if this ShopId is a valid Id in the database.
        Shop shop = db.Shops.Find(updated.ShopId);


        if (shop == null) {
          return BadRequest($"Specified ShopId {updated.ShopId} does not exist.");
        }


        //Then remove the previous reference to this Price object from the old
        //associated Shop.
        price.Shop.Prices.Remove(price);


        //Add this Price into the new Shop's price list and update the relationship.
        shop.Prices.Add(price);
        price.Shop = shop;
        price.ShopId = shop.ShopId;
      }


      db.Entry(price).State = EntityState.Modified;


      try {
        db.SaveChanges();
      }
      catch (DbUpdateConcurrencyException) {
        if (!PriceExists(updated.PriceId)) {
          return NotFound();
        }
        else {
          throw;
        }
      }


      return StatusCode(HttpStatusCode.NoContent);
    }




    // POST: api/PriceData
    /// <summary>
    /// Creates the specified Price to be stored into the database.
    /// </summary>
    /// <param name="payload">
    /// PriceId, CreationDate and LastAttestationDate are all automatically 
    /// generated upon creation. So the only values needed for the PriceSerializable
    /// are the Value, ItemId, ShopId, and UserId
    /// </param>
    /// <returns>
    /// HTTP 200 : Ok along with the Serialized Price object. Or HTTP 400 : Bad 
    /// Request if there were any errors.
    /// </returns>
    [ResponseType(typeof(PriceSerializable))]
    public IHttpActionResult PostPrice(PriceSerializable payload) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }


      //First obtain the Navigational Properties and validate each one.
      ApplicationUser user = db.Users.Find(payload.UserId);
      if (user == null) return BadRequest($"The specified UserId {payload.UserId} does not exist.");

      Item item = db.Items.Find(payload.ItemId);
      if (item == null) return BadRequest($"The specified ItemId {payload.ItemId} does not exist.");

      Shop shop = db.Shops.Find(payload.ShopId);
      if (shop == null) return BadRequest($"The specified ShopId {payload.ItemId} does not exist.");


      //Then initialize the new Price object.
      Price new_price = new Price {
        PriceId = 0,
        CreationDate = DateTime.Now,
        LastAttestationDate = DateTime.Now,
        Value = (float)payload.Value,
        ItemId = (int)payload.ItemId,
        Item = item,
        ShopId = (int)payload.ShopId,
        Shop = shop,
        Users = new List<ApplicationUser>(),
      };


      //Assign the relationships.
      new_price.Users.Add(user);
      user.PriceAttestations.Add(new_price);


      //Add the price into the table.
      db.Prices.Add(new_price);
      db.SaveChanges();


      return Ok(new_price.ToSerializable());
    }




    // DELETE: api/PriceData?id={PRICE_ID}
    /// <summary>
    /// Attempts to Delete the Price associated with the specified PriceId.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>HTTP 404 : Not Found or HTTP 200 : Ok along with the deleted Price.</returns>
    [ResponseType(typeof(PriceSerializable))]
    public IHttpActionResult DeletePrice([FromUri] int id) {
      Price price = db.Prices.Find(id);
      if (price == null) {
        return NotFound();
      }


      db.Prices.Remove(price);
      db.SaveChanges();


      return Ok(price.ToSerializable());
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool PriceExists(int id) {
      return db.Prices.Count(e => e.PriceId == id) > 0;
    }
  }
}