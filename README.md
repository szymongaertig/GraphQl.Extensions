[![Build status](https://ci.appveyor.com/api/projects/status/1nhsll02v4t1m9vc?svg=true)](https://ci.appveyor.com/project/garfieldos/graphql-extensions)

# GraphQl.Extensions
Extensions for GraphQl library

## Get it on NuGet:
```
PM> Install-Package GraphQl.Extensions
```

## Available extensions:
* TryExportToDataTable - allows converting result of ExecutionResult class to DataTable.

## Available OutputFormatters:
* GraphQlXlsxFormatter - generates xlsx output from generated GraphQlResult (only for flat data structure)
* GraphQlCsvFormatter - generates csv output from generated GraphQlResult (only for flat data structure)

## Usage
To enable output formatters, you need to add to the ConfigureServices method from startup.cs:
```
                services.AddMvc(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.OutputFormatters.Add(new GraphQlCsvFormatter("products", ";", Encoding.UTF8));
                    options.OutputFormatters.Add(new GraphQlXlsxFormatter("products"));

                })
```

## Samples are available here: 
https://github.com/garfieldos/GraphQl.Extensions/tree/master/src/samples/GraphQl.Extensions.Samples.AspNetCore
