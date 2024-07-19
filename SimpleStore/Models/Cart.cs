using System.Collections.Generic;
using System.Linq;

namespace SimpleStore.Models
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items => items;

        public void AddItem(Product product, int quantity)
        {
            var existingItem = items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                items.Add(new CartItem { Product = product, Quantity = quantity });
            }
        }

        public void RemoveItem(Product product)
        {
            var existingItem = items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (existingItem != null)
            {
                items.Remove(existingItem);
            }
        }

        public decimal TotalValue => items.Sum(i => i.Product.Price * i.Quantity);

        public void Clear()
        {
            items.Clear();
        }
    }
}
