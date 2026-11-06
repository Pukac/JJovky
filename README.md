# JJovky / JJ Poems

Personal poetry, dreams and journal app built with .NET MAUI for Android.

## Open and run

Open `JJPoems/JJPoems.slnx` in Visual Studio 2026 with the .NET MAUI Android
workload installed. Select an Android emulator or connected Android phone.

```powershell
dotnet restore JJPoems/JJPoems.csproj --configfile JJPoems/NuGet.config
dotnet build JJPoems/JJPoems.csproj -f net10.0-android
```

## Project layout

- `JJPoems/`: application, XAML views, MVVM view models, local storage and assets.
- `Tests/StorageChecks/`: executable checks for storage recovery and import.
- `TEST-VERSION.md`: test APK installation, verification and known limitations.
- `JJPoems/README.md`: original design and application documentation.

## Storage checks

```powershell
dotnet run --project Tests/StorageChecks/StorageChecks.csproj
```

## Build an installable APK

```powershell
dotnet publish JJPoems/JJPoems.csproj -c Release -f net10.0-android -p:AndroidPackageFormats=apk -p:EmbedAssembliesIntoApk=true
```

Signed output is generated in `JJPoems/bin/Release/net10.0-android/publish/`.
Build outputs, APK files, IDE state, package caches and signing keys are not
tracked by Git. Keep the same signing key when updating an existing install.

## Current test version

Version 0.1.0 provides local profiles, personal writings, themes, a monthly
calendar, draft recovery and JSON export/import. Profiles are stored on the
device; email is not online authentication. Cloud sync and weekly writing
goals are not implemented. Device UI verification is still pending.
