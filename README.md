# Overview
&nbsp;
This project aims to create a full stack application that allows users to create individual grocery items and price data all stored within a central database. This data can be accessed by other users of the application and price data for each item is aggregated from the price with the most attestations. As more users create and attest to prices in the database, eventually it will develop into a self-validating community driven database of grocery items and prices.

&nbsp;
&nbsp;

# Database Structure
## Items
 - ItemId : int, primary key;
 - Brand : string;
 - Name : string;
 - Variant : string; for example milk has 0%, 1%, 2% and 3.5%
 - PriceType : string, either "weight" or "item"; describes whether the item is sold by "weight" or sold by individual "item"s
 - DefaultQuantity : int;
 - Prices : Price[]; describes a collection of Prices associated with this item

&nbsp;
&nbsp;

## Shops
 - ShopId : int, primary key;
 - OverpassId : long; the id of the OverPass Node
 - Latitude : float; 
 - Longitude : float;
 - Name : string;
 - Address : string;
 - Prices : Price[]; describes a collection of Prices associated with this shop

&nbsp;
&nbsp;

## Prices
- PriceId : int, primary key;
- CreationDate : DateTime, automatically created;
- LastAttestationDate : DateTime; describes the last time an update was made to this item's list of users
- Value : float; describes the actual 'price' that this entry is associated with
- ItemId : int, foreign key;
- ShopId : int, foreign key;
- Users : ApplicationUser[]; describes the collection of users that attested to this price record

&nbsp;
&nbsp;

## Relationships
 - An *Item* can have many *Prices* associated with it.
 - A *Shop* can also have many *Prices* associated with it.
 - Each *Price* can only be associated with a single *Item* and *Shop*.
 - Each *Price* can be attested by many *ApplicationUsers*.
 - Each *ApplicationUser* can attest to many *Prices*.

&nbsp;
&nbsp;

# Extra Features
## Automated Shop Entry Insertion using OverPass API and GeoLocation
This project leverages the user's location data and OpenStreetMap's node data to automate the way *Shop* data is populated.

First the user's location data is obtained from the browser which then makes an API call to OverPass (an API that handles queries to OpenStreetMap data) to retreive the list of nodes tagged as a "supermarket" back to the browser. These nodes describes the list of *Shops* within a specified radius of the user's location.

Then when the request is made to create a new *Price*, the controller first checks to see if there is already a *Shop* with the specified OverpassId in the database, before creating a new *Shop* using the data sent along with the creation request and associating it with the new *Price* record.

&nbsp;
&nbsp;

## Optimized API calls
Originally envisioned as a way to solve the problem of auto-filling the missing address data obtained from OverPass nodes. A separate API for reverse GeoCoding (which retreives a human readable address given latitude and longitude) was planned to be impemented. This separate API had a rate limit of 1 request per second and so making repeated bulk calls to this API was not allowed.

The idea is then to first cache the *Shop* data obtained from the OverPass API, then run a background service (or on demand) to update the missing address data through the reverse GeoCoding API.

From there, it developed into an overengineered solution where the browser keeps cached *Shop* data in session storage to avoid making repeated calls to the ASP.NET Application Server and only makes a request to update this data when it expires or the search bounds exceed the cached data. Then the ASP.NET Application Server will first check its own locally cached *Shop* data stored inside the SQL database and attempt to return that to the user, only making new requests to the API if there were missing chunks inside the requested search bounds.


> As of Feb 24, 2024, only the main server-caching phase is implemented. This means there is no session storage caching from the browser yet and there is no periodic/on-demand reverse geocoding from the main server.

&nbsp;
&nbsp;

# Challenges
## Making the database self-validating
One of the main obstacles of this idea was designing a way for this database to be self-validating. In other words, if I find an item in the database, how can I be sure that the price listed is reliable and true?

The solution I came up with was to first design an app that the user can use without being dependent on the existing data. (The shopping list calculator / tracker) From there, every user would create their own entry and input their own version of the ground truth since a legitimate user would not input false information. 

Eventually the database will have a collection of *Prices* and *Items* that it can use to aggregate. Now, when a user looks to enter a new item they haven't entered before, the system may already have that data from another user's input.

If the user can see that the data is valid, they can just proceed to adding that item into their shopping list which will automatically create an *Attestation* to this *Price* and *Item* record for the user.If the price data is not valid, the user then has an option to enter their own version of the ground truth with a new price record which other users can now attest to.

When scaled up to many legitmate users, the database will eventually become self-validating.

&nbsp;
&nbsp;

## The OverPass Query Language
[Overpass API Wiki](https://wiki.openstreetmap.org/wiki/Overpass_API)

Writing the logic was straightforward given the documentation, but I had difficulties with the query syntax and structure since the API doesn't give very descriptive errors and queries can just straight up fail without much feedback.

&nbsp;
&nbsp;

## "That dang lazy loading bug"
Inside the `PriceDataController`, there is a route that handles `GetAllPrices` which is supposed to just retreive the list of all the price records in the database as serialized Price objects.

This method kept failing with an exception that read :

```
Cannot deserialize the current JSON object (e.g. {"name":"value"}) into type 'System.Collections.Generic.IEnumerable`1[n01629177_passion_project.Models.PriceSerializable]' because the type requires a JSON array (e.g. [1,2,3]) to deserialize correctly.
```

The functions and models used to handle this was almost identical to the ones used to handle *Item* and *Shop* which worked perfectly. So I was stuck figuring out why this kept failing seemingly out of the blue.

My `.ToSerializable` method works perfectly when used to aggregate each item's price data in the Item Details page, so I eliminated that as a potential problem, especially since the method is also a fairly simple return statement that does nothing out of the ordinary.

The exception indicated an error involved with deserializing a JSON object, but that wasn't helpful since it just tells me that there was an error that occured in my http-client response but not why or where the error occured.

Eventually I discovered that using `public virtual` in my *Prices* model indicates that the property should be lazily loaded by ASP.NET. Which means when I tried to serialize my `ICollection<ApplicationUser<`, the pointer to the collection is there, but the actual data hasn't been loaded yet. So that was causing my `ToSerializable` method to fail, which returned an invalid JSON object in the HTTP response causing the "deserializing error".

The solution was to call `.Include(p => p.Users)` to eager-load my `ICollection<ApplicationUser>` before serializing it with `.ToSerializable()`.

&nbsp;
&nbsp;
&nbsp;
&nbsp;

# Changelog

## 0000-20240223 :
 - Fixed Lazy loading bug in `GetAllPrices`
 - Added Items page to view all items in the database
 - Added Items modifiable details to view item datails from that page
 - Added Prices page to view all pages in the database
 - Added Prices modifiable details to view price details from that page
 - Began implementing server-cached shop data in `ShopDataController`

## 2243-20240221 :
 - Shopping list functionality implemented
 - Items can be removed from the shopping list
 - Changing the item quantity in the shopping list recalculates the total
 - System now handles items with only 1 price correctly
 - Create item user flow implemented


## 2359-20240220 :
 - Users can no longer attest to multiple prices of the same item.
 - There is now an option to set item quantity in the details page.
 - Toast notifications are now added to indicate when an item has been added to a list.
 - Users can now add items to a shopping list or a planning list.
 - Alternate prices can now be selected and the user will automatically attest to these prices when an item is added to a list.


## 1400-20240219 :
 - item table now stores price type and default quantity
 - other associated prices only renders if there are multiple prices for the item
 - removed quantity and unit from the create price record page
 - item now displays default quantity and unit in the details page


## 0000-20240218 :
 - implemented loading price data on the item details page
 - accordian list of selectable prices now visible on item details page
 - price serializable now stores the amount of attestations
 - serializables now have an option to be deep serialized which includes the IEnumerables

## ...untracked changes :
 - refactored BasePriceRecord into Prices
 - updated Shop model to include address field and removed variant field
 - finished the 'create new price record' handling process
 - removed weight field from the item model
 - refactored shops model and completed CRUD functionality for the webapi
 - refactored items model and completed CRUD functionality for the webapi
 - updated search results with bootstrap's listgroup component
 - standardized all pages with bootstrap

## 0948-20240206 :
 - Added changelog and Readme.md
 - Implemented ItemController
 - Implemented /Search?query= route for ItemController.
 - Implemented GET /api/ItemData?search= for ItemDataController.
 - Redesigned Home page View
 - Redesigned shared Layout View
 - Added /Item/Search View
