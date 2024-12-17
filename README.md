# SoftwareUsageTime [![NuGet](https://img.shields.io/nuget/v/PicaPico.SoftwareUsageTime.svg)](https://nuget.org/packages/PicaPico.SoftwareUsageTime) [![Build AutoUpdate](https://github.com/HeHang0/SoftwareUsageTime/actions/workflows/library.nuget.yml/badge.svg)](https://github.com/HeHang0/SoftwareUsageTime/actions/workflows/library.nuget.yml)

SoftwareUsageTime is a software that counts the usage time of software.

## Usage

-------

SoftwareUsageTime is available as [NuGet package](https://www.nuget.org/packages/PicaPico.SoftwareUsageTime).

```csharp
using PicaPico;

var usage = new SoftwareUsageTime();
usage.Start();
Task.Delay(TimeSpan.FromSeconds(20)).Wait();
usage.Stop();
var r = usage.GetSoftwareUsage();
foreach (var item in r)
{
    Console.WriteLine($"{item.Key} ===== {item.Value}");
}
Console.ReadLine();
```

## Repository

-------

The source code for SoftwareUsageTime is hosted on GitHub. You can find it at the following URL: [https://github.com/HeHang0/SoftwareUsageTime](https://github.com/HeHang0/SoftwareUsageTime)

## License

-------

SoftwareUsageTime is released under the MIT license. This means you are free to use and modify it, as long as you comply with the terms of the license.
