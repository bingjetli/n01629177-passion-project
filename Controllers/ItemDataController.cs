using n01629177_passion_project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Web.WebPages;

namespace n01629177_passion_project.Controllers {
  public class ItemDataController : ApiController {
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: api/ItemData
    /// <summary>
    /// Returns a generic collection of all the Items in the database as Serializable objects.
    /// </summary>
    /// <returns>
    /// HTTP 200 (OK) with an ICollection of ItemSerializables.
    /// </returns>
    [ResponseType(typeof(ICollection<ItemSerializable>))]
    public IHttpActionResult GetAllItems() {
      return Ok(db.Items.AsEnumerable().Select(i => i.ToSerializable()));
    }




    // GET: api/ItemData?id={ITEM_ID}
    /// <summary>
    /// Attempts to find and return a Serializable Item associated with the specified id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includePrices">
    /// A boolean flag indicating whether or not to include the list of prices
    /// associated with this item.
    /// </param>
    /// <returns>
    /// HTTP 200 (OK) with the ItemSerializable if an item is found.
    /// HTTP 404 (Not Found) otherwise.
    /// </returns>
    [ResponseType(typeof(ItemSerializable))]
    public IHttpActionResult GetItem(
        [FromUri] int id,
        [FromUri] bool includePrices = false
      ) {
      Item item = db.Items.Find(id);


      if (item == null) {
        return NotFound();
      }


      return Ok(item.ToSerializable(includePrices));
    }




    // GET : api/ItemData?search={SEARCH_STRING}
    /// <summary>
    /// Attempts to search the database for Items that contain the specified search term.
    /// The search checks the item's name, brand and variant to see if it contains
    /// the specified search term.
    /// </summary>
    /// <param name="search"></param>
    /// <returns>
    /// HTTP 200 (Ok) with an ICollection of ItemSerializables.
    /// </returns>
    [ResponseType(typeof(ICollection<ItemSerializable>))]
    public IHttpActionResult GetSearchResults([FromUri] string search) {

      //Trim the search string and normalize the search string to a lowercase value.
      string search_string = search == null ? "" : search.Trim().ToLower();


      return Ok(
        db.Items
          .Where(i => (
            i.Name.Contains(search_string) ||
            i.Brand.Contains(search_string) ||
            i.Variant.Contains(search_string)
          ))
          .AsEnumerable()
          .Select(i => i.ToSerializable())
        );
    }




    // PUT: api/ItemData
    /// <summary>
    /// Attempts to update the specified item in the database.
    /// 
    /// The ItemId must be included as part of the request.
    /// </summary>
    /// <param name="updated"></param>
    /// <returns>
    /// HTTP 204 (No Content) if the update was successful.
    /// HTTP 400 (Bad Request) and HTTP 404 (Not Found) Otherwise.
    /// </returns>
    [ResponseType(typeof(void))]
    public IHttpActionResult PutItem(ItemSerializable updated) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      Item item = db.Items.Find(updated.ItemId);
      if (item == null) {
        return BadRequest($"The Item with the following ItemId {updated.ItemId} was not found.");
      }


      if (updated.Name.IsEmpty() == false) {
        item.Name = updated.Name;
      }

      if (updated.Brand.IsEmpty() == false) {
        item.Brand = updated.Brand;
      }

      if (updated.Variant.IsEmpty() == false) {
        item.Variant = updated.Variant;
      }

      if (updated.PriceType.IsEmpty() == false) {
        item.PriceType = updated.PriceType;
      }


      if (updated.DefaultQuantity > 0) {
        item.DefaultQuantity = updated.DefaultQuantity;
      }


      db.Entry(item).State = EntityState.Modified;


      try {
        db.SaveChanges();
      }
      catch (DbUpdateConcurrencyException) {
        if (!ItemExists(updated.ItemId)) {
          return NotFound();
        }
        else {
          throw;
        }
      }


      return StatusCode(HttpStatusCode.NoContent);
    }




    // POST: api/ItemData
    /// <summary>
    /// Attempts to create a new item in the database with the specified payload.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns>
    /// HTTP 200 (Ok) with the newly created item if successful.
    /// HTTP 400 (Bad Request) otherwise.
    /// </returns>
    [ResponseType(typeof(ItemSerializable))]
    public IHttpActionResult PostItem(ItemSerializable payload) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }


      Item new_item = new Item {
        ItemId = 0,
        Brand = payload.Brand,
        Name = payload.Name,
        Variant = payload.Variant,
        PriceType = payload.PriceType,
        DefaultQuantity = payload.DefaultQuantity,
        Prices = new List<Price>(),
      };


      db.Items.Add(new_item);
      db.SaveChanges();


      return Ok(new_item.ToSerializable());
    }




    // DELETE: api/ItemData?id={ITEM_ID}
    /// <summary>
    /// Attempts to delete the Item associated with the specified ItemId from
    /// the database.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// HTTP 200 (Ok) along with the deleted Item if successful.
    /// HTTP 404 (Not Found) otherwise.
    /// </returns>
    [ResponseType(typeof(ItemSerializable))]
    public IHttpActionResult DeleteItem([FromUri] int id) {
      Item item = db.Items.Find(id);
      if (item == null) {
        return NotFound();
      }

      db.Items.Remove(item);
      db.SaveChanges();

      return Ok(item.ToSerializable());
    }


    protected override void Dispose(bool disposing) {
      if (disposing) {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool ItemExists(int id) {
      return db.Items.Count(e => e.ItemId == id) > 0;
    }
  }
}