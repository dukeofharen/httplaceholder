---
title: Configuration validation in .NET Core
date: 2018-02-21T20:12:59+02:00
description: Configuration validation using the .NET Core configuration and data annotations library.
---

Hi,

TLDR: Configuration validation using the .NET Core configuration and data annotations library. Not interested in reading the whole thing? The GitHub link to the repository is at the bottom of the page.

The customer I currently work at has a large and very intricate collection of .NET reusable libraries. One of these libraries contains code to validate and read custom configuration sections in Web.config and App.config files. With the dawn of .NET Core, the "classic" configuration model is replaced by a newer and more modular one. The new .NET Core configuration libraries enable you to use any sort of configuration file (e.g. JSON files, XML files, YAML files etc.) and also simplifies the deserialization of said file to an object.

We needed to develop a .NET Core application, so the old "full .NET" libraries were unusable. The use of the new .NET Core configuration library is a no-brainer, but what about the validation part? Well, .NET already has a complete assembly available for model validation: System.ComponentModel.DataAnnotations. This class contains several validators and attributes that you can use to validate models. This assembly is already used by both the "old" ASP.NET MVC and ASP.NET MVC Core for the validation of posted content. Let's take a look at the following example:

```
public class ConfigurationModel
{
    [Required]
    public string ApplicationName { get; set; }

    [Required]
    [Url]
    public string RootUrl { get; set; }

    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; }
}
```

Like you can see in the code above, all fields are required, RootUrl should be a valid URL and AdminEmail should be a valid email address. Easy, right? Now, we have to find a way to load the `appsettings.json` file and to validate the application. There are a few steps we need to do:

1. Add a ModelValidator class. This class validates an instance of a class where the properties are decorated with a validation attribute (like in the example above) and returns any validation errors.
1. Add a ValidatedSettings class. This class can be injected in every other class where you need the settings object. This class holds an instance of the current settings and validates this class if it hasn't already been done.
1. Add the possibility to recursively validate objects. Right now, it is only possible to validate the root settings class, while you might also want to validate any subclasses of the settings class that also have validation attributes of their own.

## ModelValidator

Like I explained above, we need to add a ModelValidator class to the application. The following class (and corresponding) interface need to be added to the application.

```
public interface IModelValidator
{
    IEnumerable<ValidationResult> ValidateModel(object model);
}
```

```
internal class ModelValidator : IModelValidator
{
    public IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model, null, null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
}
```

The ModelValidator class takes any object and passes it to a `ValidationContext` object. Eventually, `Validator.TryValidateObject` will be called, with an instance of the model, the validation context and a results list. The results list will be filled with `ValidatonResult` objects (the list will stay empty if no validation errors occurred).

## ValidatedSettings

We need to add a class that always has the latest and validated settings class inside. For this reason, I made the `ValidatedSettings` class (with corresponding interface).

```
public interface IValidatedSettings<TSettings> where TSettings : class
{
    TSettings GetValidatedSettings(bool forceValidate = false);
}
```

```
internal class ValidatedSettings<TSettings> : IValidatedSettings<TSettings> where TSettings : class, new()
{
    private static TSettings _settings;
    private readonly IOptionsMonitor<TSettings> _options;
    private readonly IModelValidator _modelValidator;

    public ValidatedSettings(
        IOptionsMonitor<TSettings> options,
        IModelValidator modelValidator)
    {
        _options = options;
        _modelValidator = modelValidator;
    }

    public TSettings GetValidatedSettings(bool forceValidate = false)
    {
        if (_settings == null || forceValidate)
        {
        var settings = _options.CurrentValue;
        var validationResults = _modelValidator.ValidateModel(settings).ToArray();
        if (validationResults.Any())
        {
            var builder = new StringBuilder();
            builder.AppendLine("The configuration file contains the following errors:");
            foreach (var result in validationResults)
            {
                builder.AppendLine(result.ErrorMessage);
                if (result is CompositeValidationResult compositeResult)
                {
                    foreach (var compositeError in compositeResult.Results)
                    {
                    builder.AppendLine($"- {compositeError.ErrorMessage}");
                    }
                }
            }

            throw new Exception(builder.ToString());
        }

        _settings = settings;
        }

        return _settings;
    }
}
```

This class needs an instance of IOptionsMonitor (a part of the .NET Core configuration framework that checks if a settings file is changed) and an instance of the IModelValidator. The settings model will be validated. If there are validation errors, an Exception containing all the validation errors will be thrown. If the validation passes, the settings instance will be saved in memory and returned.

## ValidateObject attribute

While looking for a way to recursively validate an object, I found this article: <http://www.technofattie.com/2011/10/05/recursive-validation-using-dataannotations.html>. In this article, a custom validation attribute called `ValidateObjectAttribute` is used to explicitely tell the ModelValidator that it also has to validate this property. I modified the code a bit to also allow IEnumerables to be validated.

```
public class ValidateObjectAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (value is IEnumerable enumerable)
        {
        foreach (var subObject in enumerable)
        {
            var context = new ValidationContext(subObject, null, null);
            Validator.TryValidateObject(subObject, context, results, true);
        }
        }
        else
        {
        var context = new ValidationContext(value, null, null);
        Validator.TryValidateObject(value, context, results, true);
        }

        if (results.Count != 0)
        {
        var compositeResults = new CompositeValidationResult(string.Format("Validation for {0} failed!", validationContext.DisplayName));
        results.ForEach(compositeResults.AddResult);

        return compositeResults;
        }

        return ValidationResult.Success;
    }
}

public class CompositeValidationResult : ValidationResult
{
    private readonly List<ValidationResult> _results = new List<ValidationResult>();

    public IEnumerable<ValidationResult> Results => _results;

    public CompositeValidationResult(string errorMessage) : base(errorMessage) { }
    public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }
    protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

    public void AddResult(ValidationResult validationResult)
    {
        _results.Add(validationResult);
    }
}
```

The code above first checks if an object is an IEnumerable. If this is the case, it will loop through the IEnumerable and validate all objects within. If the object is not an IEnumerable, it will be validated as is. All validation errors are added to a CompositeValidationResult, which can contain multiple validation errors.

## Tying it together

Ok, so now we want this to work together. First, make a class structure that corresponds to the `appsettings.json` file.

```
public class Settings
{
    [Required]
    public string WebsiteName { get; set; }

    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; }

    [Required]
    [Url]
    public string RootUrl { get; set; }

    [ValidateObject]
    public DatabaseSettings DatabaseSettings { get; set; }

    [ValidateObject]
    public HyperLink[] Links { get; set; }
}
```

```
public class HyperLink
{
    [Required]
    public string Text { get; set; }

    [Url]
    [Required]
    public string Link { get; set; }
}
```

```
public class DatabaseSettings
{
    [Required]
    public string ConnectionString { get; set; }
}
```

This is the `appsettings.json` file.

```
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "WebsiteName": "ConfigTest",
  "AdminEmail": "duco@winterwerp.nl",
  "RootUrl": "https://config.test",
  "DatabaseSettings": {
    "ConnectionString": "Db=database.db"
  },
  "Links": [
    {
      "Text": "Google",
      "Link": "https://www.google.com"
    },
    {
      "Text": "Facebook",
      "Link": "https://www.facebook.nl"
    }
  ]
}
```

Next, the `Startup.cs` file needs to be updated. The following lines need to be added to the `ConfigureServices` to register the `appsettings.json` file and to register the classes for dependency injection.

```
// Register the configuration here.
var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", false, true);
Configuration = builder.Build();

services.Configure<Settings>(Configuration);

// Register the classes needed for the configuration validation
services.AddTransient<IModelValidator, ModelValidator>();
services.AddTransient<IValidatedSettings<Settings>, ValidatedSettings<Settings>>();
```

Also in the `Startup.cs` file, in the `Configure` method, these lines need to be added.

```
var monitor = app.ApplicationServices.GetService<IOptionsMonitor<Settings>>();
var validatedSettings = app.ApplicationServices.GetService<IValidatedSettings<Settings>>();
monitor.OnChange(settings =>
{
    // Make sure that every time when the settings file changes, it will be validated.
    validatedSettings.GetValidatedSettings(true);
});
```

The `OnChange` method on the `IOptionsMonitor` class makes sure that every time the `appsettings.json` file is updated, the settings class is also forcibly validated.

Lastly, you can inject the `IValidatedSettings<Settings>` where you need the settings. In this example, the settings are read and added to the ViewData class to be displayed in the Razor view.

```
public class HomeController : Controller
{
    private readonly IValidatedSettings<Settings> _validatedSettings;

    public HomeController(IValidatedSettings<Settings> validatedSettings)
    {
        _validatedSettings = validatedSettings;
    }

    public IActionResult Index()
    {
        var settings = _validatedSettings.GetValidatedSettings(true);
        ViewData["Settings"] = JsonConvert.SerializeObject(settings);
        return View();
    }
}
```

## Finally

There's nothing more to it than this. This is a nice example of how two existing components of the .NET library can work together to make something completely new. Also, I want to thank [Josh Carroll](http://www.technofattie.com) for his article on recursive validation.

Oh yeah, not focussed on in this article, but it is really easy to build a few unit tests for your configuration to check a few scenarios.

I've uploaded the complete source code to my GitHub account on <https://github.com/dukeofharen/ducodes/tree/master/ConfigValidationExample>.