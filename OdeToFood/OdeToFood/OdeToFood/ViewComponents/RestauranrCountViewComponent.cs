using Microsoft.AspNetCore.Mvc;
using OdeToFood.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdeToFood.ViewComponents
{
    public class RestauranrCountViewComponent:ViewComponent
    {
        private readonly IRestaurantData restaurantData;

        public RestauranrCountViewComponent(IRestaurantData restaurantData)
        {
            this.restaurantData = restaurantData;
        }
        public IViewComponentResult Invoke()
        {
            var count = restaurantData.GetCountOfRestaurants();
            return View(count);
        }
    }
}
