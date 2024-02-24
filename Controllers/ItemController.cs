using Microsoft.AspNet.Identity;
using n01629177_passion_project.Models;
using n01629177_passion_project.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace n01629177_passion_project.Controllers {
  public class ItemController : Controller {
    // GET: Item
    public ActionResult Index() {
      using (var http_client = new HttpClient()) {

        //Construct the base URi and fetch the resource.
        string base_uri = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Authority}";
        HttpResponseMessage response = http_client.GetAsync($"{base_uri}/api/ItemData").Result;


        return View(response.Content.ReadAsAsync<IEnumerable<ItemSerializable>>().Result);
      }
    }

    // GET: Item/ModifiableDetails?itemId={ITEM_ID}
    [HttpGet]
    public ActionResult ModifiableDetails([System.Web.Http.FromUri] int itemId) {
      using (var http_client = new HttpClient()) {

        //Construct the base URi and fetch the resource.
        string base_uri = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Authority}";
        HttpResponseMessage response = http_client.GetAsync($"{base_uri}/api/ItemData?id={itemId}&includePrices=true").Result;


        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
          return Content(response.Content.ToString());
        }


        return View(response.Content.ReadAsAsync<ItemSerializable>().Result);
      }
    }


    // GET: Item/Details?itemId={ITEM_ID}
    [HttpGet]
    public ActionResult Details([System.Web.Http.FromUri] int itemId) {
      using (var http_client = new HttpClient()) {

        //Construct the base URi and fetch the resource.
        string base_uri = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Authority}";
        HttpResponseMessage response = http_client.GetAsync($"{base_uri}/api/ItemData?id={itemId}&includePrices=true").Result;


        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
          return Content(response.Content.ToString());
        }


        return View(response.Content.ReadAsAsync<ItemSerializable>().Result);
      }
    }


    // GET: Item/Create
    public ActionResult CreateInput() {
      return View();
    }


    [HttpPost]
    public ActionResult CreateProcess(ItemSerializable payload) {

      using (var http_client = new HttpClient()) {

        //Construct the base URi and fetch the resource.
        string base_uri = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Authority}";
        HttpResponseMessage response = http_client.PostAsync($"{base_uri}/api/ItemData", payload.ToHttpContent()).Result;


        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
          return Content(response.Content.ToString());
        }



        return Redirect($"/Item/Details?itemId={response.Content.ReadAsAsync<ItemSerializable>().Result.ItemId}");
      }

    }


    // GET: Item/Search?query={SEARCH_STRING}
    [HttpGet]

    //Unable to use the `using` keyword for `.FromUri` since that contains 
    //definitions for the annotations of the Http Verbs as well.
    public ActionResult Search([System.Web.Http.FromUri] string query) {

      //Redirect the user back to the home page if the search query is invalid.
      if (query == null || query.Length < 3) {
        return RedirectToAction("Index", "Home");
      }

      //The `using` keyword allows automatic disposal of resources.
      //This is similar to the `with` keyword in Python.
      //`var` can be used here to infer the type, similar to c++ `auto`.
      using (var http_client = new HttpClient()) {

        //Construct the base URI using the scheme and the authority which
        //should give the current host and port.
        string base_uri = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Authority}";


        HttpResponseMessage response = http_client.GetAsync($"{base_uri}/api/ItemData?search={query}").Result;

        return View(response.Content.ReadAsAsync<IEnumerable<ItemSerializable>>().Result);

      }
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
