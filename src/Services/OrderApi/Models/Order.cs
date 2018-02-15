using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.Models
{
    public class Order
    {
        protected Order()
        {
            OrderItems = new List<OrderItem>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime OrderDate { get; set; }

        public string UserName { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string Address { get; set; }

        public string PaymentAuthCode { get; set; }

        // public Guid RequestId { get;  set; }
        public decimal OrderTotal { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }

    public enum OrderStatus
    {
        Preparing = 1,
        Shipped = 2,
        Delivered = 3
    }
}