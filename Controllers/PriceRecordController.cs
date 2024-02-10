using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using n01629177_passion_project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace n01629177_passion_project.Controllers {


  public class PriceRecordController : Controller {

    //private readonly UserManager<ApplicationUser> _UserManager;

    //public PriceRecordController(UserManager<ApplicationUser> user_manager) {
    //  _UserManager = user_manager;
    //}


    // GET: PriceRecord
    public ActionResult Index() {
      return View();
    }

    // GET: PriceRecord/Details/5
    public ActionResult Details(int id) {
      return View();
    }

    // GET: PriceRecord/CreateInput?itemId={ITEM_ID}
    [HttpGet]
    public ActionResult CreateInput([System.Web.Http.FromUri] int itemId) {

      //Pass the following values to the View to be used.
      ViewBag.itemId = itemId;

      return View();
    }


    // POST: PriceRecord/CreateProcess
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId">The integer id of the item to create a price record for.</param>
    /// <param name="form_data">
    ///   The form data associated with the POST request. The expected schema for the form data is :
    ///   {
    ///     "price" : float,
    ///     "unitAmount" : float,
    ///     "unit" : int - corresponds to the following (1=grams, 2=units),
    ///     "itemId" : int,
    ///     "shopOverpassId" : int,
    ///     "shopName" : string,
    ///     "shopAddress" : string,
    ///     "shopLatitude" : float,
    ///     "shopLongitude" : float,
    ///   }
    /// </param>
    /// <returns>Nothing, redirects back to the Item Page.</returns>

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateProcess(FormCollection form_data) {
      using (var http_client = new HttpClient()) {

        //Construct the base URi.
        string base_uri = $"{HttpContext.Request.Url.Scheme}://{HttpContext.Request.Url.Authority}";


        try {

          string user_id = User.Identity.GetUserId();
          int item_id = item_id = int.Parse(form_data["itemId"]);
          int? shop_id = null;


          //Check to see if the shopId exists in the database already. If not, create 
          //the shop with the information provided.
          HttpResponseMessage response = await http_client.GetAsync($"{base_uri}/api/ShopData?shopOverpassId={form_data["shopOverpassId"]}");
          if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {

            //This shop isn't cached in the system yet.

            //So, we try to create the entry in the database.
            var shop_post_payload = new ShopPostPayload {
              ShopOverpassId = long.Parse(form_data["shopOverpassId"]),
              ShopName = form_data["shopName"],
              ShopAddress = form_data["shopAddress"],
              ShopLatitude = float.Parse(form_data["shopLatitude"]),
              ShopLongitude = float.Parse(form_data["shopLongitude"]),
            };


            //Make a request to create this shop in the database. 
            response = await http_client.PostAsync($"{base_uri}/api/ShopData", new StringContent(
              JsonConvert.SerializeObject(shop_post_payload),
              Encoding.UTF8,
              "application/json"
            ));


            if (response.StatusCode == System.Net.HttpStatusCode.OK) {

              //If the attempt to add the shop into the database succeeds, obtain and store the shop id.
              string response_string = await response.Content.ReadAsStringAsync();
              var response_shop = JsonConvert.DeserializeObject<Shop>(response_string);
              shop_id = response_shop.ShopId;

            }
            else {

              //If the attempt to add the shop into the database fails, abort the 
              //price entry creation process and report the error to the user.
              return Content(response.StatusCode.ToString());
            }



          }
          else {

            //This shop exists already in the database, so we can just extract the shop id 
            //from the response.
            string response_string = await response.Content.ReadAsStringAsync();
            shop_id = JsonConvert.DeserializeObject<Shop>(response_string).ShopId;
          }


          //By this point, we should have the shopId along with the itemId to associate to this
          //price record. Now we create the price Record entry.

          BasePriceRecordPostPayload payload = new BasePriceRecordPostPayload {
            UserId = user_id,
            ShopId = (int)shop_id,
            ItemId = item_id,
            BasePrice = float.Parse(form_data["price"]),
            CreationDate = DateTime.Now,
            LastAttestationDate = DateTime.Now,
          };


          response = await http_client.PostAsync($"{base_uri}/api/PriceRecordData", payload.ToHttpContent());


          if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) {
            return Content(response.StatusCode.ToString());
          }

        }
        catch (Exception e) {
          return Content(e.Message);
        }
      }


      return RedirectToAction("Details", "Item", new { itemId = form_data["itemId"] });
    }

    // GET: PriceRecord/Edit/5
    public ActionResult Edit(int id) {
      return View();
    }

    // POST: PriceRecord/Edit/5
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

    // GET: PriceRecord/Delete/5
    public ActionResult Delete(int id) {
      return View();
    }

    // POST: PriceRecord/Delete/5
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
