using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FjordNestPro.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public static class SeedData
{

    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        try
        {
            // Checkpoint 1 registrere DBcontext
            using var context = new FjordNestProDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<FjordNestProDbContext>>());

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "Database context is not available.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (userManager == null || roleManager == null)
            {
                throw new ArgumentNullException("User or Role manager service is not available.");
            }

            var random = new Random();
            var positiveReviews = new string[]
            {
            "Great stay! Loved the place.",
            "The view was magnificent. Would definitely return.",
            "Best vacation ever. The property was amazing!",
            "Truly a home away from home.",
            "Clean, comfortable, and close to nature. Loved every moment.",
            "Top-notch service. Felt like royalty!",
            "The atmosphere was peaceful, just what I needed.",
            "Impressed by the hospitality and location.",
            "Waking up to that view was the highlight of my trip.",
            "Every amenity I needed was available.",
            "Charming decor and very spacious!",
            "Couldn't have chosen a better place for my getaway.",
            "Seamless check-in and check-out experience.",
            "The surroundings were breathtaking.",
            "Felt safe and well taken care of during my stay.",
            "The perfect blend of comfort and luxury.",
            "Every corner had a unique touch.",
            "Memories made here will last a lifetime.",
            "Fantastisk opphold! Elsket hvert øyeblikk.",
            "Utsikten var utrolig. Kommer definitivt tilbake.",
            "Beste ferien noensinne. Eiendommen var imponerende!",
            "Følte meg virkelig som hjemme.",
            "Rent, komfortabelt og nær naturen.",
            "Utmerket service. Følte meg som kongelig!",
            "Så avslappende å våkne opp her hver morgen.",
            "Alt jeg trengte var tilgjengelig.",
            "Dekorasjonen var sjarmerende og rommet var romslig!",
            "Kunne ikke valgt et bedre sted for min ferie.",
            "Innsjekking og utsjekking gikk som en drøm.",
            "Opplevelsene her vil jeg huske for alltid."
            };

            // Checkpoint 2 Makes roles 
            string[] roles = new[] { "ADMIN", "USER" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    Console.WriteLine($"Creating role: {role}");
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"Failed to create role: {role}");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Role {role} already exists.");
                }
            }


            // Checkpoint 3 Makes Admin
            if (!await userManager.Users.AnyAsync(u => u.UserName == "admin@domain.com"))
            {
                Console.WriteLine("Creating admin user.");
                var admin = new ApplicationUser { Id = "ADMIN", UserName = "admin@domain.com", Email = "admin@domain.com" };
                var result = await userManager.CreateAsync(admin, "Admin123!"); // Password should be stored securely or configured
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "ADMIN");
                }
                else
                {
                    Console.WriteLine("Failed to create admin user.");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }

            // Makes Users
            for (int i = 1; i <= 40; i++)
            {
                var email = $"user{i}@domain.com";
                if (await userManager.FindByEmailAsync(email) == null) // Check if user exists
                {
                    var user = new ApplicationUser
                    {
                        Id = $"{i}",
                        UserName = email,
                        NormalizedUserName = email.ToUpper(),
                        Email = email,
                        NormalizedEmail = email.ToUpper(),
                    };

                    var password = $"User1{i}23!";
                    var createUserResult = await userManager.CreateAsync(user, password);  // Directly use the password parameter

                    if (createUserResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "USER");
                    }
                    else
                    {
                        // Log the user creation error.
                        foreach (var error in createUserResult.Errors)
                        {
                            Console.WriteLine($"User creation error for {email}: {error.Description}");
                        }
                    }
                }
            }

            await context.SaveChangesAsync();  // Commit the changes to the database.


            // Check if any addresses already exist
            if (!context.Addresses.Any())
            {
                try
                {
                    var addresses = new List<Address>
                    {
                        new Address { AddressID = 1, StreetName = "Karl Johans gate 1", City = "Oslo", Postcode = "0154" },
                        new Address { AddressID = 2, StreetName = "Olav Kyrres gate 49", City = "Bergen", Postcode = "5015" },
                        new Address { AddressID = 3, StreetName = "Munkegata 1", City = "Trondheim", Postcode = "7013" },
                        new Address { AddressID = 4, StreetName = "Kongens gate 10", City = "Stavanger", Postcode = "4005" },
                        new Address { AddressID = 5, StreetName = "Storgata 95", City = "Tromsø", Postcode = "9008" },
                        new Address { AddressID = 6, StreetName = "Kjøpmannsgata 38", City = "Trondheim", Postcode = "7011" },
                        new Address { AddressID = 7, StreetName = "Torggata 7", City = "Oslo", Postcode = "0181" },
                        new Address { AddressID = 8, StreetName = "Nedre Slottsgate 4", City = "Oslo", Postcode = "0157" },
                        new Address { AddressID = 9, StreetName = "Kirkegata 15", City = "Stavanger", Postcode = "4006" },
                        new Address { AddressID = 10, StreetName = "Strandkaien 2", City = "Bergen", Postcode = "5013" },
                        new Address { AddressID = 11, StreetName = "Prinsens gate 39", City = "Oslo", Postcode = "0158" },
                        new Address { AddressID = 12, StreetName = "Akersgata 55", City = "Oslo", Postcode = "0180" },
                        new Address { AddressID = 13, StreetName = "Dronningens gate 40", City = "Trondheim", Postcode = "7011" },
                        new Address { AddressID = 14, StreetName = "C.J. Hambros plass 2", City = "Oslo", Postcode = "0164" },
                        new Address { AddressID = 15, StreetName = "Strandgaten 91", City = "Bergen", Postcode = "5004" },
                        new Address { AddressID = 16, StreetName = "Storgata 33", City = "Oslo", Postcode = "0184" },
                        new Address { AddressID = 17, StreetName = "Sjøgata 14", City = "Tromsø", Postcode = "9006" },
                        new Address { AddressID = 18, StreetName = "Holbergs gate 21", City = "Oslo", Postcode = "0166" },
                        new Address { AddressID = 19, StreetName = "Lagasvei 56", City = "Stavanger", Postcode = "4010" },
                        new Address { AddressID = 20, StreetName = "Torgallmenningen 3", City = "Bergen", Postcode = "5014" },
                        new Address { AddressID = 21, StreetName = "Markveien 58", City = "Oslo", Postcode = "0554" },
                        new Address { AddressID = 22, StreetName = "Bygdøy allé 2", City = "Oslo", Postcode = "0257" },
                        new Address { AddressID = 23, StreetName = "Nordre gate 13", City = "Trondheim", Postcode = "7011" },
                        new Address { AddressID = 24, StreetName = "Rådhusgata 28", City = "Oslo", Postcode = "0151" },
                        new Address { AddressID = 25, StreetName = "Bogstadveien 30", City = "Oslo", Postcode = "0355" },
                        new Address { AddressID = 26, StreetName = "Hans Nielsen Hauges gate 7", City = "Oslo", Postcode = "0481" },
                        new Address { AddressID = 27, StreetName = "Sandakerveien 116", City = "Oslo", Postcode = "0484" },
                        new Address { AddressID = 28, StreetName = "Fridtjof Nansens plass 6", City = "Oslo", Postcode = "0160" },
                        new Address { AddressID = 29, StreetName = "Søndre gate 22", City = "Trondheim", Postcode = "7011" },
                        new Address { AddressID = 30, StreetName = "Valkendorfsgaten 1A", City = "Bergen", Postcode = "5012" },
                        new Address { AddressID = 31, StreetName = "Dalsveien 40", City = "Oslo", Postcode = "0775" },
                        new Address { AddressID = 32, StreetName = "Grønland 28", City = "Oslo", Postcode = "0188" },
                        new Address { AddressID = 33, StreetName = "Youngs gate 11", City = "Oslo", Postcode = "0181" },
                        new Address { AddressID = 34, StreetName = "Waldemar Thranes gate 98", City = "Oslo", Postcode = "0175" },
                        new Address { AddressID = 35, StreetName = "Kong Oscars gate 59", City = "Bergen", Postcode = "5017" },
                        new Address { AddressID = 36, StreetName = "Helgesens gate 42", City = "Oslo", Postcode = "0554" },
                        new Address { AddressID = 37, StreetName = "Breigata 14", City = "Stavanger", Postcode = "4006" },
                        new Address { AddressID = 38, StreetName = "Pedersgata 38", City = "Stavanger", Postcode = "4013" },
                        new Address { AddressID = 39, StreetName = "Vestre Strandgate 23", City = "Kristiansand", Postcode = "4611" },
                        new Address { AddressID = 40, StreetName = "Rica Parken Hotel", City = "Ålesund", Postcode = "6003" },
                        new Address { AddressID = 41, StreetName = "Fjordgata 1", City = "Trondheim", Postcode = "7010" },
                        new Address { AddressID = 42, StreetName = "Olav Tryggvasons gate 5", City = "Trondheim", Postcode = "7011" },
                        new Address { AddressID = 43, StreetName = "Bragernes Torg 2A", City = "Drammen", Postcode = "3017" },
                        new Address { AddressID = 44, StreetName = "Stortorget 1", City = "Hammerfest", Postcode = "9615" },
                        new Address { AddressID = 45, StreetName = "Strandgata 89", City = "Harstad", Postcode = "9405" },
                        new Address { AddressID = 46, StreetName = "Jernbanetorget 1", City = "Oslo", Postcode = "0154" },
                        new Address { AddressID = 47, StreetName = "Storgata 41", City = "Bodø", Postcode = "8006" },
                        new Address { AddressID = 48, StreetName = "Kjøpmannsgata 73", City = "Trondheim", Postcode = "7010" },
                        new Address { AddressID = 49, StreetName = "Skostredet 5", City = "Bergen", Postcode = "5453" },
                    };
                    context.AddRange(addresses);
                    context.SaveChanges();

                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    Console.WriteLine($"Error seeding addresses: {ex.Message}");
                }
            }

            // Checkpoint for properties
            if (!context.Properties.Any())
            {
                Console.WriteLine("No properties found. Seeding properties...");
                try
                {
                    var propertyTypes = new string[] { "house", "cabin", "apartment", "fishing hut", "boathouse" };

                    for (int i = 1; i <= 25; i++)
                    {
                        var chosenPropertyType = propertyTypes[random.Next(propertyTypes.Length)];

                        var property = new Property
                        {
                            PropertyID = i,
                            OwnerID = $"{random.Next(1, 11)}", // Assuming UserID is a string. If it's an int, remove the string interpolation.
                            AddressID = random.Next(1, 50),
                            Title = $"{chosenPropertyType} {i}",
                            PropertyType = chosenPropertyType,
                            Description = $"{chosenPropertyType} in a unique location {i}",
                            MaxGuests = random.Next(1, 9),
                            PricePerNight = random.Next(1400, 6001),
                            LongTermStay = random.Next(0, 2) == 1,
                            ImageUrl = $"images/property/{i}.jpg"
                        };

                        Console.WriteLine($"Adding property {i} of type {chosenPropertyType}");

                        context.Properties.Add(property);
                    }

                    // Commit changes to the database
                    Console.WriteLine("Saving properties to the database...");
                    context.SaveChanges();
                    Console.WriteLine("Properties saved successfully.");
                }
                catch (Exception ex)
                {
                    // Log the exception in more detail
                    Console.WriteLine($"Error seeding properties: {ex.Message}. Stack Trace: {ex.StackTrace}");
                }
            }
            else
            {
                Console.WriteLine("Properties already exist. Skipping seeding.");
            }


            // Makes Reviews and Bookings
            if (!context.Bookings.Any())
            {
                int reviewsPerProperty = 40;

                for (int propertyId = 1; propertyId <= 25; propertyId++)
                {
                    var property = context.Properties.Find(propertyId);
                    if (property == null)
                    {
                        Console.WriteLine($"Property with ID {propertyId} not found. Skipping...");
                        continue;
                    }

                    for (int i = 0; i < reviewsPerProperty; i++)
                    {
                        try
                        {
                            string userId;
                            do
                            {
                                userId = $"{random.Next(1, 41)}";
                            } while (userId == property.OwnerID);

                            DateTime checkIn;
                            DateTime checkOut;

                            // For the first half, ensure bookings are in the past
                            if (i < reviewsPerProperty / 2)
                            {
                                checkIn = DateTime.Now.AddDays(random.Next(-30, -2));
                                checkOut = DateTime.Now.AddDays(random.Next(-1, 30) - checkIn.Day);
                            }
                            else
                            {
                                checkIn = DateTime.Now.AddDays(random.Next(-30, 30));
                                checkOut = checkIn.AddDays(random.Next(1, 31));
                            }

                            var booking = new Booking
                            {
                                PropertyID = propertyId,
                                GuestID = userId,
                                CheckInDate = checkIn,
                                CheckOutDate = checkOut
                            };

                            context.Add(booking);
                            await context.SaveChangesAsync();

                            if (DateTime.Now > booking.CheckOutDate)
                            {
                                var review = new Review
                                {
                                    BookingID = booking.BookingID,
                                    GuestID = userId,
                                    Content = positiveReviews[random.Next(positiveReviews.Length)],
                                    Rating = random.Next(1, 5)
                                };

                                context.Add(review);
                                await context.SaveChangesAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error seeding bookings and reviews for property {propertyId}: {ex.Message}. Stack Trace: {ex.StackTrace}");
                        }
                    }
                }
            }



            if (!context.Questions.Any())
            {
                var randomEmails = new string[]
                {
                    "eksempel1@mail.com", "eksempel2@mail.com", "eksempel3@mail.com", "eksempel4@mail.com",
                    "example1@mail.com", "example2@mail.com", "example3@mail.com", "example4@mail.com"
                };

                var questionsContent = new string[]
                {
                    "Hvordan lager jeg en bruker?", "Hvilke betalingsmåter tar dere?",
                    "Hvordan kan jeg legge ut en eiendom?", "Kan jeg endre e-postadressen min?",
                    "How do I create an account?", "What payment methods do you accept?",
                    "How can I list a property?", "Can I change my email address?"
                };

                var answersContent = new string?[]
                {
                    "For å lage en bruker, klikk på 'Registrer' knappen og følg instruksjonene.",
                    "Vi aksepterer Visa, MasterCard og bankoverføring.",
                    "Etter du har logget inn, klikk på 'Legg ut eiendom' og fyll inn nødvendig informasjon.",
                    "Ja, gå til 'Profilinnstillinger' for å endre e-postadressen din.",
                    "To create an account, click on the 'Register' button and follow the prompts.",
                    "We accept Visa, MasterCard, and bank transfer.",
                    null, null
                };

                var questionsWithAnswerImages = new HashSet<int> { 0, 1, 2, 3, 4, 5 };
                var questionsWithImages = new HashSet<int> { 0, 1, 2, 3, 4 }; // 5 spørsmål med bilder

                var questionsList = new List<Question>();

                for (int i = 0; i < questionsContent.Length; i++)
                {
                    var question = new Question
                    {
                        Email = randomEmails[i],
                        Content = questionsContent[i],
                        AnswerContent = answersContent[i]
                    };

                    if (questionsWithAnswerImages.Contains(i))
                    {
                        question.AnswerImageUrl = $"images/questions/answers/{i}.jpg";
                    }

                    if (questionsWithImages.Contains(i))
                    {
                        question.ImageUrl = $"images/questions/{i}.jpg";
                    }

                    questionsList.Add(question);
                }

                context.Questions.AddRange(questionsList);
                context.SaveChanges();
            }



        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during database initialization: {ex.Message}");
            throw;
        }
    }


}


