using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderApi.Infrastructure.Exceptions;

namespace OrderApi.Models
{
    public class OrderItem
    {
        protected OrderItem()
        {
        }

        public OrderItem(int productId, string productName, decimal unitPrice, string pictureUrl, int units = 1)
        {
            if (units <= 0) throw new OrderingDomainException("Invalid number of units");

            ProductId = productId;

            ProductName = productName;
            UnitPrice = unitPrice;

            Units = units;
            PictureUrl = pictureUrl;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public decimal UnitPrice { get; set; }

        public int Units { get; set; }
        public int ProductId { get; }
        public Order Order { get; set; }
        public int OrderId { get; set; }

        public void SetPictureUri(string pictureUri)
        {
            if (!string.IsNullOrWhiteSpace(pictureUri)) PictureUrl = pictureUri;
        }


        public void AddUnits(int units)
        {
            if (units < 0) throw new OrderingDomainException("Invalid units");

            Units += units;
        }
    }
}