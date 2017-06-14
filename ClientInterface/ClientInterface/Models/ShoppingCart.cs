using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientInterface.Models
{
    public partial class ShoppingCart
    {
        StoreEntities storeDB = new StoreEntities();
        string ShoppingCartID { get; set; }
        public const string CartSessionKey = "CartID";
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartID = cart.GetCartId(context);
            return cart;
        }
        //Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public void AddToCart(Product product)
        {
            // Get matching cart and product instances
            var cartItem = storeDB.Carts.SingleOrDefault(
                c => c.CartID == ShoppingCartID && c.ProductID == product.id);

            if(cartItem == null)
            {
                //Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    ProductID = product.id,
                    CartID = ShoppingCartID,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                storeDB.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart,
                // then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            storeDB.SaveChanges();
        }
        public int RemoveFromCart(int id)
        {
            //Get the cart
            var cartItem = storeDB.Carts.Single(cart => cart.CartID == ShoppingCartID && cart.RecordID == id);
            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = (int)cartItem.Count;
                }
                else
                {
                    storeDB.Carts.Remove(cartItem);
                }
                //Save changes
                storeDB.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = storeDB.Carts.Where(cart
               => cart.CartID == ShoppingCartID);

            foreach (var cartItem in cartItems)
            {
                storeDB.Carts.Remove(cartItem);
            }
            // Save changes
            storeDB.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return storeDB.Carts.Where(cart => cart.CartID == ShoppingCartID).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in storeDB.Carts
                          where cartItems.CartID == ShoppingCartID
                          select (int?)cartItems.Count).Sum();
            //return 0 if all entries are null
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            //Multiply product price by count of that product to get
            // the current price for each of those products in the cart
            // sum all product price totals to get the cart total
            decimal? total = (from cartItems in storeDB.Carts
                              where cartItems.CartID == ShoppingCartID
                              select (int?)cartItems.Count * cartItems.Product.Price).Sum();
            return total ?? decimal.Zero;
        }

        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            // Iterate over the items in the cart,
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductID = (int)item.ProductID,
                    OrderID = order.id,
                    Price = item.Product.Price,
                    Amount = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += ((int)item.Count * (int)item.Product.Price);

                storeDB.OrderDetails.Add(orderDetail);
            }
            // Save the order
            storeDB.SaveChanges();
            // Empty the shopping cart
            EmptyCart();
            //return the OrderId as the confirmation number
            return order.id;
        }

        //We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.GUID class
                    Guid tempCartId = Guid.NewGuid();
                    //Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDB.Carts.Where(
                c => c.CartID == ShoppingCartID);

            foreach (Cart item in shoppingCart)
            {
                item.CartID = userName;
            }
            storeDB.SaveChanges();
        }
    }
}