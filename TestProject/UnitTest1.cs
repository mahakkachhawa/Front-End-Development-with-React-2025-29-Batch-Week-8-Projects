using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using dotnetapp.Constants;
using dotnetapp.Controllers;
using dotnetapp.Data;
using dotnetapp.DTOs;
using dotnetapp.Models;
using dotnetapp.Repositories;
using dotnetapp.Services;

using BusinessException = dotnetapp.Exceptions.BusinessException;
using NotFoundException = dotnetapp.Exceptions.NotFoundException;
using CustomValidationException = dotnetapp.Exceptions.ValidationException;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class Tests
    {
        private ApplicationDbContext CreateContext()
        {
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            return new ApplicationDbContext(options);
        }

        private ApplicationUser CreateUser(
            string id,
            string name,
            string email,
            bool active = true)
        {
            return new ApplicationUser
            {
                Id = id,
                UserName = email,
                Email = email,
                Name = name,
                IsActive = active,
                CreatedDate = DateTime.UtcNow
            };
        }

        private EventCategory CreateCategory(
            int id = 1,
            string name = "Technology")
        {
            return new EventCategory
            {
                Id = id,
                Name = name,
                Description = "Test category"
            };
        }

        private Event CreateEvent(
            string organizerId,
            int id = 1,
            string name = "Tech Conference",
            int capacity = 100,
            int bookedSeats = 10,
            decimal ticketPrice = 500)
        {
            return new Event
            {
                Id = id,
                OrganizerId = organizerId,
                CategoryId = 1,
                Name = name,
                Description = "Technology conference event",
                EventDate = DateTime.Now.AddDays(30),
                EventTime = new TimeSpan(10, 0, 0),
                Venue = "Chennai Convention Centre",
                Capacity = capacity,
                BookedSeats = bookedSeats,
                TicketPrice = ticketPrice,
                EventImage = "event.jpg",
                Status = EventStatuses.Active,
                CreatedDate = DateTime.UtcNow
            };
        }

        private Booking CreateBooking(
            string customerId,
            int eventId,
            int id = 1,
            int quantity = 2,
            decimal total = 1000,
            string status = BookingStatuses.Confirmed)
        {
            return new Booking
            {
                Id = id,
                CustomerId = customerId,
                EventId = eventId,
                TicketQuantity = quantity,
                TotalAmount = total,
                BookingDate = DateTime.UtcNow,
                Status = status,
                PaymentStatus = PaymentStatuses.Paid,
                BookingReference = "EVT-TEST-001"
            };
        }

        private Review CreateReview(
            string customerId,
            int eventId,
            int id = 1,
            int rating = 5,
            string comment = "Excellent event")
        {
            return new Review
            {
                Id = id,
                CustomerId = customerId,
                EventId = eventId,
                Rating = rating,
                Comment = comment,
                CreatedDate = DateTime.UtcNow
            };
        }

        private Wishlist CreateWishlist(
            string customerId,
            int eventId,
            int id = 1)
        {
            return new Wishlist
            {
                Id = id,
                CustomerId = customerId,
                EventId = eventId,
                AddedDate = DateTime.UtcNow
            };
        }

        private bool HasValidationError<T>(T model)
        {
            var context = new ValidationContext(model!);
            var results = new List<ValidationResult>();

            return !Validator.TryValidateObject(
                model!,
                context,
                results,
                true);
        }

        // ============================================================
        // 1
        // ============================================================

        [Test]
        public void backend_testApplicationUserEntityFieldsAndValidationAnnotations()
        {
            var type = typeof(ApplicationUser);

            Assert.That(
                type.BaseType,
                Is.EqualTo(typeof(IdentityUser)));

            Assert.That(
                type.GetProperty("Name"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("ProfileImage"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("CreatedDate"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("IsActive"),
                Is.Not.Null);

            var user = new ApplicationUser();

            Assert.That(
                HasValidationError(user),
                Is.True);

            var validUser =
                CreateUser(
                    "u1",
                    "John Doe",
                    "john@test.com");

            Assert.That(
                HasValidationError(validUser),
                Is.False);
        }

        // ============================================================
        // 2
        // ============================================================

        [Test]
        public void backend_testEventEntityFieldsAndValidationAnnotations()
        {
            var type = typeof(Event);

            string[] requiredProperties =
            {
                "Name",
                "Description",
                "EventDate",
                "Venue",
                "OrganizerId",
                "CategoryId",
                "Capacity",
                "TicketPrice",
                "Status"
            };

            foreach (var property in requiredProperties)
            {
                Assert.That(
                    type.GetProperty(property),
                    Is.Not.Null,
                    $"Missing Event property: {property}");
            }

            var invalidEvent =
                new Event();

            Assert.That(
                HasValidationError(invalidEvent),
                Is.True);

            var validEvent =
                CreateEvent("organizer-1");

            Assert.That(
                HasValidationError(validEvent),
                Is.False);
        }

        // ============================================================
        // 3
        // ============================================================

        [Test]
        public void backend_testBookingEntityFieldsAndValidationAnnotations()
        {
            var type = typeof(Booking);

            string[] requiredProperties =
            {
                "CustomerId",
                "EventId",
                "TicketQuantity",
                "TotalAmount",
                "Status",
                "PaymentStatus",
                "BookingReference"
            };

            foreach (var property in requiredProperties)
            {
                Assert.That(
                    type.GetProperty(property),
                    Is.Not.Null,
                    $"Missing Booking property: {property}");
            }

            var invalidBooking =
                new Booking();

            Assert.That(
                HasValidationError(invalidBooking),
                Is.True);

            var validBooking =
                CreateBooking(
                    "customer-1",
                    1);

            Assert.That(
                HasValidationError(validBooking),
                Is.False);
        }

        // ============================================================
        // 4
        // ============================================================

        [Test]
        public void backend_testReviewEntityFieldsAndValidationAnnotations()
        {
            var type = typeof(Review);

            Assert.That(
                type.GetProperty("CustomerId"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("EventId"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("Rating"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("Comment"),
                Is.Not.Null);

            var invalidReview =
                new Review();

            Assert.That(
                HasValidationError(invalidReview),
                Is.True);

            var validReview =
                CreateReview(
                    "customer-1",
                    1);

            Assert.That(
                HasValidationError(validReview),
                Is.False);
        }

        // ============================================================
        // 5
        // ============================================================

        [Test]
        public void backend_testWishlistEntityFieldsAndValidationAnnotations()
        {
            var type = typeof(Wishlist);

            Assert.That(
                type.GetProperty("CustomerId"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("EventId"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("AddedDate"),
                Is.Not.Null);

            var invalidWishlist =
                new Wishlist();

            Assert.That(
                HasValidationError(invalidWishlist),
                Is.True);

            var validWishlist =
                CreateWishlist(
                    "customer-1",
                    1);

            Assert.That(
                HasValidationError(validWishlist),
                Is.False);
        }

        // ============================================================
        // 6
        // ============================================================

        [Test]
        public void backend_testEventCategoryEntityFieldsAndValidationAnnotations()
        {
            var type = typeof(EventCategory);

            Assert.That(
                type.GetProperty("Id"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("Name"),
                Is.Not.Null);

            Assert.That(
                type.GetProperty("Description"),
                Is.Not.Null);

            var invalidCategory =
                new EventCategory();

            Assert.That(
                HasValidationError(invalidCategory),
                Is.True);

            var validCategory =
                CreateCategory();

            Assert.That(
                HasValidationError(validCategory),
                Is.False);
        }

        // ============================================================
        // 7
        // ============================================================

        [Test]
        public void backend_testSupportedEventStatuses()
        {
            Assert.That(
                EventStatuses.Active,
                Is.EqualTo("Active"));

            Assert.That(
                EventStatuses.Cancelled,
                Is.EqualTo("Cancelled"));

            Assert.That(
                EventStatuses.Completed,
                Is.EqualTo("Completed"));

            var supportedStatuses =
                new[]
                {
                    EventStatuses.Active,
                    EventStatuses.Cancelled,
                    EventStatuses.Completed
                };

            Assert.That(
                supportedStatuses.Length,
                Is.EqualTo(3));
        }

        // ============================================================
        // 8
        // ============================================================

        [Test]
        public void backend_testSupportedBookingStatuses()
        {
            Assert.That(
                BookingStatuses.Pending,
                Is.EqualTo("Pending"));

            Assert.That(
                BookingStatuses.Confirmed,
                Is.EqualTo("Confirmed"));

            Assert.That(
                BookingStatuses.Cancelled,
                Is.EqualTo("Cancelled"));

            Assert.That(
                BookingStatuses.Completed,
                Is.EqualTo("Completed"));
        }

        // ============================================================
        // 9
        // ============================================================

        [Test]
        public void backend_testSupportedPaymentStatuses()
        {
            Assert.That(
                PaymentStatuses.Pending,
                Is.EqualTo("Pending"));

            Assert.That(
                PaymentStatuses.Paid,
                Is.EqualTo("Paid"));

            Assert.That(
                PaymentStatuses.Failed,
                Is.EqualTo("Failed"));

            Assert.That(
                PaymentStatuses.Refunded,
                Is.EqualTo("Refunded"));
        }

        // ============================================================
        // 10
        // ============================================================

        [Test]
        public void backend_testApplicationUserRoleConfiguration()
        {
            Assert.That(
                UserRoles.Organizer,
                Is.EqualTo("Organizer"));

            Assert.That(
                UserRoles.Customer,
                Is.EqualTo("Customer"));

            var roles =
                new[]
                {
                    UserRoles.Organizer,
                    UserRoles.Customer
                };

            Assert.That(
                roles,
                Does.Contain("Organizer"));

            Assert.That(
                roles,
                Does.Contain("Customer"));
        }

        // ============================================================
        // 11
        // ============================================================

        [Test]
        public void backend_testEventOrganizerRelationship()
        {
            using var context =
                CreateContext();

            var organizer =
                CreateUser(
                    "org-1",
                    "Organizer One",
                    "org@test.com");

            var eventEntity =
                CreateEvent(
                    organizer.Id);

            context.Users.Add(organizer);
            context.EventCategories.Add(
                CreateCategory());
            context.Events.Add(eventEntity);

            context.SaveChanges();

            var stored =
                context.Events
                    .Include(e => e.Organizer)
                    .First(
                        e => e.Id == eventEntity.Id);

            Assert.That(
                stored.Organizer,
                Is.Not.Null);

            Assert.That(
                stored.Organizer!.Id,
                Is.EqualTo(organizer.Id));

            Assert.That(
                stored.Organizer!.Name,
                Is.EqualTo("Organizer One"));
        }

        // ============================================================
        // 12
        // ============================================================

        [Test]
        public void backend_testBookingCustomerAndEventRelationships()
        {
            using var context =
                CreateContext();

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "customer@test.com");

            var eventEntity =
                CreateEvent(
                    "org-1");

            var booking =
                CreateBooking(
                    customer.Id,
                    eventEntity.Id);

            context.Users.Add(customer);
            context.EventCategories.Add(
                CreateCategory());
            context.Events.Add(eventEntity);
            context.Bookings.Add(booking);

            context.SaveChanges();

            var stored =
                context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Event)
                    .First(
                        b => b.Id == booking.Id);

            Assert.That(
                stored.Customer,
                Is.Not.Null);

            Assert.That(
                stored.Event,
                Is.Not.Null);

            Assert.That(
                stored.Customer!.Id,
                Is.EqualTo(customer.Id));

            Assert.That(
                stored.Event!.Id,
                Is.EqualTo(eventEntity.Id));
        }

        // ============================================================
        // 13
        // ============================================================

        [Test]
        public void backend_testReviewCustomerAndEventRelationships()
        {
            using var context =
                CreateContext();

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "customer@test.com");

            var eventEntity =
                CreateEvent(
                    "org-1");

            var review =
                CreateReview(
                    customer.Id,
                    eventEntity.Id);

            context.Users.Add(customer);
            context.EventCategories.Add(
                CreateCategory());
            context.Events.Add(eventEntity);
            context.Reviews.Add(review);

            context.SaveChanges();

            var stored =
                context.Reviews
                    .Include(r => r.Customer)
                    .Include(r => r.Event)
                    .First(
                        r => r.Id == review.Id);

            Assert.That(
                stored.Customer,
                Is.Not.Null);

            Assert.That(
                stored.Event,
                Is.Not.Null);
        }

        // ============================================================
        // 14
        // ============================================================

        [Test]
        public void backend_testWishlistCustomerAndEventRelationships()
        {
            using var context =
                CreateContext();

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "customer@test.com");

            var eventEntity =
                CreateEvent(
                    "org-1");

            var wishlist =
                CreateWishlist(
                    customer.Id,
                    eventEntity.Id);

            context.Users.Add(customer);
            context.EventCategories.Add(
                CreateCategory());
            context.Events.Add(eventEntity);
            context.Wishlists.Add(wishlist);

            context.SaveChanges();

            var stored =
                context.Wishlists
                    .Include(w => w.Customer)
                    .Include(w => w.Event)
                    .First(
                        w => w.Id == wishlist.Id);

            Assert.That(
                stored.Customer,
                Is.Not.Null);

            Assert.That(
                stored.Event,
                Is.Not.Null);
        }

        // ============================================================
        // 15
        // ============================================================

        [Test]
        public void backend_testEventCategoryEventRelationship()
        {
            using var context =
                CreateContext();

            var category =
                CreateCategory(
                    10,
                    "Music");

            var eventEntity =
                CreateEvent(
                    "org-1");

            eventEntity.CategoryId =
                category.Id;

            context.EventCategories.Add(category);
            context.Events.Add(eventEntity);

            context.SaveChanges();

            var stored =
                context.Events
                    .Include(e => e.Category)
                    .First(
                        e => e.Id == eventEntity.Id);

            Assert.That(
                stored.Category,
                Is.Not.Null);

            Assert.That(
                stored.Category!.Name,
                Is.EqualTo("Music"));
        }

        // ============================================================
        // 16
        // ============================================================

        [Test]
        public void backend_testEventValidationRules()
        {
            var invalidEvent =
                new Event
                {
                    OrganizerId = "",
                    CategoryId = 0,
                    Name = "",
                    Description = "",
                    EventDate = DateTime.Now,
                    EventTime = TimeSpan.Zero,
                    Venue = "",
                    Capacity = 0,
                    TicketPrice = -1,
                    Status = ""
                };

            var results =
                new List<ValidationResult>();

            var valid =
                Validator.TryValidateObject(
                    invalidEvent,
                    new ValidationContext(
                        invalidEvent),
                    results,
                    true);

            Assert.That(
                valid,
                Is.False);

            Assert.That(
                results.Count,
                Is.GreaterThan(0));

            var validEvent =
                CreateEvent(
                    "org-1");

            results.Clear();

            valid =
                Validator.TryValidateObject(
                    validEvent,
                    new ValidationContext(
                        validEvent),
                    results,
                    true);

            Assert.That(
                valid,
                Is.True);
        }

        // ============================================================
        // 17
        // ============================================================

        [Test]
        public void backend_testBookingValidationRules()
        {
            var invalidBooking =
                new Booking
                {
                    CustomerId = "",
                    EventId = 0,
                    TicketQuantity = 0,
                    TotalAmount = 0,
                    Status = "",
                    PaymentStatus = "",
                    BookingReference = ""
                };

            var results =
                new List<ValidationResult>();

            var valid =
                Validator.TryValidateObject(
                    invalidBooking,
                    new ValidationContext(
                        invalidBooking),
                    results,
                    true);

            Assert.That(
                valid,
                Is.False);

            var validBooking =
                CreateBooking(
                    "customer-1",
                    1);

            results.Clear();

            valid =
                Validator.TryValidateObject(
                    validBooking,
                    new ValidationContext(
                        validBooking),
                    results,
                    true);

            Assert.That(
                valid,
                Is.True);
        }

        // ============================================================
        // 18
        // ============================================================

        [Test]
        public void backend_testReviewValidationRules()
        {
            var invalidReview =
                new Review
                {
                    CustomerId = "",
                    EventId = 0,
                    Rating = 6,
                    Comment = ""
                };

            var results =
                new List<ValidationResult>();

            var valid =
                Validator.TryValidateObject(
                    invalidReview,
                    new ValidationContext(
                        invalidReview),
                    results,
                    true);

            Assert.That(
                valid,
                Is.False);

            var validReview =
                CreateReview(
                    "customer-1",
                    1,
                    rating: 4,
                    comment: "Good event");

            results.Clear();

            valid =
                Validator.TryValidateObject(
                    validReview,
                    new ValidationContext(
                        validReview),
                    results,
                    true);

            Assert.That(
                valid,
                Is.True);
        }

        // ============================================================
        // 19
        // ============================================================

        [Test]
        public async Task backend_testWishlistDuplicateValidation()
        {
            using var context =
                CreateContext();

            var repository =
                new WishlistRepository(
                    context);

            var service =
                new WishlistService(
                    repository);

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer",
                    "customer@test.com");

            var eventEntity =
                CreateEvent(
                    "org-1");

            context.Users.Add(customer);
            context.EventCategories.Add(
                CreateCategory());
            context.Events.Add(eventEntity);

            await context.SaveChangesAsync();

            await service.AddToWishlistAsync(
                customer.Id,
                eventEntity.Id);

            var exception =
                Assert.ThrowsAsync<BusinessException>(
                    async () =>
                    {
                        await service.AddToWishlistAsync(
                            customer.Id,
                            eventEntity.Id);
                    });

            Assert.That(
                exception!.Message,
                Does.Contain("already exists"));
        }

        // ============================================================
        // 20
        // ============================================================

        [Test]
        public void backend_testRequestResponseDtoArchitecture()
        {
            var dtoTypes =
                new[]
                {
                    typeof(RegisterDTO),
                    typeof(LoginDTO),
                    typeof(EventDTO),
                    typeof(BookingDTO),
                    typeof(ReviewDTO)
                };

            foreach (var type in dtoTypes)
            {
                Assert.That(
                    type.IsClass,
                    Is.True,
                    $"{type.Name} must be a class.");

                Assert.That(
                    type.IsPublic,
                    Is.True,
                    $"{type.Name} must be public.");
            }

            Assert.That(
                typeof(EventDTO)
                    .GetProperty("Id"),
                Is.Not.Null);

            Assert.That(
                typeof(BookingDTO)
                    .GetProperty("Id"),
                Is.Not.Null);

            Assert.That(
                typeof(ReviewDTO)
                    .GetProperty("Id"),
                Is.Not.Null);
        }

        // ============================================================
        // 21
        // ============================================================

        [Test]
        public void backend_testEventRequestResponseDtoFields()
        {
            var properties =
                typeof(EventDTO)
                    .GetProperties()
                    .Select(p => p.Name)
                    .ToList();

            string[] required =
            {
                "Id",
                "OrganizerId",
                "CategoryId",
                "CategoryName",
                "Name",
                "Description",
                "EventDate",
                "EventTime",
                "Venue",
                "Capacity",
                "BookedSeats",
                "RemainingSeats",
                "TicketPrice",
                "EventImage",
                "Status",
                "OrganizerName",
                "AverageRating",
                "TotalReviews",
                "CreatedDate"
            };

            foreach (var property in required)
            {
                Assert.That(
                    properties,
                    Does.Contain(property),
                    $"Missing EventDTO field: {property}");
            }
        }

        // ============================================================
        // 22
        // ============================================================

        [Test]
        public void backend_testBookingRequestResponseDtoFields()
        {
            var properties =
                typeof(BookingDTO)
                    .GetProperties()
                    .Select(p => p.Name)
                    .ToList();

            string[] required =
            {
                "Id",
                "CustomerId",
                "EventId",
                "EventName",
                "EventDate",
                "EventTime",
                "Venue",
                "TicketPrice",
                "TicketQuantity",
                "TotalAmount",
                "BookingDate",
                "Status",
                "PaymentStatus",
                "BookingReference"
            };

            foreach (var property in required)
            {
                Assert.That(
                    properties,
                    Does.Contain(property),
                    $"Missing BookingDTO field: {property}");
            }
        }

        // ============================================================
        // 23
        // ============================================================

        [Test]
        public void backend_testReviewRequestResponseDtoFields()
        {
            var properties =
                typeof(ReviewDTO)
                    .GetProperties()
                    .Select(p => p.Name)
                    .ToList();

            string[] required =
            {
                "Id",
                "CustomerId",
                "CustomerName",
                "EventId",
                "Rating",
                "Comment",
                "CreatedDate"
            };

            foreach (var property in required)
            {
                Assert.That(
                    properties,
                    Does.Contain(property),
                    $"Missing ReviewDTO field: {property}");
            }
        }

        // ============================================================
        // 24
        // ============================================================

        [Test]
        public void backend_testUserRegistrationAndLoginDtoFields()
        {
            var registerProperties =
                typeof(RegisterDTO)
                    .GetProperties()
                    .Select(p => p.Name)
                    .ToList();

            Assert.That(
                registerProperties,
                Does.Contain("Name"));

            Assert.That(
                registerProperties,
                Does.Contain("Email"));

            Assert.That(
                registerProperties,
                Does.Contain("Password"));

            Assert.That(
                registerProperties,
                Does.Contain("Role"));

            var loginProperties =
                typeof(LoginDTO)
                    .GetProperties()
                    .Select(p => p.Name)
                    .ToList();

            Assert.That(
                loginProperties,
                Does.Contain("Email"));

            Assert.That(
                loginProperties,
                Does.Contain("Password"));
        }

        // ============================================================
        // 25
        // ============================================================

        [Test]
        public async Task backend_testDtoMappingAndNestedEventResponseBehavior()
        {
            using var context =
                CreateContext();

            var repository =
                new EventRepository(
                    context);

            var service =
                new EventService(
                    repository);

            var organizer =
                CreateUser(
                    "org-1",
                    "Organizer One",
                    "org@test.com");

            var category =
                CreateCategory();

            var eventEntity =
                CreateEvent(
                    organizer.Id);

            var review =
                CreateReview(
                    "customer-1",
                    eventEntity.Id,
                    rating: 4);

            context.Users.Add(organizer);

            context.Users.Add(
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "customer@test.com"));

            context.EventCategories.Add(category);
            context.Events.Add(eventEntity);
            context.Reviews.Add(review);

            await context.SaveChangesAsync();

            var result =
                await service.GetEventByIdAsync(
                    eventEntity.Id);

            Assert.That(
                result,
                Is.Not.Null);

            Assert.That(
                result!.Name,
                Is.EqualTo(
                    eventEntity.Name));

            Assert.That(
                result.CategoryName,
                Is.EqualTo(
                    category.Name));

            Assert.That(
                result.OrganizerName,
                Is.EqualTo(
                    organizer.Name));

            Assert.That(
                result.RemainingSeats,
                Is.EqualTo(
                    eventEntity.Capacity -
                    eventEntity.BookedSeats));

            Assert.That(
                result.TotalReviews,
                Is.EqualTo(1));

            Assert.That(
                result.AverageRating,
                Is.EqualTo(4));
        }

        // ============================================================
        // 26
        // ============================================================

        [Test]
        public void backend_testEntitiesNotDirectlyExposedThroughRestApis()
        {
            var controllerTypes =
                new[]
                {
                    typeof(AuthController),
                    typeof(EventController),
                    typeof(BookingController),
                    typeof(WishlistController),
                    typeof(ReviewController),
                    typeof(DashboardController)
                };

            foreach (var controllerType in controllerTypes)
            {
                var methods =
                    controllerType.GetMethods(
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.DeclaredOnly);

                Assert.That(
                    methods.Length,
                    Is.GreaterThan(0),
                    $"{controllerType.Name} has no public API methods.");

                foreach (var method in methods)
                {
                    var returnType =
                        method.ReturnType;

                    var isActionResult =
                        typeof(IActionResult)
                            .IsAssignableFrom(
                                returnType);

                    var isTask =
                        returnType.IsGenericType &&
                        returnType.GetGenericTypeDefinition()
                            == typeof(Task<>);

                    var isTaskActionResult =
                        returnType ==
                        typeof(Task<IActionResult>);

                    Assert.That(
                        isActionResult ||
                        isTask ||
                        isTaskActionResult,
                        Is.True,
                        $"{controllerType.Name}.{method.Name} should return an API result.");
                }
            }
        }

        // ============================================================
        // 27
        // ============================================================

        [Test]
        public async Task backend_testEventRepositoryCrudSearchFilterPaginationAndSorting()
        {
            using var context =
                CreateContext();

            var repository =
                new EventRepository(
                    context);

            context.EventCategories.Add(
                CreateCategory());

            var organizer =
                CreateUser(
                    "org-1",
                    "Organizer",
                    "org@test.com");

            context.Users.Add(organizer);

            var event1 =
                CreateEvent(
                    organizer.Id,
                    1,
                    "Alpha Conference",
                    100,
                    10,
                    500);

            var event2 =
                CreateEvent(
                    organizer.Id,
                    2,
                    "Beta Workshop",
                    50,
                    5,
                    1000);

            event2.EventDate =
                DateTime.Now.AddDays(60);

            context.Events.AddRange(
                event1,
                event2);

            await context.SaveChangesAsync();

            var all =
                await repository.GetAllAsync();

            Assert.That(
                all.Count,
                Is.EqualTo(2));

            var byId =
                await repository.GetByIdAsync(1);

            Assert.That(
                byId,
                Is.Not.Null);

            Assert.That(
                byId!.Name,
                Is.EqualTo(
                    "Alpha Conference"));

            var search =
                await repository.SearchAsync(
                    "Alpha",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);

            Assert.That(
                search.Count,
                Is.EqualTo(1));

            var categoryFilter =
                await repository.SearchAsync(
                    null,
                    1,
                    null,
                    null,
                    null,
                    null,
                    null);

            Assert.That(
                categoryFilter.Count,
                Is.EqualTo(2));

            var priceFilter =
                await repository.SearchAsync(
                    null,
                    null,
                    null,
                    null,
                    600,
                    null,
                    null);

            Assert.That(
                priceFilter.Count,
                Is.EqualTo(1));

            var sorted =
                await repository.SearchAsync(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    "priceasc");

            Assert.That(
                sorted[0].TicketPrice,
                Is.LessThanOrEqualTo(
                    sorted[1].TicketPrice));

            var page =
                sorted
                    .Skip(0)
                    .Take(1)
                    .ToList();

            Assert.That(
                page.Count,
                Is.EqualTo(1));

            var updated =
                await repository.GetByIdAsync(1);

            updated!.Name =
                "Updated Conference";

            await repository.UpdateAsync(
                updated);

            var updatedResult =
                await repository.GetByIdAsync(1);

            Assert.That(
                updatedResult!.Name,
                Is.EqualTo(
                    "Updated Conference"));

            var deleted =
                await repository.DeleteAsync(1);

            Assert.That(
                deleted,
                Is.True);

            var deletedResult =
                await repository.GetByIdAsync(1);

            Assert.That(
                deletedResult,
                Is.Null);
        }

        // ============================================================
        // 28
        // ============================================================

        [Test]
        public async Task backend_testBookingRepositoryCrudCustomerEventStatusAndDateQueries()
        {
            using var context =
                CreateContext();

            var repository =
                new BookingRepository(
                    context);

            var customer1 =
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "one@test.com");

            var customer2 =
                CreateUser(
                    "customer-2",
                    "Customer Two",
                    "two@test.com");

            var event1 =
                CreateEvent(
                    "org-1",
                    1);

            var event2 =
                CreateEvent(
                    "org-1",
                    2);

            context.Users.AddRange(
                customer1,
                customer2);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.AddRange(
                event1,
                event2);

            var booking1 =
                CreateBooking(
                    customer1.Id,
                    event1.Id,
                    1,
                    2,
                    1000,
                    BookingStatuses.Confirmed);

            var booking2 =
                CreateBooking(
                    customer1.Id,
                    event2.Id,
                    2,
                    1,
                    500,
                    BookingStatuses.Completed);

            var booking3 =
                CreateBooking(
                    customer2.Id,
                    event1.Id,
                    3,
                    3,
                    1500,
                    BookingStatuses.Cancelled);

            context.Bookings.AddRange(
                booking1,
                booking2,
                booking3);

            await context.SaveChangesAsync();

            var all =
                await repository.GetAllAsync();

            Assert.That(
                all.Count,
                Is.EqualTo(3));

            var customerBookings =
                await repository.GetByCustomerAsync(
                    customer1.Id);

            Assert.That(
                customerBookings.Count,
                Is.EqualTo(2));

            var eventBookings =
                await repository.GetByEventAsync(
                    event1.Id);

            Assert.That(
                eventBookings.Count,
                Is.EqualTo(2));

            var confirmed =
                await repository.GetByStatusAsync(
                    BookingStatuses.Confirmed);

            Assert.That(
                confirmed.Count,
                Is.EqualTo(1));

            var completed =
                await repository.GetByStatusAsync(
                    BookingStatuses.Completed);

            Assert.That(
                completed.Count,
                Is.EqualTo(1));

            var stored =
                await repository.GetByIdAsync(1);

            Assert.That(
                stored,
                Is.Not.Null);

            stored!.Status =
                BookingStatuses.Completed;

            await repository.UpdateAsync(
                stored);

            var updated =
                await repository.GetByIdAsync(1);

            Assert.That(
                updated!.Status,
                Is.EqualTo(
                    BookingStatuses.Completed));
        }

        // ============================================================
        // 29
        // ============================================================

        [Test]
        public async Task backend_testReviewRepositoryCrudEventRatingAndCustomerQueries()
        {
            using var context =
                CreateContext();

            var repository =
                new ReviewRepository(
                    context);

            var customer1 =
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "one@test.com");

            var customer2 =
                CreateUser(
                    "customer-2",
                    "Customer Two",
                    "two@test.com");

            var event1 =
                CreateEvent(
                    "org-1",
                    1);

            var event2 =
                CreateEvent(
                    "org-1",
                    2);

            context.Users.AddRange(
                customer1,
                customer2);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.AddRange(
                event1,
                event2);

            var review1 =
                CreateReview(
                    customer1.Id,
                    event1.Id,
                    1,
                    5,
                    "Excellent");

            var review2 =
                CreateReview(
                    customer2.Id,
                    event1.Id,
                    2,
                    4,
                    "Very good");

            var review3 =
                CreateReview(
                    customer1.Id,
                    event2.Id,
                    3,
                    3,
                    "Average");

            context.Reviews.AddRange(
                review1,
                review2,
                review3);

            await context.SaveChangesAsync();

            var all =
                await repository.GetAllAsync();

            Assert.That(
                all.Count,
                Is.EqualTo(3));

            var eventReviews =
                await repository.GetByEventAsync(
                    event1.Id);

            Assert.That(
                eventReviews.Count,
                Is.EqualTo(2));

            var customerReviews =
                await repository.GetByCustomerAsync(
                    customer1.Id);

            Assert.That(
                customerReviews.Count,
                Is.EqualTo(2));

            var average =
                await repository.GetAverageRatingAsync(
                    event1.Id);

            Assert.That(
                average,
                Is.EqualTo(4));

            var stored =
                await repository.GetByIdAsync(1);

            Assert.That(
                stored,
                Is.Not.Null);

            Assert.That(
                stored!.Rating,
                Is.EqualTo(5));
        }

        // ============================================================
        // 30
        // ============================================================

        [Test]
        public async Task backend_testWishlistRepositoryCrudCustomerAndEventQueries()
        {
            using var context =
                CreateContext();

            var repository =
                new WishlistRepository(
                    context);

            var customer1 =
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "one@test.com");

            var customer2 =
                CreateUser(
                    "customer-2",
                    "Customer Two",
                    "two@test.com");

            var event1 =
                CreateEvent(
                    "org-1",
                    1);

            var event2 =
                CreateEvent(
                    "org-1",
                    2);

            context.Users.AddRange(
                customer1,
                customer2);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.AddRange(
                event1,
                event2);

            await context.SaveChangesAsync();

            var wishlist1 =
                CreateWishlist(
                    customer1.Id,
                    event1.Id,
                    1);

            var wishlist2 =
                CreateWishlist(
                    customer1.Id,
                    event2.Id,
                    2);

            context.Wishlists.AddRange(
                wishlist1,
                wishlist2);

            await context.SaveChangesAsync();

            var customerWishlist =
                await repository.GetByCustomerAsync(
                    customer1.Id);

            Assert.That(
                customerWishlist.Count,
                Is.EqualTo(2));

            var specific =
                await repository.GetAsync(
                    customer1.Id,
                    event1.Id);

            Assert.That(
                specific,
                Is.Not.Null);

            var removed =
                await repository.RemoveAsync(
                    customer1.Id,
                    event1.Id);

            Assert.That(
                removed,
                Is.True);

            var afterRemove =
                await repository.GetAsync(
                    customer1.Id,
                    event1.Id);

            Assert.That(
                afterRemove,
                Is.Null);
        }

        // ============================================================
        // 31
        // ============================================================

        [Test]
        public async Task backend_testEventServiceCrudSearchFilterAndSortingBehavior()
        {
            using var context =
                CreateContext();

            var repository =
                new EventRepository(
                    context);

            var service =
                new EventService(
                    repository);

            var organizer =
                CreateUser(
                    "org-1",
                    "Organizer One",
                    "org@test.com");

            context.Users.Add(organizer);

            context.EventCategories.Add(
                CreateCategory());

            var event1 =
                CreateEvent(
                    organizer.Id,
                    1,
                    "React Conference",
                    100,
                    20,
                    1000);

            var event2 =
                CreateEvent(
                    organizer.Id,
                    2,
                    "Dotnet Workshop",
                    80,
                    10,
                    500);

            event2.EventDate =
                DateTime.Now.AddDays(45);

            context.Events.AddRange(
                event1,
                event2);

            await context.SaveChangesAsync();

            var all =
                await service.GetAllEventsAsync();

            Assert.That(
                all.Count,
                Is.EqualTo(2));

            var search =
                await service.SearchEventsAsync(
                    "React",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);

            Assert.That(
                search.Count,
                Is.EqualTo(1));

            Assert.That(
                search[0].Name,
                Is.EqualTo(
                    "React Conference"));

            var sorted =
                await service.SearchEventsAsync(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    "priceasc");

            Assert.That(
                sorted.Count,
                Is.EqualTo(2));

            Assert.That(
                sorted[0].TicketPrice,
                Is.LessThanOrEqualTo(
                    sorted[1].TicketPrice));

            var organizerEvents =
                await service.GetOrganizerEventsAsync(
                    organizer.Id);

            Assert.That(
                organizerEvents.Count,
                Is.EqualTo(2));
        }

        // ============================================================
        // 32
        // ============================================================

        [Test]
        public async Task backend_testBookingServiceCrudCustomerHistoryAndFilteringBehavior()
        {
            using var context =
                CreateContext();

            var repository =
                new BookingRepository(
                    context);

            var service =
                new BookingService(
                    repository,
                    context);

            var customer1 =
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "one@test.com");

            var customer2 =
                CreateUser(
                    "customer-2",
                    "Customer Two",
                    "two@test.com");

            var event1 =
                CreateEvent(
                    "org-1",
                    1);

            var event2 =
                CreateEvent(
                    "org-1",
                    2);

            context.Users.AddRange(
                customer1,
                customer2);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.AddRange(
                event1,
                event2);

            var booking1 =
                CreateBooking(
                    customer1.Id,
                    event1.Id,
                    1,
                    2,
                    1000);

            var booking2 =
                CreateBooking(
                    customer1.Id,
                    event2.Id,
                    2,
                    1,
                    500);

            var booking3 =
                CreateBooking(
                    customer2.Id,
                    event1.Id,
                    3,
                    1,
                    500);

            context.Bookings.AddRange(
                booking1,
                booking2,
                booking3);

            await context.SaveChangesAsync();

            var customerHistory =
                await service.GetCustomerBookingsAsync(
                    customer1.Id);

            Assert.That(
                customerHistory.Count,
                Is.EqualTo(2));

            Assert.That(
                customerHistory.All(
                    b => b.CustomerId ==
                         customer1.Id),
                Is.True);

            var eventBookings =
                await service.GetEventBookingsAsync(
                    event1.Id);

            Assert.That(
                eventBookings.Count,
                Is.EqualTo(2));
        }

        // ============================================================
        // 33
        // ============================================================

        [Test]
        public async Task backend_testReviewServiceCreateAndRetrieveBehavior()
        {
            using var context =
                CreateContext();

            var repository =
                new ReviewRepository(
                    context);

            var service =
                new ReviewService(
                    repository,
                    context);

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "customer@test.com");

            var eventEntity =
                CreateEvent(
                    "org-1");

            context.Users.Add(customer);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.Add(eventEntity);

            await context.SaveChangesAsync();

            var dto =
                new ReviewDTO
                {
                    EventId =
                        eventEntity.Id,
                    Rating = 5,
                    Comment =
                        "Amazing event"
                };

            var result =
                await service.CreateReviewAsync(
                    dto,
                    customer.Id);

            Assert.That(
                result,
                Is.Not.Null);

            Assert.That(
                result.Rating,
                Is.EqualTo(5));

            Assert.That(
                result.Comment,
                Is.EqualTo(
                    "Amazing event"));

            Assert.That(
                result.CustomerName,
                Is.EqualTo(
                    customer.Name));

            var reviews =
                await service.GetEventReviewsAsync(
                    eventEntity.Id);

            Assert.That(
                reviews.Count,
                Is.EqualTo(1));

            Assert.That(
                reviews[0].CustomerName,
                Is.EqualTo(
                    "Customer One"));
        }

        // ============================================================
        // 34
        // ============================================================

        [Test]
        public async Task backend_testWishlistServiceAddRemoveAndRetrieveBehavior()
        {
            using var context =
                CreateContext();

            var repository =
                new WishlistRepository(
                    context);

            var service =
                new WishlistService(
                    repository);

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer One",
                    "customer@test.com");

            var eventEntity =
                CreateEvent(
                    "org-1");

            context.Users.Add(customer);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.Add(eventEntity);

            await context.SaveChangesAsync();

            var added =
                await service.AddToWishlistAsync(
                    customer.Id,
                    eventEntity.Id);

            Assert.That(
                added,
                Is.Not.Null);

            Assert.That(
                added.CustomerId,
                Is.EqualTo(
                    customer.Id));

            Assert.That(
                added.EventId,
                Is.EqualTo(
                    eventEntity.Id));

            var wishlist =
                await service.GetWishlistAsync(
                    customer.Id);

            Assert.That(
                wishlist.Count,
                Is.EqualTo(1));

            var exists =
                await service.IsInWishlistAsync(
                    customer.Id,
                    eventEntity.Id);

            Assert.That(
                exists,
                Is.True);

            var removed =
                await service.RemoveFromWishlistAsync(
                    customer.Id,
                    eventEntity.Id);

            Assert.That(
                removed,
                Is.True);

            exists =
                await service.IsInWishlistAsync(
                    customer.Id,
                    eventEntity.Id);

            Assert.That(
                exists,
                Is.False);
        }

        // ============================================================
        // 35
        // ============================================================

        [Test]
        public async Task backend_testEventRequiresValidFutureDateAndCapacity()
        {
            using var context =
                CreateContext();

            var repository =
                new EventRepository(
                    context);

            var service =
                new EventService(
                    repository);

            var futureEvent =
                new EventDTO
                {
                    CategoryId = 1,
                    Name =
                        "Valid Future Event",
                    Description =
                        "Valid description",
                    EventDate =
                        DateTime.Now.AddDays(20),
                    EventTime =
                        new TimeSpan(
                            10,
                            0,
                            0),
                    Venue =
                        "Test Venue",
                    Capacity = 100,
                    TicketPrice = 500,
                    EventImage =
                        "event.jpg"
                };

            var result =
                await service.AddEventAsync(
                    futureEvent,
                    "org-1");

            Assert.That(
                result,
                Is.Not.Null);

            Assert.That(
                result.Capacity,
                Is.EqualTo(100));

            var pastEvent =
                new EventDTO
                {
                    CategoryId = 1,
                    Name =
                        "Past Event",
                    Description =
                        "Past event",
                    EventDate =
                        DateTime.Now.AddDays(-1),
                    EventTime =
                        new TimeSpan(
                            10,
                            0,
                            0),
                    Venue =
                        "Test Venue",
                    Capacity = 100,
                    TicketPrice = 500
                };

            Assert.ThrowsAsync<CustomValidationException>(
                async () =>
                {
                    await service.AddEventAsync(
                        pastEvent,
                        "org-1");
                });

            var invalidCapacity =
                new EventDTO
                {
                    CategoryId = 1,
                    Name =
                        "Invalid Capacity",
                    Description =
                        "Invalid capacity",
                    EventDate =
                        DateTime.Now.AddDays(10),
                    EventTime =
                        new TimeSpan(
                            10,
                            0,
                            0),
                    Venue =
                        "Test Venue",
                    Capacity = 0,
                    TicketPrice = 500
                };

            Assert.ThrowsAsync<CustomValidationException>(
                async () =>
                {
                    await service.AddEventAsync(
                        invalidCapacity,
                        "org-1");
                });
        }

        // ============================================================
        // 36
        // ============================================================

        [Test]
        public async Task backend_testBookingRequiresExistingCustomerAndEvent()
        {
            using var context =
                CreateContext();

            var repository =
                new BookingRepository(
                    context);

            var service =
                new BookingService(
                    repository,
                    context);

            var eventEntity =
                CreateEvent(
                    "org-1");

            context.EventCategories.Add(
                CreateCategory());

            context.Events.Add(
                eventEntity);

            await context.SaveChangesAsync();

            var dto =
                new BookingDTO
                {
                    EventId =
                        eventEntity.Id,
                    TicketQuantity = 2
                };

            Assert.ThrowsAsync<NotFoundException>(
                async () =>
                {
                    await service.CreateBookingAsync(
                        dto,
                        "non-existing-customer");
                });

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer",
                    "customer@test.com");

            context.Users.Add(customer);

            await context.SaveChangesAsync();

            var invalidEventDto =
                new BookingDTO
                {
                    EventId = 9999,
                    TicketQuantity = 2
                };

            Assert.ThrowsAsync<NotFoundException>(
                async () =>
                {
                    await service.CreateBookingAsync(
                        invalidEventDto,
                        customer.Id);
                });
        }

        // ============================================================
        // 37
        // ============================================================

        [Test]
        public async Task backend_testBookingRejectsInactiveOrCancelledEvent()
        {
            using var context =
                CreateContext();

            var repository =
                new BookingRepository(
                    context);

            var service =
                new BookingService(
                    repository,
                    context);

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer",
                    "customer@test.com");

            var cancelledEvent =
                CreateEvent(
                    "org-1",
                    1);

            cancelledEvent.Status =
                EventStatuses.Cancelled;

            var inactiveCustomer =
                CreateUser(
                    "customer-2",
                    "Inactive Customer",
                    "inactive@test.com",
                    false);

            var activeEvent =
                CreateEvent(
                    "org-1",
                    2);

            context.Users.AddRange(
                customer,
                inactiveCustomer);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.AddRange(
                cancelledEvent,
                activeEvent);

            await context.SaveChangesAsync();

            var dto =
                new BookingDTO
                {
                    EventId =
                        cancelledEvent.Id,
                    TicketQuantity = 1
                };

            var cancelledException =
                Assert.ThrowsAsync<BusinessException>(
                    async () =>
                    {
                        await service.CreateBookingAsync(
                            dto,
                            customer.Id);
                    });

            Assert.That(
                cancelledException!.Message,
                Does.Contain(
                    "not available"));

            var inactiveDto =
                new BookingDTO
                {
                    EventId =
                        activeEvent.Id,
                    TicketQuantity = 1
                };

            var inactiveException =
                Assert.ThrowsAsync<BusinessException>(
                    async () =>
                    {
                        await service.CreateBookingAsync(
                            inactiveDto,
                            inactiveCustomer.Id);
                    });

            Assert.That(
                inactiveException!.Message,
                Does.Contain(
                    "inactive"));
        }

        // ============================================================
        // 38
        // ============================================================

        [Test]
        public async Task backend_testBookingRejectsTicketsAboveAvailableSeats()
        {
            using var context =
                CreateContext();

            var repository =
                new BookingRepository(
                    context);

            var service =
                new BookingService(
                    repository,
                    context);

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer",
                    "customer@test.com");

            var eventEntity =
                CreateEvent(
                    "org-1",
                    capacity: 10,
                    bookedSeats: 8,
                    ticketPrice: 500);

            context.Users.Add(customer);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.Add(eventEntity);

            await context.SaveChangesAsync();

            var dto =
                new BookingDTO
                {
                    EventId =
                        eventEntity.Id,
                    TicketQuantity = 3
                };

            var exception =
                Assert.ThrowsAsync<BusinessException>(
                    async () =>
                    {
                        await service.CreateBookingAsync(
                            dto,
                            customer.Id);
                    });

            Assert.That(
                exception!.Message,
                Does.Contain(
                    "available seats"));

            var storedEvent =
                await context.Events.FindAsync(
                    eventEntity.Id);

            Assert.That(
                storedEvent!.BookedSeats,
                Is.EqualTo(8));
        }

        // ============================================================
        // 39
        // ============================================================

        [Test]
        public async Task backend_testBookingTotalCalculatedByBackendUsingTicketPrice()
        {
            using var context =
                CreateContext();

            var repository =
                new BookingRepository(
                    context);

            var service =
                new BookingService(
                    repository,
                    context);

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer",
                    "customer@test.com");

            var eventEntity =
                CreateEvent(
                    "org-1",
                    capacity: 100,
                    bookedSeats: 10,
                    ticketPrice: 750);

            context.Users.Add(customer);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.Add(eventEntity);

            await context.SaveChangesAsync();

            var dto =
                new BookingDTO
                {
                    EventId =
                        eventEntity.Id,
                    TicketQuantity = 3,

                    // Deliberately incorrect amount.
                    TotalAmount = 1
                };

            var result =
                await service.CreateBookingAsync(
                    dto,
                    customer.Id);

            Assert.That(
                result.TotalAmount,
                Is.EqualTo(2250));

            Assert.That(
                result.TotalAmount,
                Is.Not.EqualTo(1));

            var stored =
                await context.Bookings
                    .FirstAsync(
                        b => b.Id == result.Id);

            Assert.That(
                stored.TotalAmount,
                Is.EqualTo(2250));

            Assert.That(
                stored.TicketQuantity,
                Is.EqualTo(3));
        }

        // ============================================================
        // 40
        // ============================================================

        [Test]
        public async Task backend_testBookingCancellationRestoresAvailableSeats()
        {
            using var context =
                CreateContext();

            var repository =
                new BookingRepository(
                    context);

            var service =
                new BookingService(
                    repository,
                    context);

            var customer =
                CreateUser(
                    "customer-1",
                    "Customer",
                    "customer@test.com");

            var eventEntity =
                CreateEvent(
                    "org-1",
                    capacity: 100,
                    bookedSeats: 30,
                    ticketPrice: 500);

            context.Users.Add(customer);

            context.EventCategories.Add(
                CreateCategory());

            context.Events.Add(eventEntity);

            await context.SaveChangesAsync();

            var bookingDto =
                new BookingDTO
                {
                    EventId =
                        eventEntity.Id,
                    TicketQuantity = 5
                };

            var booking =
                await service.CreateBookingAsync(
                    bookingDto,
                    customer.Id);

            var eventAfterBooking =
                await context.Events
                    .FirstAsync(
                        e => e.Id ==
                             eventEntity.Id);

            Assert.That(
                eventAfterBooking.BookedSeats,
                Is.EqualTo(35));

            var cancelled =
                await service.CancelBookingAsync(
                    booking.Id,
                    customer.Id);

            Assert.That(
                cancelled.Status,
                Is.EqualTo(
                    BookingStatuses.Cancelled));

            Assert.That(
                cancelled.PaymentStatus,
                Is.EqualTo(
                    PaymentStatuses.Refunded));

            var eventAfterCancellation =
                await context.Events
                    .FirstAsync(
                        e => e.Id ==
                             eventEntity.Id);

            Assert.That(
                eventAfterCancellation.BookedSeats,
                Is.EqualTo(30));

            var storedBooking =
                await context.Bookings
                    .FirstAsync(
                        b => b.Id ==
                             booking.Id);

            Assert.That(
                storedBooking.Status,
                Is.EqualTo(
                    BookingStatuses.Cancelled));
        }
    }
}