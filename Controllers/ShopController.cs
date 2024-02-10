//PLANNED OPTIMIZATIONS / NOTES :
//
//`ST_Distance_Sphere` is an SQL function that can be used to determine the
//distance between 2 geographical coordinates inside the SQL database.
//
//Given a point and a search radius, first convert that into a box of chunks
//That need to be queried.
//
//Now we can query the database using a range filter for points that fall
//inside this box of chunks.
//
//Then check the box of chunks for chunks that have data and aren't null. 
//And run a Graph QL Query to OverPass API for node data coresponding to
//those missing chunks.
//
//Assign the node data to their appropriate chunks, and insert these nodes
//into the cached map data.
//
//Return the data back to the client, and loop through each of the returned
//nodes for the search pattern and sort by distance.
//
//Node data should also be cached locally on the browser as session data.
//In the case of repeated queries of varying sizes.
//
//So basically the browser should first check the locally cached chunk data to
//see if the data has expired yet. First check all the locally cached chunks to
//see if they expired yet. Then if an expired chunk is found, query the server
//for the entire search window. Then the server checks it's own cached data and
//determines if it needs to update and returns the search window chunks.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace n01629177_passion_project.Controllers {
  public class ShopController : Controller {
    // GET: Shop
    public ActionResult Index() {
      return View();
    }

    // GET: Shop/Details/5
    public ActionResult Details(int id) {
      return View();
    }

    // GET: Shop/Create
    public ActionResult Create() {
      return View();
    }

    // POST: Shop/Create
    [HttpPost]
    public ActionResult Create(FormCollection collection) {
      try {
        // TODO: Add insert logic here

        return RedirectToAction("Index");
      }
      catch {
        return View();
      }
    }

    // GET: Shop/Edit/5
    public ActionResult Edit(int id) {
      return View();
    }

    // POST: Shop/Edit/5
    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection) {
      try {
        // TODO: Add update logic here

        return RedirectToAction("Index");
      }
      catch {
        return View();
      }
    }

    // GET: Shop/Delete/5
    public ActionResult Delete(int id) {
      return View();
    }

    // POST: Shop/Delete/5
    [HttpPost]
    public ActionResult Delete(int id, FormCollection collection) {
      try {
        // TODO: Add delete logic here

        return RedirectToAction("Index");
      }
      catch {
        return View();
      }
    }
  }
}
