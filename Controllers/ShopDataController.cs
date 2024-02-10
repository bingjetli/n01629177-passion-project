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
using System.Web.WebPages;

namespace n01629177_passion_project.Controllers {
  public class ShopDataController : ApiController {
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: api/ShopData
    /// <summary>
    /// Returns a generic collection of Serializable Shop objects stored in the database.
    /// </summary>
    /// <returns>
    /// Always returns HTTP 200 : OK, with an ICollection of `ShopSerializable` objects.
    /// </returns>
    [ResponseType(typeof(ICollection<ShopSerializable>))]
    public IHttpActionResult GetAllShops() {
      return Ok(db.Shops.AsEnumerable().Select(s => s.ToSerializable()));
    }




    // GET: api/ShopData?id={SHOP_ID}&isOverpass={TRUE/FALSE}
    /// <summary>
    /// Returns a Serializable Shop object associated with the specified id.
    /// </summary>
    /// <param name="id">Either the primary key ShopId or the OverpassId.</param>
    /// <param name="isOverpass">
    /// Boolean flag to specify that the provided Id is an OverpassId.
    /// </param>
    /// <returns>
    /// HTTP 200 : OK and A `ShopSerializable` associated with the specified `id` if found.
    /// Otherwise HTTP 404 : Not Found.
    /// </returns>
    [ResponseType(typeof(ShopSerializable))]
    public IHttpActionResult GetShop([FromUri] long id, bool isOverpass) {
      Shop result = null;

      if (isOverpass == true) {

        //Search by the Overpass Id...
        result = db.Shops.Where(s => s.OverpassId == id).SingleOrDefault();
      }
      else {

        //Search by default Id instead...
        result = db.Shops.Find((int)id);
      }


      if (result == null) return NotFound();


      return Ok(result.ToSerializable());
    }




    // GET: api/ShopData?shopId={SHOP_ID}&searchOverpassId={TRUE/FALSE}
    //[HttpGet]
    //[ResponseType(typeof(Shop))]
    //public IHttpActionResult GetShop(
    //    int shopId,
    //    bool? searchOverpassId
    //  ) {

    //  Shop shop;
    //  if (searchOverpassId == true) {
    //    shop = db.Shops.Where(s => s.ShopOverpassId == shopId).FirstOrDefault();
    //  }
    //  else {
    //    shop = db.Shops.Find(shopId);
    //  }


    //  if (shop == null) {
    //    return NotFound();
    //  }


    //  return Ok(shop);
    //}




    // GET: api/ShopData?shopOverpassId={SHOP_OVERPASS_ID}
    //[HttpGet]
    //[ResponseType(typeof(ShopDto))]
    //public IHttpActionResult GetShopByOverpassId([System.Web.Http.FromUri] long shopOverpassId) {

    //  //RESOLVED : Find out why the heck this fails to handle integers greater than 9 digits when using the
    //  //`int` datatype for the parameter. This shouldn't fail since int32 values range from [-2,147,483,647 to 2,147,483,647].
    //  //But for some reason `https://localhost:44320/api/ShopData?shopOverpassId=5355856076` fails, whilst
    //  // `https://localhost:44320/api/ShopData?shopOverpassId=535585607` works. 

    //  //As it turns out, 5 355 856 076 does in-fact exceed the max value for int32. The spacing was visually deceptive to me.

    //  Shop shop = db.Shops.Where(s => s.ShopOverpassId == shopOverpassId).FirstOrDefault();
    //  if (shop == null) {
    //    return NotFound();
    //  }

    //  return Ok(shop.ToDto());
    //}




    // PUT: api/ShopData
    /// <summary>
    /// Updates the specified Shop with the values specified. The ShopId of the
    /// target Shop object must be included in the payload.
    /// 
    /// The ShopId, Name, and Address fields must always be included in the payload.
    ///
    /// The Name and Address fields are only updated if the string is not empty.
    /// 
    /// Only the Name, Address, OverpassId, Latitude and Longitude fields can be
    /// updated.
    ///
    /// </summary>
    /// <param name="updated">
    /// A `ShopSerializable` object containing the data for the Shop to be updated.
    /// </param>
    /// <returns>
    /// HTTP 204 : No Content if the update was successful.
    /// HTTP 404 : Not Found & HTTP 400 : Bad Request if errors occured.
    /// </returns>
    [ResponseType(typeof(void))]
    public IHttpActionResult PutShop(ShopSerializable updated) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }


      Shop shop = db.Shops.Find(updated.ShopId);
      if (shop == null) return BadRequest($"The Shop with {updated.ShopId} as their Id doesn't exist.");


      if (updated.OverpassId != null) {
        shop.OverpassId = (long)updated.OverpassId;
      }

      if (updated.Latitude != null) {
        shop.Latitude = (float)updated.Latitude;
      }

      if (updated.Longitude != null) {
        shop.Longitude = (float)updated.Longitude;
      }

      if (shop.Name.IsEmpty() == false) {
        shop.Name = updated.Name;
      }

      if (shop.Address.IsEmpty() == false) {
        shop.Address = updated.Address;
      }


      db.Entry(shop).State = EntityState.Modified;


      try {
        db.SaveChanges();
      }
      catch (DbUpdateConcurrencyException) {
        if (!ShopExists(updated.ShopId)) {
          return NotFound();
        }
        else {
          throw;
        }
      }


      return StatusCode(HttpStatusCode.NoContent);
    }


    // POST: api/ShopData
    /// <summary>
    /// Attempts to insert the specified Shop data into the database.
    /// </summary>
    /// <param name="payload">
    ///   This should be an object with the following schema :
    ///   {
    ///     "OverpassId" : int?,
    ///     "Name" : string,
    ///     "Address" : string,
    ///     "Latitude" : float,
    ///     "Longitude" : float,
    ///   }
    /// </param>
    /// <returns>
    /// HTTP 200 (OK) along with a Serializable Shop if the insert was successful.
    /// HTTP 400 (Bad Request) if there were errors with the insert.
    /// </returns>
    [ResponseType(typeof(ShopSerializable))]
    public IHttpActionResult PostShop(ShopSerializable payload) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }


      //SELF : the `.Add()` method does not overwrite existing entries. If elements are deleted
      //from the database table, autoincrement will still increment from the previous value.
      //This method also doesn't allow setting the primary key.

      //This method should avoid creating duplicate entries, it should check the database for
      //an existing `ShopOverpassId` first. If it detects one, it should fail.
      Shop existing_shop = db.Shops
        .Where(s => s.OverpassId == payload.OverpassId)
        .SingleOrDefault();


      if (existing_shop != null) {
        return BadRequest($"Shop with an OverpassId of {payload.OverpassId} already exists.");
      }


      Shop new_shop = db.Shops.Add(new Shop {
        ShopId = -1,
        OverpassId = (long)payload.OverpassId,
        Name = payload.Name,
        Address = payload.Address,
        Latitude = (float)payload.Latitude,
        Longitude = (float)payload.Longitude,
        Prices = new List<Price>(),
      });


      db.SaveChanges();


      return Ok(new_shop.ToSerializable());
    }


    // DELETE: api/ShopData?id={SHOP_ID}
    /// <summary>
    /// Attempts to Delete the Shop associated with the specified id from the database.
    /// </summary>
    /// <param name="id">
    /// The integer primary key associated with the Shop.
    /// </param>
    /// <returns>
    /// HTTP 200 (OK) with a Serializable Shop object of the Shop deleted.
    /// HTTP 404 (Not Found) if the specified id is not associated with a Shop.
    /// </returns>
    [ResponseType(typeof(ShopSerializable))]
    public IHttpActionResult DeleteShop([FromUri] int id) {
      Shop shop = db.Shops.Find(id);
      if (shop == null) {
        return NotFound();
      }

      db.Shops.Remove(shop);
      db.SaveChanges();

      return Ok(shop.ToSerializable());
    }


    protected override void Dispose(bool disposing) {
      if (disposing) {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool ShopExists(int id) {
      return db.Shops.Count(e => e.ShopId == id) > 0;
    }
  }
}