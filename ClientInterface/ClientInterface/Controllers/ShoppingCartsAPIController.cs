using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using ClientInterface.Models;
using ClientInterface.ViewModels;



namespace ClientInterface.Controllers
{
    public class ShoppingCartsAPIController : ApiController
    {
        private StoreEntities storeDB = new StoreEntities();

        // GET: api/ShoppingCartsAPI
        public IQueryable<ShoppingCartViewModel> GetShoppingCartViewModels()
        {
            return storeDB.ShoppingCartViewModels;
        }

        // GET: api/ShoppingCartsAPI/5
        [ResponseType(typeof(ShoppingCartViewModel))]
        public IHttpActionResult GetShoppingCartViewModel(int id)
        {
            ShoppingCartViewModel shoppingCartViewModel = storeDB.ShoppingCartViewModels.Find(id);
            if (shoppingCartViewModel == null)
            {
                return NotFound();
            }

            return Ok(shoppingCartViewModel);
        }

        // PUT: api/ShoppingCartsAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShoppingCartViewModel(int id, ShoppingCartViewModel shoppingCartViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shoppingCartViewModel.id)
            {
                return BadRequest();
            }

            storeDB.Entry(shoppingCartViewModel).State = EntityState.Modified;

            try
            {
                storeDB.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShoppingCartViewModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ShoppingCartsAPI
        [ResponseType(typeof(ShoppingCartViewModel))]
        public IHttpActionResult PostShoppingCartViewModel(ShoppingCartViewModel shoppingCartViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            storeDB.ShoppingCartViewModels.Add(shoppingCartViewModel);
            storeDB.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = shoppingCartViewModel.id }, shoppingCartViewModel);
        }

        // DELETE: api/ShoppingCartsAPI/5
        [HttpGet]
        [Route("api/ShoppingCartsAPI/DeleteShoppingCartViewModel")]
        [ResponseType(typeof(ShoppingCartViewModel))]
        public IHttpActionResult DeleteShoppingCartViewModel(int id)
        {


            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(new HttpContextWrapper(HttpContext.Current));

            // Get the name of the product to display confirmation
            string productName = storeDB.Carts.Single(item => item.RecordID == id).Product.name;

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = productName + " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                storeDB.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShoppingCartViewModelExists(int id)
        {
            return storeDB.ShoppingCartViewModels.Count(e => e.id == id) > 0;
        }
    }
}