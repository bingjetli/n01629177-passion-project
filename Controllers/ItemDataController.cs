﻿using n01629177_passion_project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace n01629177_passion_project.Controllers {
  public class ItemDataController : ApiController {
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: api/ItemData
    public IQueryable<Item> GetItems() {
      return db.Items;
    }

    // GET: api/ItemData/5
    [ResponseType(typeof(Item))]
    public IHttpActionResult GetItem(int id) {
      Item item = db.Items.Find(id);
      if (item == null) {
        return NotFound();
      }

      return Ok(item);
    }

    // GET : api/ItemData?search={SEARCH_STRING}
    public IHttpActionResult GetItems([FromUri] string search) {

      //Trim the search string and normalize the search string to a lowercase value.
      string search_string = search == null ? "" : search.Trim().ToLower();


      //Try to search for the entire search string first.
      IEnumerable<Item> initial_search_results = db.Items
        .Where(i => (
          i.ItemName.Contains(search_string) ||
          i.ItemBrand.Contains(search_string) ||
          i.ItemVariant.Contains(search_string)
        ));


      //Return HTTP 200 and the search results.
      return Ok(initial_search_results);
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