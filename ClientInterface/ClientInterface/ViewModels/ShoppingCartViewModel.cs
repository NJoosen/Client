using System.Collections.Generic;
using ClientInterface.Models;
using System.ComponentModel.DataAnnotations;

namespace ClientInterface.ViewModels
{
    public class ShoppingCartViewModel
    {
        [Key]
        public int id { get; set; }

        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}