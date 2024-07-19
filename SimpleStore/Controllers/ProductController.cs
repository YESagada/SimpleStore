using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SimpleStore.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly List<Product> products = new List<Product>
        {
            new Product { Id = 1, Name = "Ноутбук", Price = 1000, Description = "Мощный ноутбук для работы и игр" },
            new Product { Id = 2, Name = "Смартфон", Price = 500, Description = "Современный смартфон с отличной камерой" },
            new Product { Id = 3, Name = "Наушники", Price = 200, Description = "Беспроводные наушники с хорошим звуком" }
        };

        // Метод для получения корзины из сессии
        private Cart GetCart()
        {
            var cart = HttpContext.Session.GetString("Cart");
            return cart == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(cart);
        }

        // Метод для сохранения корзины в сессию
        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }

        // GET: Product
        public IActionResult Index()
        {
            return View(products);
        }

        // GET: Product/Details/5
        public IActionResult Details(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/AddToCart/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCart();
            cart.AddItem(product, 1);
            SaveCart(cart);

            // Для отладки: Выводим содержимое корзины в консоль
            foreach (var item in cart.Items)
            {
                Console.WriteLine($"Товар: {item.Product.Name}, Количество: {item.Quantity}");
            }

            // Возвращаем представление корзины с обновленным содержимым
            return RedirectToAction("Cart");
        }

        // GET: Product/Cart
        public IActionResult Cart()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Дополнительно: методы для добавления, редактирования и удаления продуктов

        // Добавление продукта
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                products.Add(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Редактирование продукта
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(Product product)
        {
            var existingProduct = products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        // Удаление продукта
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
