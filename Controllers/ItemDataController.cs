using n01629177_passion_project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace n01629177_passion_project.Controllers {
  public class ItemDataController : ApiController {
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: api/ItemData
    public IEnumerable<ItemDto> GetItems() {
      var items = db.Items.AsEnumerable();
      var item_dtos = new List<ItemDto>();


      foreach (var i in items) {
        item_dtos.Add(i.ToDto());
      }


      return item_dtos;
    }


    // GET: api/ItemData?id={ITEM_ID}
    public ItemDto GetItem([System.Web.Http.FromUri] int id) {
      Item item = db.Items.Find(id);


      if (item == null) {
        return null;
      }


      return item.ToDto();
    }


    // GET : api/ItemData?search={SEARCH_STRING}
    public List<ItemDto> GetItems([FromUri] string search) {

      //Trim the search string and normalize the search string to a lowercase value.
      string search_string = search == null ? "" : search.Trim().ToLower();


      //Try to search for the entire search string first.
      var results = db.Items
        .Where(i => (
          i.ItemName.Contains(search_string) ||
          i.ItemBrand.Contains(search_string) ||
          i.ItemVariant.Contains(search_string)
        ));

      List<ItemDto> results_dto = new List<ItemDto>();
      foreach (var r in results) {
        results_dto.Add(r.ToDto());
      }



      //Check if any results were found using the initial string.
      // if(initial_search_results.Count() == 0) {

      //   //TODO: If no results were found, try to see if the search string can be
      //   //split into individual tokens and try to search for those tokens
      //   //instead.
      // }


      return results_dto;
    }

    // PUT: api/ItemData/5
    [ResponseType(typeof(void))]
    public IHttpActionResult PutItem(int id, Item item) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      if (id != item.ItemId) {
        return BadRequest();
      }

      db.Entry(item).State = EntityState.Modified;

      try {
        db.SaveChanges();
      }
      catch (DbUpdateConcurrencyException) {
        if (!ItemExists(id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // POST: api/ItemData
    [ResponseType(typeof(Item))]
    public IHttpActionResult PostItem(Item item) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      db.Items.Add(item);
      db.SaveChanges();

      return CreatedAtRoute("DefaultApi", new { id = item.ItemId }, item);
    }

    // DELETE: api/ItemData/5
    [ResponseType(typeof(Item))]
    public IHttpActionResult DeleteItem(int id) {
      Item item = db.Items.Find(id);
      if (item == null) {
        return NotFound();
      }

      db.Items.Remove(item);
      db.SaveChanges();

      return Ok(item);
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