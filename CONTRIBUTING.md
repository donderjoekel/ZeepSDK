# Contributing 
Contributions are welcome!

The main goal of ZeepSDK is to provide a translation layer in between mods and Zeepkist that allows for easy and intuitive access to functionality from the game. By building a layer in between we can easily fix any issues that arise without having to update a multitude of individual mods

With that said, there are a couple of guidelines to keep in mind that make for a good contribution which will be listed down below.

## Feature
You have an idea for a new feature, that's great!

Is this new feature that you thought of something that is beneficial to one mod, or multiple? If it is specific to one mod then it might be better to implement it in said mod instead of here. Feel free to discuss this as something that might initially seem for just one mod might be very useful for others as well!

Okay good, so we've established that this is something to go into ZeepSDK, from here on out there are a few things to consider:

Make sure to fork the ZeepSDK repository and create a new branch that starts with `feature/` and replace spaces with underscores. Something like `feature/my_awesome_new_feature`.

1. Outward facing functionality should be contained within `Api` classes (think `ChatApi`, `CosmeticsApi`).
    - If there is already a class like that, feel free to put your new functionality in there.
    - If not you're free to make a new one! Keep in mind that it should be a `public static` class that ends with `Api`.
2. Inward facing functionality should be hidden with the `internal` or `private` keyword.
    - Think of utility classes or methods that should not be touched by a mod directly.  
3. Don't return concrete implementations, return interfaces instead.
    - Returning an `interface` allows us to easily maintain and fix functionality without having to update depending mods.
4. Don't forget to describe your new outward facing code in the `/// <summary>` block!
5. Test. test. test. Test your code thoroughly!
6. Catch as many exceptions as you can, and appropriately. If something goes wrong in the ZeepSDK it is essential that the depending mods will be able to continue functioning.
    - There are cases where this won't work e.g. where Zeepkist has removed something that your functionality relies on, it's unfortunate but impossible to avoid.

 Once you've wrapped everything up, tested it, and it seems all good you're almost ready to open a Pull Request!

 One last thing to do is bumping the version number. For new features we most of the time would bump the `minor` segment of the version according to the [Semantic Versioning](https://semver.org) standard. E.g. from `1.20.1` to `1.21.0`.

 When you open your pull request be sure to start the title with `feature:` and give a short one-line description. In the big text box you can describe what this does to your heart's content.

 ## Fix
 Oops, something is broken and you're here to fix it, nice!

 Most of the rules of the Feature section apply here as well, with only a few small changes:

 1. The branch should not start with `feature/` but with `fix/`.
 2. For versioning we bump the `patch` segment of the version. E.g. from `1.25.0` to `1.25.1`.
 3. When opening the pull request start the title with `fix:`.
