using OdeToFood.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OdeToFood.Data
{
   public interface IRestaurantData
    {
        IEnumerable<Restaurant> GetRestaurantByName(string name);
        Restaurant GetById(int id);
        Restaurant Update(Restaurant updateRestaurant);
        Restaurant Add(Restaurant newRestaurant);
        Restaurant Delete(int Id);
        int GetCountOfRestaurants();
        int Commit();
    }

}
