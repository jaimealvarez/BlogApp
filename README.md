Hi people from Zemoga!

This is my solution to the technical test

# INSTALLATION

**Software prerequisites**

* Windows 10 Home or Pro
* Visual Studio 2017 or newer
* Internet access 

**Steps to build**

The source code can be obtained using the provided zip file or fetched using any Git tool. The application includes all the libraries needed so it can be executed using F5 once the solution file is loaded. Steps:

1. Download, fetch from GitHub using the command:
```
git clone https://github.com/jaimealvarez/BlogApp.git
```	
2. Open the solution file BlogApp.sln from the /BlogApp folder using Visual Studio 2017 or newer.

3. The only additional component used in the solution is the InMemory database. If not already installed in Visual Studio this packaged can be easily obtained following these steps:

* In the Visual Studio menu go to Tools -> Manage Nuget Packages -> Manage NuGet Packages for the solution...
* Making sure that the package source nuget.org is configured, select Browse and enter Microsoft.EntityFrameworkCore.InMemory in the search box
* Select the package that appears (use the latest possible version)
* At the right hand side select BlogApp and click the Install button

4. Run the solution using F5.

**Time spent**

I spent approximately 12 hours to build, test and debug the application, including the time spent writing this document.

**Sample credentials**

There are two built-in users:

* username: writer, password: abc123, role: writer
* username: editor, password: abc123, role: editor


# SOLUTION TO PART I

**Access**

The application runs on the following url:

https://localhost:44381/

The web application was developed using .Net Core 3.1

**Home Page**

The home page shows the approved posts, if any.

**Users**

There are two built-in users:

username: writer, password: abc123, role: writer

username: editor, password: abc123, role: editor

When entering the application the Login option must be selected to access the protected functions of each role.
After the user logs in successfuly its name appears in the page header.

**Security**

Session control is implemented using the default CookieAuthentication provider of the .Net Core framework.
This provider offers enough functionality to fulfill the requirements of the test. 
As no cookie expiration time is configured the session will be kept alive until the user logs out.

**Writer requirements**

The initial view for the writer includes an option to create a post and shows a list of all her posts indicating whether they are created, submitted, rejected or published.
Unsubmitted or rejected posts have options to edit or submit.
Submitted or approved (published) posts cannot be edited or submitted again.

After the creation, edition or submition of a post the list updates automatically.


**Editor requirements**

The editor user is shown a list of pending posts. She has three options: approve, reject or delete.

When the editor approves a post it remains in the list of pending posts but cannot be accepted o rejected again.

When the editor rejects a post it dissapears from the list of pending posts and can be edited by the writer.

When the editor deletes a post it dissapears from the database.

The editor can delete submitted posts even after they are approved (published).

**Dependency Injection/IoC container**

The application uses the default IoC container of .Net Core which offers just enough functionality for the application.

All data access is abstracted in the EntityContext class which is an extension of DbContext.
This EntityContext is created as a cross-application service in the Startup class. 
The framework provides this EntityContext to all controllers through their constructor methods

**ORM**

The application uses the Microsoft.EntityFrameworkCore components as its ORM engine

**Database**

The application uses the Microsoft.EntityFrameworkCore.InMemory database as the data does not need to persist beyond this technical test.
This database is really simple to configure and access and offers all the features needed in this scenario.
There is no database script as all structures are automatically created in run-time.

# SOLUTION TO PART II

All endpoints can be accesed using any API testing tool like Postman or SOAPUI

**Query endpoint**

The query endpoint that returns the list of the posts that are pending for approval is located in the following URL: 

https://localhost:44381/api/Post/GetPendingPosts

This is a GET endpoint and does not require any parameter and returns a list of posts in JSON format.

**Approval endpoint**

The approval endpoint is located in the following URL:

https://localhost:44381/api/Post/{id}

Where \{id\} is the post id

This is a PUT endpoint and receives a JSON message with the following structure:

```
{
	"id":{id},
	"approved":true/false
}
```
