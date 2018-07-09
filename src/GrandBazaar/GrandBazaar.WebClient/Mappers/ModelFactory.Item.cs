using GrandBazaar.Domain.Models;
using GrandBazaar.WebClient.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace GrandBazaar.WebClient.Mappers
{
    public static class ModelFactory
    {
        public static Item ToDomainModel(this ItemViewModel model, IFormFileCollection images)
        {
            Item item = new Item
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Quantity = model.Quantity,
                Images = new List<Image>()
            };

            foreach (IFormFile image in images)
            {
                if (image.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        image.CopyTo(stream);
                        item.Images.Add(new Image
                        {
                            Name = image.FileName,
                            DataBase64Encoded = Convert.ToBase64String(stream.ToArray())
                        });
                    }
                }
            }

            return item;
        }
    }
}
