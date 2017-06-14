using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClientInterface.Models
{
    [MetadataType(typeof(OrderDetailMetaData))]
    public partial class OrderDetail
    {

    }

    public class OrderDetailMetaData
    {
        [Key, Column(Order = 0)]
        public int OrderID { get; set; }
        [Key, Column(Order = 1)]
        public int ProductID { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> Amount { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}