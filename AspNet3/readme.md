# AspNet3 Readme

## Changing default SSL port number

IISExpress uses http.sys for its communication and it requires SSL ports to be registered as Administrator.
To avoid running Visual Studio as administrator, IIS Express reserves the port range 44300 - 44399 when it is installed.
As long as you select a port within this range (Properties->launchSettings.json->iisSettings->sslPort) you do not need to 
run IISExpressAdminCmd to register the URL.

# Razor syntax reference for ASP.NET Core

https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-3.1

In Razor Views, implicit expressions cannot contain C# generics, as the characters 
inside the brackets (<>) are interpreted as an HTML tag. 
    
The following code is not valid:

```html  
<p>@GenericMethod<int>()</p>
```
Generic method calls must be wrapped in an explicit Razor expression or a Razor code block.
    
Razor expression:
-----------------
Explicit Razor expressions consist of an @ symbol with balanced parenthesis: @()

```html
<p>@(GenericMethod<int>())</p>
```

Razor code blocks:
------------------
Razor code blocks start with @ and are enclosed by {}. Unlike expressions, C# code inside code blocks isn't rendered.
Code blocks and expressions in a view share the same scope and are defined in the order they are written.

```csharp
@{
    var str = "This is a string.";
}
```

In code blocks, we can declare local functions with markup to serve as templating methods:

```csharp
@{
    void RenderName(string name)
    {
        <p>Name: <strong>@name</strong></p>
    }

    RenderName("Emre");        
}
```
    
Implicit transitions:
---------------------

The default language in a code block is C#, but the Razor Page can transition back to HTML:

```csharp
@{
    var inCSharp = true;
    <p>Now in HTML, was in C# @inCSharp</p>
}
```

Explicit delimited transition:
------------------------------

To define a subsection of a code block that should render HTML, surround the characters for 
rendering with the Razor <text> tag:

```csharp
@for (var i = 0; i < people.Length; i++)
{
    var person = people[i];
    <text>Name: @person.Name</text>
}
```

Use this approach to render HTML that isn't surrounded by an HTML tag. Without an HTML or 
Razor tag, a Razor runtime error occurs.

The ```<text>``` tag is useful to control whitespace when rendering content:

* Only the content between the <text> tag is rendered.
* No whitespace before or after the <text> tag appears in the HTML output.

The ```<text>``` tag is an element that is treated specially by Razor. It causes Razor to interpret the inner contents of the ```<text>``` block as content, and to not render the containing ```<text>``` tag element (meaning only the inner contents of the ```<text>``` element will be rendered – the tag itself will not).  This makes it convenient when you want to render multi-line content blocks that are not wrapped by an HTML element. 

Explicit line transition:
-------------------------

Not all content container blocks start with a tag element tag, though, and there are scenarios where the Razor parser can’t implicitly detect a content block.

Razor addresses this by enabling you to explicitly indicate the beginning of a line of content by using the ```@:``` character sequence within a code block.  The @: sequence indicates that the line of content that follows should be treated as a content block.

To render the rest of an entire line as HTML inside a code block, use ```@:``` syntax:

```csharp
@for (var i = 0; i < people.Length; i++)
{
    var person = people[i];
    @:Name: @person.Name
}
```

Without the @: in the code, a Razor runtime error is generated.

In addition to outputting static content, you can also have code nuggets embedded within a content block that is initiated using a ```@:``` character sequence. 

```csharp
@: Time is @DateTime.Now
```