---
title: C# Find Free TCP Port
date: 2018-03-18T21:29:33+02:00
description: Use this handy little code snippet to find a free TCP port on your machine. 
---

Hi,

Use this handy little code snippet to find a free TCP port on your machine. I always use this so I can perform multiple in-memory WebApi unit tests at the same time.

```
public static class TcpUtilities
{
   public static int GetFreeTcpPort()
   {
      var listener = new TcpListener(IPAddress.Loopback, 0);
      listener.Start();
      var port = ((IPEndPoint)listener.LocalEndpoint).Port;
      listener.Stop();
      return port;
   }
}
```