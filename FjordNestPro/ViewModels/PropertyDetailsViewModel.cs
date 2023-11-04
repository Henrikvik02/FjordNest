using FjordNestPro.Models;

namespace FjordNestPro.ViewModels
{
    public class PropertyDetailsViewModel
    {
        public int PropertyID { get; set; }

        public string UserID { get; set; }

        public string Title { get; set; }

        public string PropertyType { get; set; }

        public string Description { get; set; }

        public int MaxGuests { get; set; }

        public decimal PricePerNight { get; set; }

        public bool LongTermStay { get; set; }

        public string ImageUrl { get; set; }

        public AddressViewModel Address { get; set; }
        public ICollection<ReviewViewModel> Reviews { get; set; }


    }
}
