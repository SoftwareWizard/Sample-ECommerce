namespace ShoesOnContainers.Web.WebMvc.ViewModels
{
    public class CartComponentViewModel
    {
        public int ItemsInCart { get; set; }
        public decimal TotalCost { get; set; }
        public string Disabled => ItemsInCart == 0 ? "is-disabled" : "";
    }
}