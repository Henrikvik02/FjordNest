namespace FjordNestPro.ViewModels
{
    
        public class ReviewViewModel
        {
            public int ReviewID { get; set; }

            public string Content { get; set; } = default!;

            public int Rating { get; set; }

            public string GuestID { get; set; } = default!;

            public int BookingID { get; set; }
        }
    }


