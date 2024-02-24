using n01629177_passion_project.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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


    [HttpPost]
    [ResponseType(typeof(IEnumerable<ShopSerializable>))]
    public IHttpActionResult GetAllShopsInBoundingBox(
      [System.Web.Http.FromUri] bool useOverpass,
      [System.Web.Http.FromBody] ShopLocationBounds bbox
    ) {


      //OverPass API Query Variables.
      string node_definition = "node[\"shop\"~\"supermarket\", i]";
      string query_header = "[out:json];\n" +
                            "(\n";
      string query_footer = ");\n" +
                            "out;\n";
      string overpass_endpoint = "https://overpass-api.de/api/interpreter";
      string overpass_query = null;

      //1. First, try to get a list of the cached shops already in the database.
      //   and check to see if there are any cached shops.
      IEnumerable<Shop> cached_shops = db.Shops.Where(s => (
          s.Latitude >= bbox.SouthWest.Latitude &&
          s.Latitude <= bbox.NorthEast.Latitude &&
          s.Longitude <= bbox.NorthEast.Longitude &&
          s.Longitude >= bbox.SouthWest.Longitude
      ));
      if (cached_shops.Count() > 0) {

        //2. There are cached shops to in the database, so see if the cached shops
        //   contain any missing chunks.
        List<float> cached_latitudes = new List<float>();
        List<float> cached_longitudes = new List<float>();
        foreach (Shop s in cached_shops) {

          //Chunks are now rounded up to the nearest .1 since rounding up to the 
          //nearest whole number in geographical coordinates is too large of an area.
          int lat_chunk = (int)s.Latitude * 10;
          int lon_chunk = (int)s.Longitude * 10;

          if (cached_latitudes.Contains(lat_chunk / 10) == false) {

            //Add this to the list of cached latitudes if it isn't already in there.
            cached_latitudes.Add(lat_chunk / 10);
          }


          if (cached_longitudes.Contains(lon_chunk / 10) == false) {

            //Add this to the list of cached longitudes if it isn't already in there.
            cached_longitudes.Add(lon_chunk / 10);
          }
        }

        //SELF: There's probably a better way to do this.

        //Now we'll have a list of all the latitudes and longitudes that was in the 
        //cached list of shops.
        List<ShopLocationBounds> missing_chunks = new List<ShopLocationBounds>();
        for (
            float lat = ((int)bbox.SouthWest.Latitude * 10) / 10;
            lat <= ((int)bbox.NorthEast.Latitude * 10) / 10;
            lat++
          ) {
          for (
            float lon = ((int)bbox.SouthWest.Longitude * 10) / 10;
            lon <= ((int)bbox.NorthEast.Longitude * 10) / 10;
            lon++
            ) {

            //Check if this chunk is in the cached latitudes and longitudes.
            if (cached_latitudes.Contains(lat) == false || cached_longitudes.Contains(lon) == false) {

              //This means that this is a missing chunk, so we should create the
              //bounding box for it.
              missing_chunks.Add(new ShopLocationBounds {
                NorthEast = new ShopLocationBounds.Coordinate {
                  Latitude = (float)(lat + 0.1),
                  Longitude = (float)(lon + 0.1),
                },
                SouthWest = new ShopLocationBounds.Coordinate {
                  Latitude = lat,
                  Longitude = lon,
                }
              });
            }
          }
        }


        //First, check if there are any missing chunks, if there are, we run a graphQL
        //query to update the data. Otherwise return the list of cached chunks to the
        //user.
        if (missing_chunks.Count() > 0) {

          //3. There are missing chunks, so loop through each of the missing
          //   chunks and build a specialized OverPass query string based on
          //   the missing chunks.
          StringBuilder query = new StringBuilder(query_header);
          foreach (var chunk in missing_chunks) {
            query.Append($"{node_definition}({chunk.ToOverPassBoundString()});\n");
          }
          query.Append(query_footer);
          overpass_query = query.ToString();

        }
        else {

          //4. There are no missing chunks, so return the cached data.
          return Ok(cached_shops.AsEnumerable().Select(cs => cs.ToSerializable()));
        }
      }
      else {

        //5. There aren't any cached shops, so build an OverPass API query with
        //   the entire bounding box for the request.
        StringBuilder query = new StringBuilder(query_header);
        query.Append($"{node_definition}({bbox.ToOverPassBoundString()});\n");
        query.Append(query_footer);
        overpass_query = query.ToString();
      }


      //At this point in time, `overpass_query` should not be null. If there were
      //no missing chunks, then the function should have returned with HTTP 200
      //and the cached data by now.

      //6. Try to send a request to the overpass API with the specified query.
      using (var http_client = new HttpClient()) {

        //Construct the base URi and fetch the resource.
        HttpResponseMessage response = http_client.PostAsync(
          overpass_endpoint,
          new StringContent(overpass_query, Encoding.UTF8, "text/plain")
        ).Result;


        // 7. Was the query successful?
        if (response.StatusCode != System.Net.HttpStatusCode.OK) {

          //8. The query was not successful, were there any cached shops?
          if (cached_shops.Count() > 0) {

            //10. There were cached shops, so return HTTP 200 with the cached
            //    shop data.
            return Ok(cached_shops.AsEnumerable().Select(cs => cs.ToSerializable()));
          }
          else {

            //9. There weren't any cached shops, so return HTTP 500 : failed to 
            //   shop data.
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
            return InternalServerError(new Exception("Error occured retreiving data from the OverPass API."));
          }
        }


        //11. The query was successful, so loop through each node and create a
        //    new shop inside the database for each node.
        string response_json_string = response.Content.ReadAsStringAsync().Result;
        JObject response_json = JObject.Parse(response_json_string);

        ICollection<ShopSerializable> added_shops = new List<ShopSerializable>();

        int element_length = response_json["elements"].Count();
        for (int i = 0; i < element_length; i++) {

          if (
            (string)response_json["elements"][i]["tags"]["name"] == null ||
            ((string)response_json["elements"][i]["tags"]["name"]).Count() < 3
          ) {
            continue;
          }


          ShopAddress address = new ShopAddress {
            StreetNumber = (string)response_json["elements"][i]["tags"]["addr:housenumber"],
            StreetName = (string)response_json["elements"][i]["tags"]["addr:name"],
            City = (string)response_json["elements"][i]["tags"]["addr:city"],
          };


          Shop new_shop = db.Shops.Add(new Shop {
            ShopId = -1,
            OverpassId = (long)response_json["elements"][i]["id"],
            Name = (string)response_json["elements"][i]["tags"]["name"],
            Address = address.ToString(),
            Latitude = (float)response_json["elements"][i]["lat"],
            Longitude = (float)response_json["elements"][i]["lon"],
            Prices = new List<Price>(),
          });


          added_shops.Add(new_shop.ToSerializable());


          //13. For each shop, check if this has valid address data.

          //14. If there isn't, add it to a queue of shops that need their addresses
          //    updated.


          db.SaveChanges();
        }


        //15. Return the list of cached shops if it exists, concatenated with
        //    the newly added data.
        if (cached_shops.Count() > 0) {
          return Ok(Enumerable.Concat<ShopSerializable>(
            cached_shops
              .AsEnumerable()
              .Select(cs => cs.ToSerializable()),
            added_shops
          ).ToList());
        }
        else {
          return Ok(added_shops);
        }

      }
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