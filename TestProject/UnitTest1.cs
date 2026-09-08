using dotnetapp.Data;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using dotnetapp.Repositories;
using dotnetapp.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using AppValidationException = dotnetapp.Exceptions.ValidationException;

namespace dotnetapp.Tests;

[TestFixture]
public class Tests
{
    private ApplicationDbContext c = null!;

    [SetUp]
    public void Setup()
    {
        c = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
    }

    [TearDown]
    public void TearDown()
    {
        c.Dispose();
    }

    private static void Props<T>(params string[] names)
    {
        foreach (var name in names)
        {
            Assert.That(
                typeof(T).GetProperty(name),
                Is.Not.Null,
                $"Missing property: {name}");
        }
    }

    private static void Req<T>(params string[] names)
    {
        foreach (var name in names)
        {
            var property = typeof(T).GetProperty(name);

            Assert.That(property, Is.Not.Null, $"Missing property: {name}");

            Assert.That(
                property!.GetCustomAttributes(typeof(RequiredAttribute), true).Any(),
                Is.True,
                $"{name} should have RequiredAttribute");
        }
    }

    private static ApplicationUser U(
        string id = "customer-1",
        bool active = true)
    {
        return new ApplicationUser
        {
            Id = id,
            UserName = id + "@test.com",
            Email = id + "@test.com",
            Name = id,
            IsActive = active
        };
    }

    private static Event E(
        int id = 1,
        string org = "org-1",
        decimal price = 100,
        int cap = 100,
        int booked = 0,
        string status = EventStatus.Active)
    {
        return new Event
        {
            Id = id,
            OrganizerId = org,
            Name = "Event " + id,
            CategoryId = 1,
            Description = "Valid event",
            EventDate = DateTime.UtcNow.Date.AddDays(10),
            EventTime = new TimeSpan(10, 0, 0),
            Venue = "Chennai Hall",
            Capacity = cap,
            BookedSeats = booked,
            TicketPrice = price,
            EventImage = "image.jpg",
            Status = status
        };
    }

    private async Task Seed()
    {
        c.Users.Add(U());
        c.Users.Add(U("org-1"));

        c.EventCategories.Add(
            new EventCategory
            {
                Id = 1,
                Name = "Technology",
                Description = "Technology events"
            });

        c.Events.Add(E());

        await c.SaveChangesAsync();
    }

    [Test, Order(1)]
    public void backend_testApplicationUserEntityFieldsAndValidationAnnotations()
    {
        Props<ApplicationUser>(
            "Id",
            "Name",
            "ProfileImage",
            "CreatedDate",
            "IsActive",
            "Email",
            "UserName");

        Req<ApplicationUser>("Name");
    }

    [Test, Order(2)]
    public void backend_testEventEntityFieldsAndValidationAnnotations()
    {
        Props<Event>(
            "Id",
            "OrganizerId",
            "Name",
            "CategoryId",
            "Description",
            "EventDate",
            "EventTime",
            "Venue",
            "Capacity",
            "BookedSeats",
            "RemainingSeats",
            "TicketPrice",
            "EventImage",
            "Status");

        Req<Event>(
            "OrganizerId",
            "Name",
            "Description",
            "EventDate",
            "Venue",
            "Status");

        Assert.That(
            typeof(Event)
                .GetProperty("Capacity")!
                .GetCustomAttributes(typeof(RangeAttribute), true)
                .Any(),
            Is.True);
    }

    [Test, Order(3)]
    public void backend_testBookingEntityFieldsAndValidationAnnotations()
    {
        Props<Booking>(
            "Id",
            "CustomerId",
            "EventId",
            "TicketQuantity",
            "TotalAmount",
            "BookingDate",
            "Status",
            "PaymentStatus",
            "BookingReference");

        Req<Booking>(
            "CustomerId",
            "EventId",
            "Status",
            "PaymentStatus",
            "BookingReference");
    }

    [Test, Order(4)]
    public void backend_testReviewEntityFieldsAndValidationAnnotations()
    {
        Props<Review>(
            "Id",
            "CustomerId",
            "EventId",
            "Rating",
            "Comment",
            "CreatedDate");

        Req<Review>(
            "CustomerId",
            "EventId",
            "Comment");

        Assert.That(
            typeof(Review)
                .GetProperty("Rating")!
                .GetCustomAttributes(typeof(RangeAttribute), true)
                .Any(),
            Is.True);
    }

    [Test, Order(5)]
    public void backend_testWishlistEntityFieldsAndValidationAnnotations()
    {
        Props<Wishlist>(
            "Id",
            "CustomerId",
            "EventId",
            "AddedDate");

        Req<Wishlist>(
            "CustomerId",
            "EventId");
    }

    [Test, Order(6)]
    public void backend_testEventCategoryEntityFieldsAndValidationAnnotations()
    {
        Props<EventCategory>(
            "Id",
            "Name",
            "Description",
            "Events");

        Req<EventCategory>("Name");
    }

    [Test, Order(7)]
    public void backend_testSupportedEventStatuses()
    {
        Assert.Multiple(() =>
        {
            Assert.That(EventStatus.Active, Is.EqualTo("Active"));
            Assert.That(EventStatus.Cancelled, Is.EqualTo("Cancelled"));
            Assert.That(EventStatus.Completed, Is.EqualTo("Completed"));
        });
    }

    [Test, Order(8)]
    public void backend_testSupportedBookingStatuses()
    {
        Assert.Multiple(() =>
        {
            Assert.That(BookingStatus.Pending, Is.EqualTo("Pending"));
            Assert.That(BookingStatus.Confirmed, Is.EqualTo("Confirmed"));
            Assert.That(BookingStatus.Cancelled, Is.EqualTo("Cancelled"));
            Assert.That(BookingStatus.Completed, Is.EqualTo("Completed"));
        });
    }

    [Test, Order(9)]
    public void backend_testSupportedPaymentStatuses()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PaymentStatus.Pending, Is.EqualTo("Pending"));
            Assert.That(PaymentStatus.Paid, Is.EqualTo("Paid"));
            Assert.That(PaymentStatus.Failed, Is.EqualTo("Failed"));
            Assert.That(PaymentStatus.Refunded, Is.EqualTo("Refunded"));
        });
    }

    [Test, Order(10)]
    public void backend_testApplicationUserRoleConfiguration()
    {
        Assert.That(
            typeof(ApplicationUser).BaseType,
            Is.EqualTo(typeof(IdentityUser)));

        Assert.That(typeof(IdentityRole), Is.Not.Null);
    }

    [Test, Order(11)]
    public void backend_testEventOrganizerRelationship()
    {
        var entity = c.Model.FindEntityType(typeof(Event));

        Assert.That(entity, Is.Not.Null);

        Assert.That(
            entity!.FindNavigation(nameof(Event.Organizer)),
            Is.Not.Null);

        Assert.That(
            entity.GetForeignKeys()
                .Any(f => f.PrincipalEntityType.ClrType == typeof(ApplicationUser)),
            Is.True);
    }

    [Test, Order(12)]
    public void backend_testBookingCustomerAndEventRelationships()
    {
        var entity = c.Model.FindEntityType(typeof(Booking));

        Assert.That(entity, Is.Not.Null);

        Assert.That(
            entity!.FindNavigation(nameof(Booking.Customer)),
            Is.Not.Null);

        Assert.That(
            entity.FindNavigation(nameof(Booking.Event)),
            Is.Not.Null);
    }

    [Test, Order(13)]
    public void backend_testReviewCustomerAndEventRelationships()
    {
        var entity = c.Model.FindEntityType(typeof(Review));

        Assert.That(entity, Is.Not.Null);

        Assert.That(
            entity!.FindNavigation(nameof(Review.Customer)),
            Is.Not.Null);

        Assert.That(
            entity.FindNavigation(nameof(Review.Event)),
            Is.Not.Null);
    }

    [Test, Order(14)]
    public void backend_testWishlistCustomerAndEventRelationships()
    {
        var entity = c.Model.FindEntityType(typeof(Wishlist));

        Assert.That(entity, Is.Not.Null);

        Assert.That(
            entity!.FindNavigation(nameof(Wishlist.Customer)),
            Is.Not.Null);

        Assert.That(
            entity.FindNavigation(nameof(Wishlist.Event)),
            Is.Not.Null);
    }

    [Test, Order(15)]
    public void backend_testEventCategoryEventRelationship()
    {
        Assert.That(
            c.Model.FindEntityType(typeof(Event))!
                .FindNavigation(nameof(Event.Category)),
            Is.Not.Null);

        Assert.That(
            c.Model.FindEntityType(typeof(EventCategory))!
                .FindNavigation(nameof(EventCategory.Events)),
            Is.Not.Null);
    }

    [Test, Order(16)]
    public void backend_testEventValidationRules()
    {
        var eventEntity = new Event();

        var results = new List<ValidationResult>();

        Assert.That(
            Validator.TryValidateObject(
                eventEntity,
                new ValidationContext(eventEntity),
                results,
                true),
            Is.False);
    }

    [Test, Order(17)]
    public void backend_testBookingValidationRules()
    {
        var booking = new Booking();

        var results = new List<ValidationResult>();

        Assert.That(
            Validator.TryValidateObject(
                booking,
                new ValidationContext(booking),
                results,
                true),
            Is.False);
    }

    [Test, Order(18)]
    public void backend_testReviewValidationRules()
    {
        var review = new Review
        {
            CustomerId = "customer-1",
            EventId = 1,
            Rating = 6,
            Comment = ""
        };

        var results = new List<ValidationResult>();

        Assert.That(
            Validator.TryValidateObject(
                review,
                new ValidationContext(review),
                results,
                true),
            Is.False);
    }

    [Test, Order(19)]
    public async Task backend_testWishlistDuplicateValidation()
    {
        await Seed();

        var service = new WishlistService(c);

        await service.AddToWishlistAsync(
            new Wishlist
            {
                CustomerId = "customer-1",
                EventId = 1
            });

        Assert.ThrowsAsync<AppValidationException>(
            () => service.AddToWishlistAsync(
                new Wishlist
                {
                    CustomerId = "customer-1",
                    EventId = 1
                }));
    }

    [Test, Order(20)]
    public void backend_testRequestResponseDtoArchitecture()
    {
        Props<RegisterDTO>(
            "Name",
            "Email",
            "Password",
            "Role");

        Props<LoginDTO>(
            "Email",
            "Password");

        Props<EventDTO>(
            "EventId",
            "EventName",
            "AvailableSeats");

        Props<BookingDTO>(
            "BookingId",
            "TotalAmount");

        Props<ReviewDTO>(
            "ReviewId",
            "Rating");
    }

    [Test, Order(21)]
    public void backend_testEventRequestResponseDtoFields()
    {
        Props<EventDTO>(
            "EventId",
            "OrganizerId",
            "EventName",
            "CategoryId",
            "CategoryName",
            "Description",
            "EventDate",
            "EventTime",
            "Venue",
            "Capacity",
            "BookedSeats",
            "AvailableSeats",
            "TicketPrice",
            "EventImage",
            "Status",
            "AverageRating",
            "TotalReviews");
    }

    [Test, Order(22)]
    public void backend_testBookingRequestResponseDtoFields()
    {
        Props<BookingDTO>(
            "BookingId",
            "CustomerId",
            "EventId",
            "EventName",
            "TicketQuantity",
            "TotalAmount",
            "BookingDate",
            "Status",
            "PaymentStatus",
            "BookingReference");
    }

    [Test, Order(23)]
    public void backend_testReviewRequestResponseDtoFields()
    {
        Props<ReviewDTO>(
            "ReviewId",
            "CustomerId",
            "EventId",
            "Rating",
            "Comment",
            "CreatedDate");
    }

    [Test, Order(24)]
    public void backend_testUserRegistrationAndLoginDtoFields()
    {
        Props<RegisterDTO>(
            "Name",
            "Email",
            "Password",
            "Role");

        Props<LoginDTO>(
            "Email",
            "Password");

        Assert.That(
            typeof(RegisterDTO)
                .GetProperty("Email")!
                .GetCustomAttributes(typeof(EmailAddressAttribute), true)
                .Any(),
            Is.True);
    }

    [Test, Order(25)]
    public async Task backend_testDtoMappingAndNestedEventResponseBehavior()
    {
        await Seed();

        var service = new EventService(c);

        var result = await service.GetEventByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CategoryId, Is.EqualTo(1));
        Assert.That(result.RemainingSeats, Is.EqualTo(100));
        Assert.That(result.Category, Is.Not.Null);
        Assert.That(result.Category!.Name, Is.EqualTo("Technology"));
    }

    [Test, Order(26)]
    public void backend_testEntitiesNotDirectlyExposedThroughRestApis()
    {
        Assert.That(
            typeof(dotnetapp.Controllers.EventController)
                .GetMethod("GetAll"),
            Is.Not.Null);

        Assert.That(
            typeof(EventDTO),
            Is.Not.EqualTo(typeof(Event)));

        Assert.That(
            typeof(BookingDTO),
            Is.Not.EqualTo(typeof(Booking)));

        Assert.That(
            typeof(ReviewDTO),
            Is.Not.EqualTo(typeof(Review)));
    }

    [Test, Order(27)]
    public async Task backend_testEventRepositoryCrudSearchFilterPaginationAndSorting()
    {
        c.EventCategories.Add(
            new EventCategory
            {
                Id = 1,
                Name = "Technology"
            });

        c.Users.Add(U("org-1"));

        c.Events.AddRange(
            E(1, price: 500),
            E(2, price: 200),
            E(3, price: 800));

        await c.SaveChangesAsync();

        var repository = new EventRepository(c);

        Assert.That(
            (await repository.GetAllAsync()).Count,
            Is.EqualTo(3));

        Assert.That(
            (await repository.GetByIdAsync(2))!.Name,
            Is.EqualTo("Event 2"));

        Assert.That(
            (await repository.SearchAsync("Event 1")).Count,
            Is.EqualTo(1));

        Assert.That(
            (await repository.FilterAsync(
                null,
                1,
                null,
                null,
                300)).Count,
            Is.EqualTo(1));

        Assert.That(
            (await repository.SortAsync("priceasc")).First().TicketPrice,
            Is.EqualTo(200));

        var added = await repository.AddAsync(
            E(4, price: 50));

        added.Name = "Updated";

        await repository.UpdateAsync(added);

        Assert.That(
            (await repository.GetByIdAsync(added.Id))!.Name,
            Is.EqualTo("Updated"));

        Assert.That(
            await repository.DeleteAsync(added.Id),
            Is.True);
    }

    [Test, Order(28)]
    public async Task backend_testBookingRepositoryCrudCustomerEventStatusAndDateQueries()
    {
        await Seed();

        var repository = new BookingRepository(c);

        var booking = await repository.AddAsync(
            new Booking
            {
                CustomerId = "customer-1",
                EventId = 1,
                TicketQuantity = 2,
                TotalAmount = 200,
                Status = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                BookingReference = "R1"
            });

        Assert.That(
            (await repository.GetAllAsync()).Count,
            Is.EqualTo(1));

        Assert.That(
            (await repository.GetByCustomerAsync("customer-1")).Count,
            Is.EqualTo(1));

        Assert.That(
            (await repository.GetByEventAsync(1)).Count,
            Is.EqualTo(1));

        Assert.That(
            (await repository.GetByStatusAsync(BookingStatus.Confirmed)).Count,
            Is.EqualTo(1));

        Assert.That(
            (await repository.GetByDateAsync(booking.BookingDate)).Count,
            Is.EqualTo(1));

        Assert.That(
            await repository.DeleteAsync(booking.Id),
            Is.True);
    }

    [Test, Order(29)]
    public async Task backend_testReviewRepositoryCrudEventRatingAndCustomerQueries()
    {
        await Seed();

        var repository = new ReviewRepository(c);

        await repository.AddAsync(
            new Review
            {
                CustomerId = "customer-1",
                EventId = 1,
                Rating = 5,
                Comment = "Excellent"
            });

        await repository.AddAsync(
            new Review
            {
                CustomerId = "customer-1",
                EventId = 1,
                Rating = 4,
                Comment = "Good"
            });

        await repository.AddAsync(
            new Review
            {
                CustomerId = "customer-1",
                EventId = 1,
                Rating = 3,
                Comment = "Okay"
            });

        Assert.That(
            (await repository.GetByEventAsync(1)).Count,
            Is.EqualTo(3));

        Assert.That(
            (await repository.GetByCustomerAsync("customer-1")).Count,
            Is.EqualTo(3));

        var average = await repository.GetAverageRatingAsync(1);

        Assert.That(
            average,
            Is.EqualTo(4.0).Within(0.0001));
    }

    [Test, Order(30)]
    public async Task backend_testWishlistRepositoryCrudCustomerAndEventQueries()
    {
        await Seed();

        var repository = new WishlistRepository(c);

        var wishlist = await repository.AddAsync(
            new Wishlist
            {
                CustomerId = "customer-1",
                EventId = 1
            });

        Assert.That(
            (await repository.GetAllAsync()).Count,
            Is.EqualTo(1));

        Assert.That(
            (await repository.GetByCustomerAsync("customer-1")).Count,
            Is.EqualTo(1));

        Assert.That(
            await repository.GetByCustomerAndEventAsync(
                "customer-1",
                1),
            Is.Not.Null);

        Assert.That(
            await repository.GetByIdAsync(wishlist.Id),
            Is.Not.Null);

        Assert.That(
            await repository.RemoveAsync("customer-1", 1),
            Is.True);
    }

    [Test, Order(31)]
    public async Task backend_testEventServiceCrudSearchFilterAndSortingBehavior()
    {
        await Seed();

        var service = new EventService(c);

        Assert.That(
            (await service.GetAllEventsAsync()).Count,
            Is.EqualTo(1));

        Assert.That(
            (await service.SearchEventsAsync("Event 1")).Count,
            Is.EqualTo(1));

        Assert.That(
            (await service.FilterEventsAsync(
                null,
                1,
                null,
                null,
                1000)).Count,
            Is.EqualTo(1));

        Assert.That(
            (await service.SortEventsAsync("priceasc")).Count,
            Is.EqualTo(1));
    }

    [Test, Order(32)]
    public async Task backend_testBookingServiceCrudCustomerHistoryAndFilteringBehavior()
    {
        await Seed();

        var service = new BookingService(c);

        var booking = await service.AddBookingAsync(
            new Booking
            {
                CustomerId = "customer-1",
                EventId = 1,
                TicketQuantity = 2
            });

        Assert.That(
            booking.TotalAmount,
            Is.EqualTo(200));

        Assert.That(
            booking.Status,
            Is.EqualTo(BookingStatus.Confirmed));

        Assert.That(
            (await service.GetBookingsByCustomerAsync("customer-1")).Count,
            Is.EqualTo(1));

        Assert.That(
            (await service.GetBookingHistoryAsync(
                "customer-1",
                BookingStatus.Confirmed)).Count,
            Is.EqualTo(1));
    }

    [Test, Order(33)]
    public async Task backend_testReviewServiceCreateAndRetrieveBehavior()
    {
        await Seed();

        var service = new ReviewService(c);

        var review = await service.AddReviewAsync(
            new Review
            {
                CustomerId = "customer-1",
                EventId = 1,
                Rating = 5,
                Comment = "Great"
            });

        Assert.That(
            review.Id,
            Is.GreaterThan(0));

        Assert.That(
            (await service.GetAllReviewsAsync()).Count,
            Is.EqualTo(1));

        Assert.That(
            await service.GetAverageRatingAsync(1),
            Is.EqualTo(5).Within(0.0001));
    }

    [Test, Order(34)]
    public async Task backend_testWishlistServiceAddRemoveAndRetrieveBehavior()
    {
        await Seed();

        var serviceType = typeof(WishlistService);

        Assert.That(
            serviceType,
            Is.Not.Null);

        var service = new WishlistService(c);

        await service.AddToWishlistAsync(
            new Wishlist
            {
                CustomerId = "customer-1",
                EventId = 1
            });

        Assert.That(
            (await service.GetWishlistByCustomerAsync("customer-1")).Count,
            Is.EqualTo(1));

        Assert.That(
            await service.RemoveFromWishlistAsync(
                "customer-1",
                1),
            Is.True);
    }

    [Test, Order(35)]
    public void backend_testEventRequiresValidFutureDateAndCapacity()
    {
        var service = new EventService(c);

        Assert.ThrowsAsync<AppValidationException>(
            () => service.AddEventAsync(
                new Event
                {
                    OrganizerId = "org-1",
                    Name = "Past",
                    Description = "x",
                    CategoryId = 1,
                    EventDate = DateTime.UtcNow.Date.AddDays(-1),
                    Capacity = 100,
                    TicketPrice = 10,
                    Venue = "V",
                    Status = EventStatus.Active
                }));

        Assert.ThrowsAsync<AppValidationException>(
            () => service.AddEventAsync(
                new Event
                {
                    OrganizerId = "org-1",
                    Name = "Bad",
                    Description = "x",
                    CategoryId = 1,
                    EventDate = DateTime.UtcNow.Date.AddDays(2),
                    Capacity = 0,
                    TicketPrice = 10,
                    Venue = "V",
                    Status = EventStatus.Active
                }));
    }

    [Test, Order(36)]
    public void backend_testBookingRequiresExistingCustomerAndEvent()
    {
        var service = new BookingService(c);

        Assert.ThrowsAsync<AppValidationException>(
            () => service.AddBookingAsync(
                new Booking
                {
                    CustomerId = "missing",
                    EventId = 999,
                    TicketQuantity = 1
                }));
    }

    [Test, Order(37)]
    public async Task backend_testBookingRejectsInactiveOrCancelledEvent()
    {
        c.Users.Add(U());

        c.EventCategories.Add(
            new EventCategory
            {
                Id = 1,
                Name = "Technology"
            });

        c.Events.Add(
            E(status: EventStatus.Cancelled));

        await c.SaveChangesAsync();

        var service = new BookingService(c);

        Assert.ThrowsAsync<AppValidationException>(
            () => service.AddBookingAsync(
                new Booking
                {
                    CustomerId = "customer-1",
                    EventId = 1,
                    TicketQuantity = 1
                }));
    }

    [Test, Order(38)]
    public async Task backend_testBookingRejectsTicketsAboveAvailableSeats()
    {
        c.Users.Add(U());

        c.EventCategories.Add(
            new EventCategory
            {
                Id = 1,
                Name = "Technology"
            });

        c.Events.Add(
            E(
                cap: 5,
                booked: 4));

        await c.SaveChangesAsync();

        var service = new BookingService(c);

        Assert.ThrowsAsync<AppValidationException>(
            () => service.AddBookingAsync(
                new Booking
                {
                    CustomerId = "customer-1",
                    EventId = 1,
                    TicketQuantity = 2
                }));
    }

    [Test, Order(39)]
    public async Task backend_testBookingTotalCalculatedByBackendUsingTicketPrice()
    {
        c.Users.Add(U());

        c.EventCategories.Add(
            new EventCategory
            {
                Id = 1,
                Name = "Technology"
            });

        c.Events.Add(
            E(price: 250));

        await c.SaveChangesAsync();

        var booking = await new BookingService(c).AddBookingAsync(
            new Booking
            {
                CustomerId = "customer-1",
                EventId = 1,
                TicketQuantity = 3,
                TotalAmount = 1
            });

        Assert.That(
            booking.TotalAmount,
            Is.EqualTo(750));
    }

    [Test, Order(40)]
    public async Task backend_testBookingCancellationRestoresAvailableSeats()
    {
        c.Users.Add(U());

        c.EventCategories.Add(
            new EventCategory
            {
                Id = 1,
                Name = "Technology"
            });

        c.Events.Add(
            E(cap: 10));

        await c.SaveChangesAsync();

        var service = new BookingService(c);

        var booking = await service.AddBookingAsync(
            new Booking
            {
                CustomerId = "customer-1",
                EventId = 1,
                TicketQuantity = 3
            });

        Assert.That(
            (await c.Events.FindAsync(1))!.BookedSeats,
            Is.EqualTo(3));

        Assert.That(
            await service.CancelBookingAsync(booking.Id),
            Is.True);

        Assert.That(
            (await c.Events.FindAsync(1))!.BookedSeats,
            Is.EqualTo(0));
    }
}
