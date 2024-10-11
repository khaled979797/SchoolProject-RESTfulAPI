# SchoolProject API
#### Mimicing a school system by building API web application.
The application adopts an N-tier architecture, leveraging ASP.NET Core 8, Entity Framework Core.
The RESTful API backend ensures efficient communication, while JWT authentication enhances security and XUnit covering the testing section.

## Technologies
* ASP.NET Core 8
* Entity Framework Core
* RESTful APIs
* MS SQL Server
* JWT Authentication
* Identity for User Management
* AutoMapper
* XUnit

## Architecture
SchoolProject API follows N-tier architecture, which includes:

* **Business Layer**: Implements core business logic.
* **Core Layer**: Utilizes the Mediator design pattern for efficient managing requests.
* **Service Layer**: Manage the requests from the core layer.
* **Infrastructure Layer**: Utilizes the Repository and design pattern for efficient data retrieval.
* **Data Layer**: Hold database models and other helping functionalities.
