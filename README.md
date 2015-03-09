# Jpeg Resizing With Tasks

In this project I used Task to resize a lot of pictures.

Without Task it takes about 16 seconds for 30 Jpeg files. When Task are enabled it takes just 10 seconds. The gain is more than 35 percent for a quad core CPU (Intel Core i3-4350).

Enable Task in Program.cs :

```
/// <summary>
/// Set to True to see the power in action !
/// </summary>
private static bool _useMultiThreading = false;
```

