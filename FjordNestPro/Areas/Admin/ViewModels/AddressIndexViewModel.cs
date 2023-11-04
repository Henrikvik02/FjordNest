using System;
using FjordNestPro.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FjordNestPro.Areas.Admin.ViewModels
{
    public class AddressIndexViewModel
    {
        public List<Address> Addresses { get; set; } = default!;
        public string FilterType { get; set; } = default!;
        public List<SelectListItem> FilterOptions { get; set; } = default!;
        public string CitySearchString { get; set; } = default!;
        public string PostalCodeSearchString { get; set; } = default!;
    }

}

