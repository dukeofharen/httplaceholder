---
title: Tutorial: Build A URL Shortener with ASP.NET MVC
date: 2015-05-25T21:26:11+02:00
description: Since a few months, I'm a full time ASP.NET developer (using ASP.NET MVC & WebAPI) and thought it would be nice to write the tutorial one more time, but now using the Microsoft stack of technologies. I think the stack might be a bit hard for beginners (in comparison to Node.js / JavaScript), but I think the fact that you get to use Visual Studio, you have tape safety and compile time feedback makes up for that; I think you're way more productive on the long run. I think a URL shortener is complex enough to give you a global idea of the ASP.NET framework and gives you just the right knowledge to build something more complex.
---

**Updated on 2019-04-19**

Even though this code is very usable if you would like to learn about ASP.NET MVC, ASP.NET Core is the way to go for new projects. I might write this tutorial for ASP.NET Core at a certain point.

**[The code is available on GitHub](https://github.com/dukeofharen/shortnr.net)**

A few months ago, I've written a tutorial on how to create a URL shortener using Node.js, Express and MySQL (<a href="https://ducode.org/posts/url-shortener-in-node-js-express-js-mysql-tutorial-learning-by-doing/">/posts/url-shortener-in-node-js-express-js-mysql-tutorial-learning-by-doing/</a>). Since a few months, I'm a full time ASP.NET developer (using ASP.NET MVC & WebAPI) and thought it would be nice to write the tutorial one more time, but now using the Microsoft stack of technologies. I think the stack might be a bit hard for beginners (in comparison to Node.js / JavaScript), but I think the fact that you get to use Visual Studio, you have tape safety and compile time feedback makes up for that; I think you're way more productive on the long run. I think a URL shortener is complex enough to give you a global idea of the ASP.NET framework and gives you just the right knowledge to build something more complex.

A quote from my previous tutorial:

*I imagine that you know what a URL shortener is, but I'll explain anyway. Let's say we have a URL: ridiculouslylongurlimeanit.com/veryniceblogpost.html. This is a horrible URL to share on Twitter or even on a forum. It would be a lot nicer to have this URL shortened: e.g. d.co/1sk59. This is a nicer URL to share.*

In this tutorial, I use several techniques and tools.

*   **ASP.NET MVC**: Microsoft's modern web application framework (which I think will replace Web Forms in the future). As the name says, it pushes you to use the MVC (model view controller) software design principle.
*   **ASP.NET Web API**: Web API and MVC are used together in many applications. With MVC, the HTML of the web pages are rendered on the server, and with Web API you can, like the name says, create an API. Web API also uses the MVC principle, but returns XML (or JSON, or YAML, or ... whatever you want) instead of HTML.
*   **MySQL**: this is my primary database choice. You could also use MSSQL Server, but I will use MySQL for this tutorial.
*   **Entity Framework**: this is my favourite ORM (object relational mapping) framework. With Entity Framework, you can create a database "code-first", meaning that you can create classes (called entities) that represent the database and create the database based on those entities and its relations. When you've updated one of these entities, you can add a new migration, which means that the changes to your entities will be written to the database.
*   **Unity**: Unity is a dependency injection framework. You can read more on dependency injection and inversion of control later in this tutorial.

## Getting started

The first thing you'll need to do (if you haven't done this already) is install Visual Studio. Since about a year, Visual Studio 2013 Community edition is available. This is like the Pro version, but free to use for study, open source projects and small scale business applications. You can download it here: <https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx>. You also have Visual Studio Express, you can use that if you want, but it's highly inferior to the Community edition, in that it doesn't support plugins and comes in different editions (one for Windows Phone, one for Web, one for Desktop etc.). Everything is incorporated in the Community edition.

Secondly, also if you haven't done it already, you have to install MySQL. You can download the complete installer from the MySQL website (<http://dev.mysql.com/downloads/mysql/>), or you can download and install XAMPP (which I like, because it also includes Apache & PHP, so also phpMyAdmin, a nice interface for managing your database). You can download it from here: <https://www.apachefriends.org/index.html>.

**Note**<br />
A basic understanding of the .NET framework couldn't hurt you. If you're an absolute beginner and don't know some of the terminology or if I've forgotten something, don't hesitate to leave a comment below; I'm always happy to help.

## Basic project structure

If you didn't know already, Visual Studio works with solutions. A solution can contain multiple projects. When a solution is compiled, a DLL (or EXE if it's a WPF or Console application) is created. These DLLs, in case of an MVC application, are deployed to the server. A project can reference another project in the same solution, or assemblies compiled from another solution. A third way of adding references is using NuGet; this is a package manager for ASP.NET applications. Using this, you can add a connector to MySQL, Entity Framework, xUnit.net (testing framework) and many, many more to your solution. You can compare it to Composer (PHP) or npm (Node.js).

Once you've started Visual Studio, go to "File" => "New" => "Project". I always select "ASP.NET Web Application". In the following dialog, select "MVC", but also select "Web API". On the right hand, change authentication to "No Authentication", since we're not going to use that (not for this tutorial though, maybe later). In the bottom fields, you can fill in the name and location of your solution. I call it "Shortnr.Web" (but call it anything you want). You can uncheck "Host in the cloud", although I'm not sure what the difference is (I never hosted anything in Azure). You can now click OK.

A basic MVC site is created now. Once you click the "Run" button at the top, you'll already be able to browse the basic website. A local IIS web server is launched and your favourite browser will be opened with the MVC website.

![img1](/static/img/aspnet-shortener/img1.png)

![img3](/static/img/aspnet-shortener/img3.png)

As you can see, there are some files and folders created for you in this project. I'm going to explain what these folders and files are.

![img2](/static/img/aspnet-shortener/img2.png)

*   **App_Data**: This folder is empty for now, but if you ever want to enable uploads on your websites, it's best if they are placed here.
*   **App_Start**: this golder contains some classes which are needed to start up MVC and Web API when the application is first run. If you add new frameworks which need something of a setup, this can be placed in this folder.
*   **Content**: this folder contains your CSS files and images.
*   **Controllers**: this folder contains the controller classes of the MVC application.
*   **fonts**: as the name says, this folder contains fonts.
*   **Models**: this folder contains models which will be used to pass data from the controllers to the views.
*   **Scripts**: this folder contains (Java)script files.
*   **Views**: this folder contains the views for the MVC application.
*   **Global.asax**: this file is loaded when the application is started. In this file, the classes in the "App_Start" folder are called.
*   **packages.config**: if you're going to add packages from NuGet to your project, a reference to that project will be added to this file. When someone else receives your code and tries to build our code, Visual Studio first downloads all packages defined in this file (else the build would fail because these DLLs aren't available).
*   **Web.config**: this file contains general configuration for your application. Think of database connection strings, SMTP server settings etc.

First, let's open HomeController.cs in the folder Controllers. You'll see this code:

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;

	namespace Shortnr.Web.Controllers
	{
		public class HomeController : Controller
		{
			public ActionResult Index()
			{
				return View();
			}

			public ActionResult About()
			{
				ViewBag.Message = "Your application description page.";

				return View();
			}

			public ActionResult Contact()
			{
				ViewBag.Message = "Your contact page.";

				return View();
			}
		}
	}

This method contains three methods. When the application is started, RouteConfig (which is located in App_Start) is read and mapped to the existing controllers. In this application it means that when you go to http://yourapp/Home/About, the method About in the HomeController will be executed. This is called "convention over configuration"; you don't have to explicitly tell the application that you have added a new controller. This means that when you add a new controller, say UrlController with a method Shorten, this will be called when you go to http://yourapp/Url/Shorten.

You also see that the methods in this class return a View(). For exemple, when you browse to /Home/About, the View() searches in the folder Views to the file About.cshtml (can be another extension, but the file should be called "About"). Again, this is "convention over configuration".

Every method in this controller returns an ActionResult. An ActionResult can be a view, but can also be a redirect (which we'll use later when redirecting a short URL).

This default behaviour can be finetuned, but I think that's not necessary for now; I think it works fine this way.

Now that's explained, let's create a new controller. Right click the "Controllers" folder and create a new controller (MVC 5 Controller - Empty). I call it UrlController.

![Create a new controller](/static/img/aspnet-shortener/img4.png)

![Select empty controller](/static/img/aspnet-shortener/img5.png)

By adding a new controller, a new folder is also added in the "Views" folder; "Url". In this folder, create a new view called "Index.cshtml". This is the code for the view:

	@{
		ViewBag.Title = "URL shortener";
	}

	<h2>Index</h2>

	This is the main URL shortening view. Here, we'll show our form later on.

On the top of the view, you see a syntax you may have never seen before. This is MVC's template syntax: Razor. The property "Title" of "ViewBag" is set to "URL shortener". ViewBag is used to pass data from one view to another. In this case, the title will be rendered between the title tags of the master layout.

If you start the application now and head to /Url/Index (or just /Url, because Index is assumed if the second part isn't set; see RouteConfig.cs), you'll see our newly created view rendered. Lateron, this view will contain the form where users can fill in their long URL to be shortened, so when the root URL is reached, we would like the user to see this page, we don't want them to go to http://yourapp/Url. To accomplish this open RouteConfig.cs.

	public static void RegisterRoutes(RouteCollection routes)
	{
		routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

		routes.MapRoute(
			name: "Default",
			url: "{controller}/{action}/{id}",
			defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
		);
	}

This is the default route configuration. You'll see that by default the HomeController and Index action are selected. Change "Home" to "Url". Now, when we go to the root URL, we'll see our newly created view.

This view is a bit empty for now. First, add a new class called "Url" to the folder "Models". Like I've said before, a model is responsible for passing data between a View and a Controller. Below, you'll see the model I've created:

	public class Url
	{
		public string LongURL { get; set; }

		public string ShortURL { get; set; }
	}

The model will contain the long URL and the short URL for now. When the user has filled in the URL he / she would like to have shortened, the LongURL property will be set and it will be sent to the business layer (which we will create lateron). The business layer will validate the URL and will return a short URL. The short URL will be set and the model will be returned to the view.

Now, let's add a form to Index.cshtml.

	@model Shortnr.Web.Models.Url
	@{
		ViewBag.Title = "URL shortener";
	}

	<h2>Shorten a URL</h2>

	@using (Html.BeginForm())
	{
		<div class="form-group">
			@Html.TextBoxFor(model => model.LongURL, new { placeholder = "URL you would like to have shortened", @class = "form-control" })
		</div>
		
		<input type="submit" class="btn btn-primary" value="Shorten" />
	}

As you can see, I've added a "@model" reference at the top. The view now knows that it should use the class we've just created (you can call the model anywhere by using @Model in the view). Further on, you see we start a new HTML form.  In this form we start a new TextBox, with a reference to the LongURL property in the class "Url". The second parameter in this method is an anonymous object with several HTML attributes, like placeholder and class (class is prefixed with an "@" because it is a reserved keyword in ASP.NET, the application won't work otherwise). The last line is a plain old HTML submit button.

![Our form](/static/img/aspnet-shortener/img6.png)

All very nice, but there is no logic at all at the moment. Let's go back to UrlController.cs.

	[HttpGet]
	public ActionResult Index()
	{
		Url url = new Url();
		return View(url);
	}

	public ActionResult Index(Url url)
	{
		url.ShortURL = "http://www.google.com";
		return View(url);
	}

You now see that I've created two methods with the same name. One which doesn't have any parameters, one with a Url object as parameter. The first method will only be called when the user is first directed to the page. The second method is used when the user has pushed the submit button. MVC will serialize the data filled in on the form, place it in a URL object and pass it to that function. Here, we will be able read the original LongURL property and send it to the business layer. As of now, nothing happens with it and the ShortURL property is set to "http://www.google.com" hard coded. This is fine for now. The object with the ShortURL property set is being passed to the view, so we can read this property in the view now. If you place the snippet below in "Index.cshtml" underneath the textbox, you'll see the shortened URL when you push the submit button.

	if (!string.IsNullOrEmpty(Model.ShortURL))
	{
		<div>
			<a href="@Model.ShortURL" target="_blank">@Model.ShortURL</a>
		</div>
	}

It would be nice to have a little validation. For now, it's enough to validate that the user has actually filled in anything as long URL. So go back to "Url.cs" and change LongURL to this:

	[Required]
	public string LongURL { get; set; }

By placing this attribute directly above this property, MVC knows that this property should be set. Next, change the second Index method in UrlController.cs to this:

	public ActionResult Index(Url url)
	{
		if (ModelState.IsValid)
		{
			url.ShortURL = "http://www.google.com";
		}
		return View(url);
	}

ModelState.IsValid checks if all validation requirements are met. If yes, set the ShortURL. Finally, we would like the user to get validation feedback. In "Index.cshtml", place this piece of code anywhere you'd like (I place it directly beneath the H2 tags):

	@Html.ValidationSummary()

At this point, I deleted the HomeController and Home folder in the Views folder; we don't need it for the now.

Now, it's time to set up the other projects. Right click the solution and add new projects. The project I'm describing should be of the type "Class Library".

![Add a new project](/static/img/aspnet-shortener/img7.png)

![Class library](/static/img/aspnet-shortener/img8.png)

*   **Shortnr.Web.Business**: this project will contain the interfaces and classes needed to execute numerous business actions; for example adding a new short URL to the database, searching a short URL etc.
*   **Shortnr.Web.Data**: this project will contain our data context for Entity Framework and the migrations will be saved in this project.
*   **Shortnr.Web.Entities**: this project will contain the entities (plain old classes) for our data structure.
*   **Shortnr.Web.Exceptions**: this project will contain custom exceptions. For example, when a specific URL isn't found, an exception will be thrown and it will be caught by the MVC framework to show us a nice custom error page.

So, now we've created all project we're going to need to add several NuGet packages to the projects.

![Go to NuGet package manager](/static/img/aspnet-shortener/img9.png)

Search and install the following packages. I'm going to describe in which projects every package has to be installed to.

![NuGet package manager](/static/img/aspnet-shortener/img10.png)

*   **EntityFramework**: the ORM which we're going to use.
    *   Business
    *   Data
    *   Entities
    *   Web
*	**MySql.Data.EntityFramework**: the MySQL driver and the connector for Entity Framework.
     *   Business
     *   Data
     *   Entities
     *   Web
*	**Json.NET**: a nice Json serializer for ASP.NET. This will be needed by Web API later on.
     *   Business
     *   Data
     *   Entities
     *   Web
*	**Unity**, **Unity.Mvc** & **Unity.AspNet.WebApi**: this is an inversion of control (IoC) for ASP.NET. It is used for loose coupled web applications. I will explain this later on.
     *   Web

When installing Unity for both Web API and MVC, you might get a file conflict. Just press overwrite. You might get some errors regarding a method that doesn't exist. Replace the class UnityConfig with this class:

	public static class UnityConfig
	{
		private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
		{
			var container = new UnityContainer();
			RegisterTypes(container);
			return container;
		});

		public static IUnityContainer GetConfiguredContainer()
		{
			return container.Value;
		}

		public static void RegisterTypes(IUnityContainer container)
		{
		}
	}

The solution should be able to build now. If not, please leave a comment and maybe I can help you.

## Dependency injection

Robust applications don't have any hard coupling in their code. What I always like to do is constructor injection; whenever a controller (or any other class) is instantiated, you can fill in a few parameters in the constructor. These parameters are interfaces. Unity, an inversion of control framework, finds out which implementation belongs to this interface, and injects it. WIth this framework, you don't have hard coupling; there is only one place in your application where you fill in this interface / implementation mapping. Whenever you need to change the implementation (for example, you used Entity Framework, but want to switch to NHibernate), you just create a new class that implements that specific interface and you change the configuration for Unity. It might all sound a bit vague. Let's try to setup Unity.

### 1. The web project should reference all other projects

Right click the web project and add a new reference.

![Add a new reference](/static/img/aspnet-shortener/img11.png)

![img12-1](/static/img/aspnet-shortener/img12-1.png)

### 2. Add an interface IUrlManager and a class UrlManager (which implements IUrlManager)

*IUrlManager*

	public interface IUrlManager
	{
		Task<string> ShortenUrl(string longUrl);
	}

*UrlManager*

	public class UrlManager : IUrlManager
	{
		public Task<string> ShortenUrl(string longUrl)
		{
			return Task.Run(() =>
			{
				return "http://www.google.com";
			});
		}
	}

You'll see here that a short URL is still returned hard coded. We don't have a database connection yet, so the real implementation will come later. This is fine for now.

![The Business project as of now](/static/img/aspnet-shortener/img13.png)

We have to tell the application somehow that when an implementation for IUrlManager is desired, a UrlManager should be injected. The method RegisterTypes in the class UnityConfig will look like this now:

	public static void RegisterTypes(IUnityContainer container)
	{
		container.RegisterType<IUrlManager, UrlManager>();
		GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
	}

### 3. Update the existing UrlController

Let's take a look at the new UrlController:

	public class UrlController : Controller
	{
		private IUrlManager _urlManager;

		public UrlController(IUrlManager urlManager)
		{
			this._urlManager = urlManager;
		}

		[HttpGet]
		public ActionResult Index()
		{
			Url url = new Url();
			return View(url);
		}

		public async Task<ActionResult> Index(Url url)
		{
			if (ModelState.IsValid)
			{
				url.ShortURL = await this._urlManager.ShortenUrl(url.LongURL);
			}
			return View(url);
		}
	}

As you see here, we've added a private field and a constructor. When the controller is selected, Unity knows that it should insert the UrlManager in the IUrlManager. We have no hard coupling on the implementation at the moment. The second Index method is now async, and returns a Task. This is because our final implementation of the UrlManager will call the database and check if the inserted URL actually exists. If this isn't executed async, it will block the entire application until these actions are done; that's something you don't want.

Every new business manager you're going to add, can be injected using Unity.

## Entity Framework

As I've explained before, Entity Framework is a object relational mapping framework. You define a few classes with a few properties. These properties match the fields in the database. Before we can do anything with Entity Framework, we have to make set up the MySQL connection in the Web.config file. The code snippet below should be inserted in the configuration tag of Web.config:

	<connectionStrings>
		<add name="Shortnr" connectionString="Server=localhost;Database=shortnr;Uid=root;Pwd=password;" providerName="MySql.Data.MySqlClient" />
	</connectionStrings>

Make sure you put in the correct username (Uid) and password (Pwd).

Let's add two entities (so two tables) for the URL shortener application to the Entities project. These are just plain classes.

*ShortUrl.cs*

	[Table("short_urls")]
	public class ShortUrl
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Required]
		[Column("long_url")]
		[StringLength(1000)]
		public string LongUrl { get; set; }

		[Required]
		[Column("segment")]
		[StringLength(20)]
		public string Segment { get; set; }

		[Required]
		[Column("added")]
		public DateTime Added { get; set; }

		[Required]
		[Column("ip")]
		[StringLength(50)]
		public string Ip { get; set; }

		[Required]
		[Column("num_of_clicks")]
		public int NumOfClicks { get; set; }

		public Stat[] Stats { get; set; }
	}

*Stat.cs*

	[Table("stats")]
	public class Stat
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Required]
		[Column("click_date")]
		public DateTime ClickDate { get; set; }

		[Required]
		[Column("ip")]
		[StringLength(50)]
		public string Ip { get; set; }

		[Column("referer")]
		[StringLength(500)]
		public string Referer { get; set; }

		public ShortUrl ShortUrl { get; set; }
	}

This is a very basic entity setup.

*   **Table** tells Entity Framework what the actual table name should be.
*   **Key** tells Entity Framework that this property is the primary key.
*   **Column** tells Entity Framework what the columns name is in the database.
*   **StringLength** tells Entity Framework what the maximum string length of a property is (only if the type is "string").

**Note**<br />
Make sure the Data project references the Entities project.

This actually doesn't do anything. We have to define a "data context". The data context is the central piece in Entity Framework: it contains the relations between the different entities and contains the repositories. A repository is a collection of all records in a specific table mapped to a specific entity. Let's add a ShortnrContext to the Data project.

	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class ShortnrContext : DbContext
	{
		public ShortnrContext()
			: base("name=Shortnr")
		{

		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Stat>()
				.HasRequired(s => s.ShortUrl)
				.WithMany(u => u.Stats)
				.Map(m => m.MapKey("shortUrl_id"));
		}

		public virtual DbSet<ShortUrl> ShortUrls { get; set; }
		public virtual DbSet<Stat> Stats { get; set; }
	}

As you can see above, we have a hard coupling with MySqlEFConfiguration; for some reason, if I don't do this, it just won't work. The string "name=Shortnr" in the constructor refers to the connection string in the Web.config file. The method OnModelCreating is where all the relations between the entities are configured. At the moment, there is only one relation, so not much going on there. The latest two properties are the repositories. Entity Framework knows that these should be filled with the correct entities.

Now that we have the entities and the database configured, it's time to set up our first migration. A migration is a change to the database. When you add a migration, Entity Framework compares the database with your current entity configuration and creates a new migration. This new migration can then be pushed to the database.

First, we have to open the package manager console.

![Open the package manager console](/static/img/aspnet-shortener/img14.png)

Make sure the default project is "Shortnr.Web.Data".

![Set the default project](/static/img/aspnet-shortener/img15.png)

Next, execute the command "enable-migrations".

![Enable the migrations](/static/img/aspnet-shortener/img16.png)

A migrations configuration file will be added to the Data project. From now on, it will be possible to add new migrations. Execute the following command:

	add-migration "InitialCreate"

This will add an initial migration to your Data project. When you execute the following command:

	update-database

The migration will actually be written to the database. If everything went right, you'll now see the created (but empty) tables in the newly created database, using your favourite MySQL management tool.

## Implementing the business layer

**Note**<br />
Make sure the Business project references the Data, Entities and Exceptions projects.

First, make sure the Exceptions project contains these two classes:

*ShortnrConflictException*

	public class ShortnrConflictException : Exception
	{
	}

*ShortnrNotFoundException*

	public class ShortnrNotFoundException : Exception
	{
	}

Earlier in this tutorial, we've implemented a stub for a business manager. The manager actually doesn't do anything, but since we've implemented the database layer, it's now possible to do some advanced stuff. Below you'll see the new IUrlManager and UrlManager.

*IUrlManager*

	public interface IUrlManager
	{
		Task<ShortUrl> ShortenUrl(string longUrl, string ip, string segment = "");
		Task<Stat> Click(string segment, string referer, string ip);
	}

*UrlManager*

	public class UrlManager : IUrlManager
	{
		public Task<ShortUrl> ShortenUrl(string longUrl, string ip, string segment = "")
		{
			return Task.Run(() =>
			{
				using (var ctx = new ShortnrContext())
				{
					ShortUrl url;

					url = ctx.ShortUrls.Where(u => u.LongUrl == longUrl).FirstOrDefault();
					if (url != null)
					{
						return url;
					}

					if (!string.IsNullOrEmpty(segment))
					{
						if (ctx.ShortUrls.Where(u => u.Segment == segment).Any())
						{
							throw new ShortnrConflictException();
						}
					}
					else
					{
						segment = this.NewSegment();
					}

					if (string.IsNullOrEmpty(segment))
					{
						throw new ArgumentException("Segment is empty");
					}

					url = new ShortUrl()
					{
						Added = DateTime.Now,
						Ip = ip,
						LongUrl = longUrl,
						NumOfClicks = 0,
						Segment = segment
					};

					ctx.ShortUrls.Add(url);

					ctx.SaveChanges();

					return url;
				}
			});
		}

		public Task<Stat> Click(string segment, string referer, string ip)
		{
			return Task.Run(() =>
			{
				using (var ctx = new ShortnrContext())
				{
					ShortUrl url = ctx.ShortUrls.Where(u => u.Segment == segment).FirstOrDefault();
					if (url == null)
					{
						throw new ShortnrNotFoundException();
					}

					url.NumOfClicks = url.NumOfClicks + 1;

					Stat stat = new Stat()
					{
						ClickDate = DateTime.Now,
						Ip = ip,
						Referer = referer,
						ShortUrl = url
					};

					ctx.Stats.Add(stat);

					ctx.SaveChanges();

					return stat;
				}
			});
		}

		private string NewSegment()
		{
			using (var ctx = new ShortnrContext())
			{
				int i = 0;
				while (true)
				{
					string segment = Guid.NewGuid().ToString().Substring(0, 6);
					if (!ctx.ShortUrls.Where(u => u.Segment == segment).Any())
					{
						return segment;
					}
					if (i > 30)
					{
						break;
					}
					i++;
				}
				return string.Empty;
			}
		}
	}

You'll see two new methods here: Click and NewSegment. The Click method will be executed any time anyone clicks on a short URL; some data (like the refering website) will be stored in the database. The NewSegment method creates a unique segment for our short URL. If it hasn't found a valid segment in 30 loops, an empty string will be returned (this situation will be very rare though). The ShortenUrl method now actually shortens a long URL and stores it, together with a generated segment, in the database.

As you can see, any time a database action is executed, a new context object is created. When there are changes to the data, SaveChanges() is executed on the context at the end.

## Putting it in the UrlController

Now we have a working business layer, we can call the business methods in the URL controller. Below, you see the updated Index() method:

	public async Task<ActionResult> Index(Url url)
	{
		if (ModelState.IsValid)
		{
			ShortUrl shortUrl = await this._urlManager.ShortenUrl(url.LongURL, Request.UserHostAddress);
			url.ShortURL = string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"), shortUrl.Segment);
		}
		return View(url);
	}

The controller asks the UrlManager for a new ShortUrl. When all went well, a full URL with the segment at the end will be created. At the moment there is only one problem; when we navigate to that URL, nothing happens, so we have to implement another method in the UrlController which handles the redirects. You'll see this method below:

	public async Task<ActionResult> Click(string segment)
	{
		string referer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : string.Empty;
		Stat stat = await this._urlManager.Click(segment, referer, Request.UserHostAddress);
		return this.RedirectPermanent(stat.ShortUrl.LongUrl);
	}

Ain't much, is it? The only thing we have to do now is tell MVC that when we go to http://yourapp/segment, we wind up in that specific method. Below, you'll see the new RegisterRoutes method of the class RouteConfig.

	public static void RegisterRoutes(RouteCollection routes)
	{
		routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

		routes.MapRoute(
			name: "Click",
			url: "{segment}",
			defaults: new { controller = "Url", action = "Click" }
		);

		routes.MapRoute(
			name: "Default",
			url: "{controller}/{action}/{id}",
			defaults: new { controller = "Url", action = "Index", id = UrlParameter.Optional }
		);
	}

It's very important that the "Click" route stands above the "Default" route.

Another nice thing our URL manager supports is adding a custom segment, for example http://yourapp/mysite. We have to modify three things for that:

### 1. Update the Url model

	public class Url
	{
		[Required]
		public string LongURL { get; set; }

		public string ShortURL { get; set; }

		public string CustomSegment { get; set; }
	}

### 2. Update the Index method

	public async Task<ActionResult> Index(Url url)
	{
		if (ModelState.IsValid)
		{
			ShortUrl shortUrl = await this._urlManager.ShortenUrl(url.LongURL, Request.UserHostAddress, url.CustomSegment);
			url.ShortURL = string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"), shortUrl.Segment);
		}
		return View(url);
	}

### 3. Update the view

	@model Shortnr.Web.Models.Url
	@{
		ViewBag.Title = "URL shortener";
	}

	<h2>Shorten a URL</h2>

	@Html.ValidationSummary()

	@using (Html.BeginForm())
	{
		<div class="form-group">
			@Html.TextBoxFor(model => model.LongURL, new { placeholder = "URL you would like to have shortened", @class = "form-control" })
		</div>

		<div class="form-group">
			@Html.TextBoxFor(model => model.CustomSegment, new { placeholder = "If you like, fill in a custom word for your short URL", @class = "form-control" })
		</div>

		if (!string.IsNullOrEmpty(Model.ShortURL))
		{
			<div>
				<h3><a href="@Model.ShortURL" target="_blank">@Model.ShortURL</a></h3>
			</div>
		}
		
		<input type="submit" class="btn btn-primary" value="Shorten" />
	}

After this is done, it looks like we have a basic, and functioning, URL shortener. We still have to add some error handling; when an error is thrown from within the business layer, a big old ugly error page will be shown to the user. This is not something we want, this is what we're going to fix next.

![Our URL shortener](/static/img/aspnet-shortener/img17.png)

## Error handling

Right now, we have two custom exception classes: ShortnrConflictException (when a segment already exists) and ShortnrNotFoundException (when a segment isn't found in the database). There is also a third type: an unexpected exception, for example when there is no database connection. Normally, the user will see these errors. We have to build a mechanism where these exceptions are caught and a nice error page is shown, corresponding to the type of error, with the corresponding HTTP status code. We need to add a filter to the Web project. Add a new folder called "Filters" and add a "ShortnrExceptionFilter" class.

	public class ShortnrErrorFilter : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
			HttpStatusCode code = HttpStatusCode.InternalServerError;
			var ex = filterContext.Exception;
			string viewName = "Error500";

			if (ex is ShortnrNotFoundException)
			{
				code = HttpStatusCode.NotFound;
				viewName = "Error404";
			}
			if (ex is ShortnrConflictException)
			{
				code = HttpStatusCode.Conflict;
				viewName = "Error409";
			}

			filterContext.Result = new ViewResult()
			{
				ViewName = viewName
			};

			filterContext.ExceptionHandled = true;
			filterContext.HttpContext.Response.Clear();
			filterContext.HttpContext.Response.StatusCode = (int)code;
			filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;  
		}
	}

This filter extends from the HandleErrorAttribute, and overrides the OnException method. In this method we do our own exception handling. If the exception is one of our own exceptions, a 404 or 409 is returned, else a 500 is returned. At the bottom of the method we tell the context that the exception is handled, and the current response (the ugly error page) will be cleared and our custom views (which are defined below) will be returned.

Add the following views in the "Views\Shared" folder:

*Error404.cshtml*

	@{
		ViewBag.Title = "Not Found";
	}

	<h2>Not Found</h2>

	<p>The resource you've requested isn't found.</p>

*Error409.cshtml*

	@{
		ViewBag.Title = "Conflict";
	}

	<h2>Conflict</h2>

	<p>The name you've chosen already exists.</p>

*Error500.cshtml*

	@{
		ViewBag.Title = "Error";
	}

	<h2>Error</h2>

	<p>An unexpected error occured.</p>

The only thing we have to do now is add the filter to our FilterConfig class. In the FilterConfig class, replace the existing method with this method:

	public static void RegisterGlobalFilters(GlobalFilterCollection filters)
	{
		filters.Add(new ShortnrErrorFilter());
	}

Now, when there is an exception, a nice error page will be shown.

## Web API integration

At the moment, we have a nice and working URL shortnerer built in ASP.NET MVC. One thing which would be nice to do is add the possibility for other people to integrate your URL shortener in their application. This means that when they call a specific URL (say http://yourapp/api/url/shorten?url=https%3A%2F%2Fducode.org), they get a shortened URL in JSON format. This API part can easily be created using ASP.NET Web API. We already included Web API in our project, so let's get started. Add a new folder called `ApiControllers` to the Shortnr.Web project. Add a new controller to this folder. Pick Web API 2 Controller - Empty and call it `UrlController`. Below you'll see the complete UrlController.

	[RoutePrefix("api/url")]
    public class UrlController : ApiController
    {
		private IUrlManager _urlManager;

		public UrlController(IUrlManager urlManager)
		{
			this._urlManager = urlManager;
		}

		[Route("shorten")]
		[HttpGet]
		public async Task<Url> Shorten([FromUri]string url, [FromUri]string segment = "")
		{
			ShortUrl shortUrl = await this._urlManager.ShortenUrl(HttpUtility.UrlDecode(url), HttpContext.Current.Request.UserHostAddress, segment);
			Url urlModel = new Url()
			{
				LongURL = url,
				ShortURL = string.Format("{0}://{1}/{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, shortUrl.Segment)
			};
			return urlModel;
		}
    }

Pretty small, no? As you can see, I've used the attributes `RoutePrefix` and `Route`. This is called *attribute based routing*; you explicitly tell the controller on which URL the actions are reachable. In this case, if we call the url http://yourapp/api/url/shorten?url=myurl, the method Shorten will be called and we get a short URL in return. The URL parameters `url` and `segment` are read from the URL by Web API and passed to the method. [FromUri] means that the parameter can only be read from the URL (and not, for example, from the POST data). The parameter `url` is mandatory and the parameter `segment` optional (just like on the form we've created earlier).

Just as with the MVC part, the UrlManager is injected into this controller, so we call exact the same business manager.

A last thing; the URL which should be shortened has to be encoded, or else the application might behave unexpected. The URL is decoded once it is send to the ShortenUrl method in the business manager.

The method Register in the class WebApiConfig should look like this:

	public static void Register(HttpConfiguration config)
	{
		// Web API configuration and services

		// Web API routes
		config.MapHttpAttributeRoutes();
	}

MapHttpAttributeRoutes() means that the routes `RoutePrefix` and `Route` are read when the application is started, so the application knows what to do when those specific routes are reached.

The class `Url` in the folder models should also be changed. You see the result below:

	public class Url
	{
		[Required]
		[JsonProperty("longUrl")]
		public string LongURL { get; set; }

		[JsonProperty("shortUrl")]
		public string ShortURL { get; set; }

		[JsonIgnore]
		public string CustomSegment { get; set; }
	}

I've added three new attributes to the existing properties. JsonProperty and JsonIgnore are attributes from the Json.NET library. Whenever Web API wants to serialize this specific model to JSON, it now knows what to call each property. JsonIgnore means that this property is ignored (so not returned with the result) once the model is serialized.

So, now when I make a request to the URL, with headers **Content-Type** and **Accept** set to **application/json**, I get the result below:

![The Short URL JSON result](/static/img/aspnet-shortener/img18.png)<br />
*Request made with [RESTClient](https://addons.mozilla.org/en-US/firefox/addon/restclient/) for Firefox*

We use the same business manager in the Web API part and the MVC part. The business manager throws exceptions when things go wrong; e.g. when you supply a URL which doesn't exist or when you supply a custom segment which already exists. We've written a filter for MVC, but we should also do this for Web API.

Create a file called ShortnrApiErrorFilter in the Filters folder in the Shortnr.Web folder.

Below, you'll see the ShortnrApiErrorFilter class:

	public class ShortnrApiErrorFilter : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext ctx)
		{
			HttpStatusCode code = HttpStatusCode.InternalServerError;
			var ex = ctx.Exception;

			if (ex is ShortnrConflictException)
			{
				code = HttpStatusCode.Conflict;
			}
			else if (ex is ShortnrNotFoundException)
			{
				code = HttpStatusCode.NotFound;
			}

			ctx.Response = ctx.Request.CreateResponse(code);
		}
	}

**Note**<br />
The CreateResponse method is an extension in the System.Net.Http class, make sure you make a reference to it.

As you see above, the HTTP error code is 500 on default. When there is a conflict (e.g. the segment already exists) a HTTP conflict (409) will be returned. When something isn't found (e.g. the URL to be shortened), a HTTP not found (404) will be returned.

We now have a nice filter, but it isn't called yet when there is actually an error. Change the method Register in the class WebApiConfig to the code below:

	public static void Register(HttpConfiguration config)
	{
		// Web API configuration and services
		config.Filters.Add(new ShortnrApiErrorFilter());

		// Web API routes
		config.MapHttpAttributeRoutes();
	}

The filter will be added to the existing list of Web API filters. So now when a request goes wrong, an empty response with a corresponding HTTP error code will be returned.

Cool, we have an API now! You can use this API if you want to shorten your blog posts for example.

## Making it more robust

We have a nice and working URL shortener at the moment. There are a few things which could be better though. Off the top of my head:

*   When a user fills in a URL that doesn't exist, the request succeeds anyway. We have to make sure that when a user fills in a wrong URL (one that doesn't return HTTP status code 200), an exception should be thrown.
*   Because this URL shortener is built to be public, there should be a sort of check that people (or bots) don't misuse it. Every IP address can only shorten 10 URLs per hour.
*   Because people can create a custom URL, there should be a check that only alphanumeric characters, dashes and underscores are accepted.

### Checking the Url

Let's open UrlManager.cs. In this file, we need to add some URL checking. First, it is important to assert whether the URL is valid. If the URL is valid, we should make a web call to that URL to determine if the URL is real and can be found. If not, an exception should be thrown. Fill in the following piece of code in the ShortenUrl method in the UrlManager:

	if (!longUrl.StartsWith("http://") && !longUrl.StartsWith("https://"))
	{
		throw new ArgumentException("Invalid URL format");
	}
	Uri urlCheck = new Uri(longUrl);
	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlCheck);
	request.Timeout = 10000;
	try
	{
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();
	}
	catch (Exception)
	{
		throw new ShortnrNotFoundException();
	}

This block checks if the URL has a valid format and next tries to reach the URL. If the response wasn't successful, an exception will be thrown.

### Pushing the limit

We should build a check which determines if you are still able to create more short URLs. If you've reached your cap, an exception should be thrown from within the UrlManager. A nice place to configure the maximum number of URls per hour is the Web.config file. There is a section called AppSettings which can contain custom variables which can be used throughout the whole application. In the <appSettings> section of the Web.config file, add the following piece of code:

	<add key="MaxNumberShortUrlsPerHour" value="10" />

Next, make sure that the Shortnr.Web.Business project contains a reference to System.Configuration. Add the following piece of code to the ShortenUrl method in the UrlManager:

	int cap = 0;
	string capString = ConfigurationManager.AppSettings["MaxNumberShortUrlsPerHour"];
	int.TryParse(capString, out cap);
	DateTime dateToCheck = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
	int count = ctx.ShortUrls.Where(u => u.Ip == ip && u.Added >= dateToCheck).Count();
	if (cap != 0 && count > cap)
	{
		throw new ArgumentException("Your hourly limit has exceeded");
	}

This piece of code retrieves the maximum number of shortcodes per hour from the Web.config. Next, the count of short URLs created with your IP address in the last hour will be retrieved. If this number is higher than the cap you've configured, an exception will be thrown.

A new type of exception (ArgumentException) is now thrown. We have to update both the MVC and Web API filters to catch this exception and return a corresponding response.

Add the following piece of code to ShortnrErrorFilter.cs:

	if (ex is ArgumentException)
	{
		code = HttpStatusCode.BadRequest;
		viewName = "Error400";
	}

The following content should be added to a new file called Error400.cshtml in the folder Views\Shared:

	@{
		ViewBag.Title = "Error";
	}

	<h2>Error</h2>

	<p>An unexpected error occured.</p>

Add the following piece of code to ShortnrApiErrorFilter.cs:

	else if (ex is ArgumentException)
	{
		code = HttpStatusCode.BadRequest;
	}

### Checking the short URL

We've given the user the possibility of creating a custom URL. At the moment, it's possible to fill in everything as short URL. This shouldn't be possible. The custom URL may only contain alphanumeric characters, dashes and underscores. Also, you don't want the custom short URL to be too long, so let's check that too. Place the following check in UrlManager.cs:

	if (segment.Length > 20 || !Regex.IsMatch(segment, @"^[A-Za-z\d_-]+$"))
	{
		throw new ArgumentException("Malformed or too long segment");
	}

This check uses a regular expression to check whether the custom URL contains only letters, numbers, underscores and dashes. It also checks if the URL isn't too long.

## Done (?)

Whether you've completed the whole tutorial, or just skimmed through it, thanks for making it this far. I hope you liked it and really helped you become better at using the ASP.NET stack for building better and robuster web applications. A nice new addition to the URL shortener would be a possibility to see the statistics of a specific URL, since those stats are already saved to the database.

**[The code is available on GitHub](https://github.com/dukeofharen/shortnr.net)**