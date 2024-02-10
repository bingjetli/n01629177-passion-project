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
  public class ShopDataController : ApiController {
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: api/ShopData
    [ResponseType(typeof(ICollection<ShopDto>))]
    public IHttpActionResult GetShops() {
      ICollection<Shop> shops = db.Shops.ToList();
      ICollection<ShopDto> shops_dto = new List<ShopDto>();

      foreach (var s in shops) {
        shops_dto.Add(s.ToDto());
      }


      return Ok(shops_dto);
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
    [HttpGet]
    [ResponseType(typeof(ShopDto))]
    public IHttpActionResult GetShopByOverpassId([System.Web.Http.FromUri] long shopOverpassId) {

      //RESOLVED : Find out why the heck this fails to handle integers greater than 9 digits when using the
      //`int` datatype for the parameter. This shouldn't fail since int32 values range from [-2,147,483,647 to 2,147,483,647].
      //But for some reason `https://localhost:44320/api/ShopData?shopOverpassId=5355856076` fails, whilst
      // `https://localhost:44320/api/ShopData?shopOverpassId=535585607` works. 

      //As it turns out, 5 355 856 076 does in-fact exceed the max value for int32. The spacing was visually deceptive to me.

      Shop shop = db.Shops.Where(s => s.ShopOverpassId == shopOverpassId).FirstOrDefault();
      if (shop == null) {
        return NotFound();
      }

      return Ok(shop.ToDto());
    }


    // PUT: api/ShopData/5
    [ResponseType(typeof(void))]
    public IHttpActionResult PutShop(int id, Shop shop) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      if (id != shop.ShopId) {
        return BadRequest();
      }

      db.Entry(shop).State = EntityState.Modified;

      try {
        db.SaveChanges();
      }
      catch (DbUpdateConcurrencyException) {
        if (!ShopExists(id)) {
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
    /// 
    /// </summary>
    /// <param name="payload">
    ///   This should be an object with the following schema :
    ///   {
    ///     "ShopOverpassId" : int?,
    ///     "ShopName" : string,
    ///     "ShopAddress" : string,
    ///     "ShopLatitude" : double,
    ///     "ShopLongitude" : double,
    ///   }
    /// </param>
    /// <returns>
    /// HTTP OK if the insert was successful. HTTP BadRequest otherwise.
    /// </returns>
    [ResponseType(typeof(ShopDto))]
    public IHttpActionResult PostShop(ShopPostPayload payload) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }


      //SELF : the `.Add()` method does not overwrite existing entries. If elements are deleted
      //from the database table, autoincrement will still increment from the previous value.
      //This method also doesn't allow setting the primary key.

      //This method should avoid creating duplicate entries, it should check the database for
      //an existing `ShopOverpassId` first. If it detects one, it should fail.
      Shop existing_shop = db.Shops
        .Where(s => s.ShopOverpassId == payload.ShopOverpassId)
        .SingleOrDefault();

      if (existing_shop != null) {
        return BadRequest($"Shop with an id of {payload.ShopOverpassId} already exists.");
      }


      Shop new_shop = db.Shops.Add(new Shop {
        ShopId = -1,
        ShopOverpassId = (long)payload.ShopOverpassId,
        ShopName = payload.ShopName,
        ShopAddress = payload.ShopAddress,
        ShopLatitude = payload.ShopLatitude,
        ShopLongitude = payload.ShopLongitude
      });

      db.SaveChanges();


      return Ok(new_shop.ToDto());
    }


    // DELETE: api/ShopData/5
    [ResponseType(typeof(Shop))]
    public IHttpActionResult DeleteShop(int id) {
      Shop shop = db.Shops.Find(id);
      if (shop == null) {
        return NotFound();
      }

      db.Shops.Remove(shop);
      db.SaveChanges();

      return Ok(shop);
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