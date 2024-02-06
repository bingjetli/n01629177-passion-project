﻿using n01629177_passion_project.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace n01629177_passion_project.Controllers {
  public class ItemController : Controller {
    // GET: Item
    public ActionResult Index() {
      return View();
    }

    // GET: Item/Details/5
    public ActionResult Details(int id) {
      return View();
    }

    // GET: Item/Create
    public ActionResult Create() {
      return View();
    }


    // GET: Item/Search?query={SEARCH_STRING}
    [HttpGet]

    //Unable to use the `using` keyword for `.FromUri` since that contains 
    //definitions for the annotations of the Http Verbs as well.
    public async Task<ActionResult> Search([System.Web.Http.FromUri] string query) {

      //The `using` keyword allows automatic disposal of resources.
      //This is similar to the `with` keyword in Python.
      //`var` can be used here to infer the type, similar to c++ `auto`.
      using (var http_client = new HttpClient()) {

        //Construct the base URI using the scheme and the authority which
        //should give the current host and port.
        string base_uri = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Authority}";


        HttpResponseMessage response = await http_client.GetAsync($"{base_uri}/api/ItemData?search={query}");


        if (response.IsSuccessStatusCode == true) {
          var response_data = await response.Content.ReadAsAsync<IEnumerable<Item>>();


          return View(response_data);
        }
      }

      return View();
    }

    // POST: Item/Create
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

    // GET: Item/Edit/5
    public ActionResult Edit(int id) {
      return View();
    }

    // POST: Item/Edit/5
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

    // GET: Item/Delete/5
    public ActionResult Delete(int id) {
      return View();
    }

    // POST: Item/Delete/5
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