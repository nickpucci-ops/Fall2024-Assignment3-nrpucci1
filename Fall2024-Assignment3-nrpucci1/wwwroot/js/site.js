// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// Set the fully constructed connection string
builder.Services.AddDbContext < ApplicationDbContext > (options =>
    options.UseSqlServer(connectionStringBuilder.ConnectionString));
```
