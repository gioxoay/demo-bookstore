# Book Store

## Features:
- .NET Core 6
- ASP.NET Web APIs
- Swagger UI
- Admin UI
- Security: Not done yet
- MongoDB database


## Import books mapping
https://github.com/Microsoft/openscraping-lib-csharp

```
{
  "books": 
  {
    "_xpath": "//books/book",
    "name": ".//name",
    "isbn": ".//isbn",
    "description": ".//description",
    "author": ".//author",
    "price":
    {
      "_xpath": ".//price",
      "_transformations": [
        "TrimTransformation",
        "CastToFloatTransformation"
      ]
    },
    "quantity":
    {
      "_xpath": ".//quantity",
      "_transformations": [
        "TrimTransformation",
        "CastToIntegerTransformation"
      ]
    },
  }
}
```

## Deployment
- Ubuntu OS