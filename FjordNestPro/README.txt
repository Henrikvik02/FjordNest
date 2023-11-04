==============================================================================
                         FJORDNESTPRO RENTAL PLATFORM
==============================================================================

Table of Contents
-----------------
1. Introduction
2. Architecture
   2.1 Models
   2.2 Views
   2.3 Controllers
   2.4 Data Access Layer
   2.5 Admin-Folder Areas
3. Database
4. Features
   4.1 CRUD Operations
   4.2 Repository Pattern
5. Narrative

==============================================================================
1. INTRODUCTION
------------------------------------------------------------------------------
Sjøutsikt, a bustling city, is now home to the FjordNestPro rental platform. 
The platform, developed using ASP.NET Identity, offers an organized interface 
for property listings, user roles, bookings, and reviews.

==============================================================================
2. ARCHITECTURE
------------------------------------------------------------------------------
2.1 MODELS
----------
Core and supporting models encapsulate FjordNestPro's data and business logic. 
Models such as ApplicationUser, Property, and Booking form the core foundation, 
while supporting models like ErrorViewModel and Question add additional layers.

2.2 VIEWS
----------
Designed with ASP.NET's Razor view engine, views dynamically generate content. 
Users navigate through multiple interfaces like 'Explore', 'Home', and 'Booking
Management', while Admins have a distinct dashboard and control modules.

2.3 CONTROLLERS
---------------
Controllers like HomeController, PropertyController, and BookingController 
manage user interactions and application flow.

2.4 DATA ACCESS LAYER
---------------------
A layer with classes for the database context and data repositories. It includes 
FjordNestProDbContext, IQuestionRepository, and QuestionRepository.

2.5 AREAS WITH ADMIN-FOLDER
---------------------------
Dedicated area providing specialized views and controllers for application
 management.

==============================================================================
3. DATABASE
------------------------------------------------------------------------------
The database supports models with tables for users, properties, bookings, 
reviews, and addresses.

==============================================================================
4. FEATURES
------------------------------------------------------------------------------
4.1 CRUD OPERATIONS
-------------------
The CRUD operations enable interaction with the database entities.

4.2 REPOSITORY PATTERN
----------------------
FjordNestPro uses the Repository pattern to abstract the data layer, promoting 
a decoupled design and easier testing.

==============================================================================
5. NARRATIVE
------------------------------------------------------------------------------
In Sjøutsikt's digital space, stories of users like Anniken, Bjørn, and
Christoffer come alive. Anniken, with her 'ADMIN' role, curates property
listings. Bjørn's "Sjøhytte" becomes a sought-after property, and
Christoffer's stay there turns into a memorable experience,reflected in his
glowing review. As the platform grows, more such tales emerge,
symbolizing the platform's success.


For a detailed narrative of FjordNestPro's user stories,please refer to
the 'narrative.txt' file.

==============================================================================
Thank you for using FjordNestPro!
==============================================================================
