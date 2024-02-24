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
    [ResponseType(typeof(ICollection<PriceSerializable>))]
    public IHttpActionResult GetAllPrices() {

      //This took frustratingly long to solve. Apparently Users are lazily
      //loaded, and trying to access it throws the most cryptic exception ever.
      //
      //"Cannot deserialize the current JSON object (e.g. {"name":"value"}) into
      //type 'System.Collections.Generic.IEnumerable`1[n01629177_passion_project.Models.PriceSerializable]'
      //because the type requires a JSON array (e.g. [1,2,3]) to deserialize correctly."
      //
      //Nothing about that indicates that I should've checked the code inside my
      //ToSerializable() function, find the line that calculates the attestations
      //from the amount of users in the price list. And then consider that the
      //I might not be accessing data that's available as yet because Users is
      //lazily loaded. So I would need to use the `Include()` method to eagerly
      //load the List before accessing it. Although, maybe in hindsight, I should've
      //considered it earlier. But in the moment, that exception was incredibly
      //misleading. 🤦‍🤦‍
      return Ok(db.Prices.Include(p => p.Users).AsEnumerable().Select(p => p.ToSerializable()));
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
    public IHttpActionResult GetPrice(
        [FromUri] int id,
        [FromUri] bool includeItem = false,
        [FromUri] bool includeShop = false,
        [FromUri] bool includeUsers = false
      ) {

      //Price price = null;
      IQueryable<Price> chainable_scope = db.Prices.AsQueryable();

      if (includeItem == true) {
        chainable_scope = chainable_scope.Include(p => p.Item);
      }

      if (includeShop == true) {
        chainable_scope = chainable_scope.Include(p => p.Shop);
      }

      if (includeUsers == true) {
        chainable_scope = chainable_scope.Include(p => p.Users);
      }

      Price price = chainable_scope.Where(p => p.PriceId == id).SingleOrDefault();
      if (price == null) {
        return NotFound();
      }

      return Ok(price.ToSerializable((includeItem || includeShop || includeUsers), includeItem, includeShop, includeUsers));
    }


    [HttpGet]
    [ResponseType(typeof(PriceSerializable))]
    public IHttpActionResult FindExistingPriceByUserId(
      [FromUri] string userId,
      [FromUri] int shopId,
      [FromUri] int itemId
    ) {

      //TODO: This should be done in the create route, to avoid making multiple
      //HTTP Calls.

      //SELF: I wonder if it makes a difference to search for all the items a 
      //user has attested to first, or start with the specific item and shop id?
      ApplicationUser user = db.Users.Find(userId);
      if (user == null) {

        //If the userId doesn't exist in the database, then this is a bad request
        //since users require an account to create an item price record.
        return BadRequest($"The specified user id {userId} is invalid.");
      }


      //For a single item & shop, a user should only have at most 1 attestation.
      //TODO: Error handling if multiple attestations are found.
      Price price_attestation = user.PriceAttestations.Where(p => (p.ItemId == itemId && p.ShopId == shopId)).SingleOrDefault();
      if (price_attestation == null) {
        return NotFound();
      }


      return Ok(price_attestation.ToSerializable());
    }




    // PUT: api/PriceData/5
    /// <summary>
    /// Updates the specified Price object in the database. The PriceId of the 
    /// target Price object must be included in the PriceSerializable parameter.
    /// 
    /// Only the following fields can be updated : LastAttestationDate, UnitPrice,
    /// Type, ItemId, and ShopId.
    /// 
    /// Specifying `Reattest = true` along with a valid `UserId` will remove the
    /// user's attestation from existing price records for the same item & shop
    /// before adding the user's attestation to this current price record.
    /// 
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
      if (price == null) {
        return BadRequest($"The specified priceId {updated.PriceId} does not exist.");
      }

      //SELF: There's a bug here with DateTime.MinValue, I suspect it might be due to
      //timezone issues. DateTime.MinValue doesn't seem like a reliable way to detect
      //the default value.
      if (updated.LastAttestationDate != null && updated.LastAttestationDate != DateTime.MinValue) {

        //If the new LastAttestationDate is not the default value.
        price.LastAttestationDate = updated.LastAttestationDate;
      }

      if (updated.Value != null) {
        price.Value = (float)updated.Value;
      }

      //if (updated.UnitPrice != null) {
      //  price.UnitPrice = (float)updated.UnitPrice;
      //}


      //if (updated.Type != null) {
      //  price.Type = updated.Type;
      //}


      if (updated.ItemId != null) {

        //First check to see if this ItemId is a valid Id in the database.
        Item item = db.Items.Find(updated.ItemId);


        if (item == null) {
          return BadRequest($"Specified itemId {updated.ItemId} does not exist.");
        }


        if (updated.ItemId != price.ItemId) {

          //SELF: Come to think of it, we probably don't even need this validation
          //since the price record should already have a valid item Id. 
          //TODO: Revert this.
          //Only make changes if the new itemId is different, we still want to
          //keep the itemId validation for the reattestation step if it is enabled.


          //Then remove the previous reference to this Price object from the old
          //associated Item.
          price.Item.Prices.Remove(price);


          //Add this Price into the new Item's price list and update the relationship.
          item.Prices.Add(price);
          price.Item = item;
          price.ItemId = item.ItemId;

        }

      }


      if (updated.ShopId != null) {

        //First check to see if this ShopId is a valid Id in the database.
        Shop shop = db.Shops.Find(updated.ShopId);


        if (shop == null) {
          return BadRequest($"Specified ShopId {updated.ShopId} does not exist.");
        }


        if (updated.ShopId != price.ShopId) {

          //SELF: Come to think of it, we probably don't even need this validation
          //since the price record should already have a valid shop Id. 
          //TODO: Revert this.
          //Only make changes if the new shopId is different, we still want to
          //keep the shopId validation for the reattestation step if it is enabled.


          //Then remove the previous reference to this Price object from the old
          //associated Shop.
          price.Shop.Prices.Remove(price);


          //Add this Price into the new Shop's price list and update the relationship.
          shop.Prices.Add(price);
          price.Shop = shop;
          price.ShopId = shop.ShopId;
        }

      }


      if (updated.Reattest == true) {

        //Indicates that the user wishes to make an attestation to this price record.

        if (updated.UserId == null) {
          return BadRequest($"A userId must be supplied with this request if the Reattest option is enabled.");
        }


        ApplicationUser user = db.Users.Find(updated.UserId);
        if (user == null) {
          return BadRequest($"The supplied userId {updated.UserId} is invalid.");
        }


        //SELF: I wonder if this might cause any bugs by using `price.ItemId`
        //and `price.ShopId` to check instead of the `updated` values. Maybe
        //we might encounter a bug where `price` is still referencing an old
        //item or shop since changes haven't been commited to the database yet.
        Price existing_attestation = user.PriceAttestations.Where(p => (p.ItemId == price.ItemId && p.ShopId == price.ShopId)).SingleOrDefault();
        if (existing_attestation != null) {

          //This means the user already has an existing attestation, so now we can
          //remove this user's attestation from it first before we add the new price.
          existing_attestation.Users.Remove(user);
        }


        //Now reattest this user to this price record if they aren't already in
        //the list.
        if (price.Users.Contains(user) == false) {
          price.Users.Add(user);
        }


        //Then update the last time this price record was attested.
        price.LastAttestationDate = DateTime.Now;
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
      if (shop == null) return BadRequest($"The specified ShopId {payload.ShopId} does not exist.");


      //For a single item & shop, a user should only have at most 1 attestation.
      Price existing_attestation = user.PriceAttestations.Where(p => (p.ItemId == payload.ItemId && p.ShopId == payload.ShopId)).SingleOrDefault();
      if (existing_attestation != null) {

        //This means the user already has an existing attestation, so now we can
        //remove this user's attestation from it first before we add the new price.
        existing_attestation.Users.Remove(user);
      }


      //Validate other parts of the payload.

      //As of 2024-02-19, there are only 2 valid values for this field, we store
      //price either by weight or by individual item cost.
      //if (payload.Type != "weight" || payload.Type != "item") return BadRequest($"Invalid Type specified: {payload.Type}");


      //Negative unit prices are invalid.
      //if (payload.UnitPrice < 0) return BadRequest($"The unit price is invalid : {payload.UnitPrice}");


      //Then initialize the new Price object.
      Price new_price = new Price {
        PriceId = 0,
        CreationDate = DateTime.Now,
        LastAttestationDate = DateTime.Now,
        Value = (float)payload.Value,
        //UnitPrice = (float)payload.UnitPrice,
        //Type = payload.Type,
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