# Changelog


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
