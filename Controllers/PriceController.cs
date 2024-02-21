using Microsoft.AspNet.Identity;
using n01629177_passion_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace n01629177_passion_project.Controllers {
  public class PriceController : Controller {
    // GET: Price
    public ActionResult Index() {
      return View();
    }

    // GET: Price/Details/5
    public ActionResult Details(int id) {
      return View();
    }




    // GET: Price/CreateInput?itemId={ITEM_ID}
    [HttpGet]
    public ActionResult CreateInput([System.Web.Http.FromUri] int itemId) {

      ViewBag.itemId = itemId;

      return View();
    }




    // POST: Price/CreateProcess
    [HttpPost]
    public ActionResult CreateProcess(FormCollection form) {

      try {
        using (var http = new HttpClient()) {

          //Construct the base URi.
          string base_uri = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Authority}";


          int? shop_id = null;
          //First check to see if the shop exists in the database already. If
          //not, try to create the shop with the information provided.
          HttpResponseMessage response = http.GetAsync($"{base_uri}/api/ShopData?id={form["shopOverpassId"]}&isOverpass=true").Result;
          if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {

            //Then we try to create the shop...
            ShopSerializable shop_payload = new ShopSerializable {
              OverpassId = long.Parse(form["shopOverpassId"]),
              Name = form["shopName"],
              Address = form["shopAddress"],
              Latitude = float.Parse(form["shopLatitude"]),
              Longitude = float.Parse(form["shopLongitude"]),
            };


            //Make a request to create this shop in the database. 
            response = http.PostAsync($"{base_uri}/api/ShopData", shop_payload.ToHttpContent()).Result;


            if (response.StatusCode == System.Net.HttpStatusCode.OK) {

              //Successfully created the shop entry...
              shop_id = response.Content.ReadAsAsync<ShopSerializable>().Result.ShopId;
            }
            else {

              //Failed to create the shop entry...
              return Content(response.StatusCode.ToString());
            }
          }
          else if (response.StatusCode == System.Net.HttpStatusCode.OK) {

            //Shop already exists and was found in the database...
            shop_id = response.Content.ReadAsAsync<ShopSerializable>().Result.ShopId;
          }


          //Now that we have the shop_id and the item_id, we just need to get the
          //user_id and create the payload.
          string user_id = User.Identity.GetUserId();
          int item_id = item_id = int.Parse(form["itemId"]);


          //First check if this user_id is already associated with an existing 
          //price record for the same item and shop id. If it is, we need to
          //remove their attestation from that record before creating this new one.





          PriceSerializable price_payload = new PriceSerializable {
            ShopId = shop_id,
            UserId = user_id,
            ItemId = item_id,
            Value = float.Parse(form["price"]),
            //UnitPrice = unit_price,
            //Type = price_type,
          };


          response = http.PostAsync($"{base_uri}/api/PriceData", price_payload.ToHttpContent()).Result;
          if (response.StatusCode != System.Net.HttpStatusCode.OK) {

            //Failed to create Price in the database...
            return Content(response.StatusCode.ToString());
          }

        }

        return RedirectToAction("Details", "Item", new { itemId = form["itemId"] });
      }
      catch (Exception e) {
        return Content(e.Message);
      }
    }



    // GET: Price/Reattest?priceId={PRICE_ID}
    [HttpGet]
    public ActionResult Reattest([System.Web.Http.FromUri] int priceId) {

      using (var http_client = new HttpClient()) {
        string base_uri = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Authority}";

        PriceSerializable updated_price = new PriceSerializable {
          PriceId = priceId,
          UserId = User.Identity.GetUserId(),
          Reattest = true,
        };

        HttpResponseMessage response = http_client.PutAsync($"{base_uri}/api/PriceData", updated_price.ToHttpContent()).Result;
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) {

          //Redirect to the home page if the status code indicates a successful update.
          return RedirectToAction("Index", "Home");
        }
        else {
          return Content(response.StatusCode.ToString());
        }
      }
    }




    // GET: Price/Edit/5
    public ActionResult Edit(int id) {
      return View();
    }

    // POST: Price/Edit/5
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

    // GET: Price/Delete/5
    public ActionResult Delete(int id) {
      return View();
    }

    // POST: Price/Delete/5
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
