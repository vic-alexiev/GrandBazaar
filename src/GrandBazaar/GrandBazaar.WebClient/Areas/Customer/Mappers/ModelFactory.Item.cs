using GrandBazaar.Domain.Models;
using GrandBazaar.WebClient.Areas.Customer.Models;
using System.Collections.Generic;
using System.Linq;
using Image = GrandBazaar.WebClient.Areas.Customer.Models.Image;

namespace GrandBazaar.WebClient.Areas.Customer.Mappers
{
    public static class ModelFactory
    {
        public static ItemViewModel ToViewModel(this Item model, int stock = 0)
        {
            return new ItemViewModel
            {
                Id = model.Id,
                IpfsHash = model.IpfsHash,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = stock,
                Images = model.Images.Select(image => new Image
                {
                    Name = image.Name,
                    DataBase64Encoded = image.DataBase64Encoded
                }).ToList()
            };
        }

        public static List<ItemViewModel> ToViewModels(this List<Item> models)
        {
            return models.Select(model => model.ToViewModel()).ToList();
        }
    }
}
