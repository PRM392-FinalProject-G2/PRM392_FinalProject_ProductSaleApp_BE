using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductSaleApp.Service.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
