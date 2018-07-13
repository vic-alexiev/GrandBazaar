using GrandBazaar.Domain.Models;
using GrandBazaar.WebClient.Areas.Seller.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DomainImage = GrandBazaar.Domain.Models.Image;
using Image = GrandBazaar.WebClient.Areas.Seller.Models.Image;

namespace GrandBazaar.WebClient.Areas.Seller.Mappers
{
    public static class ModelFactory
    {
        public static Item ToDomainModel(this CreateItemViewModel model, IReadOnlyList<IFormFile> images)
        {
            Item item = new Item
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Images = new List<DomainImage>()
            };

            foreach (IFormFile image in images)
            {
                if (image.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        image.CopyTo(stream);
                        item.Images.Add(new DomainImage
                        {
                            Name = image.FileName,
                            DataBase64Encoded = Convert.ToBase64String(stream.ToArray())
                        });
                    }
                }
            }

            return item;
        }

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
