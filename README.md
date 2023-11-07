# MPF Legacy

[![Build status](https://ci.appveyor.com/api/projects/status/gb3a78crf74yu1qc?svg=true)](https://ci.appveyor.com/project/Deterous/mpf-legacy/build/artifacts)

A fork of [Media Preservation Frontend (MPF)](https://github.com/SabreTools/MPF) to enable legacy Windows users to continue to contribute to [redump.org](http://redump.org/)

MPF Legacy will only be updated with bug fixes and new versions of DIC, Aaru, and Redumper. If you **need** any new features from [MPF 3](https://github.com/SabreTools/MPF) and for whatever reason **cannot** upgrade Windows, create a PR or an issue.

### System Requirements

I am currently working on porting and testing code to older Windows systems. Until then, the following are required to run MPF Legacy:
- Windows 7 SP1 (both 32bit and 64bit supported)
- [.NET Framework 4.8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
- You will need to disable strong name validation due to `Microsoft.Management.Infrastructure` being unsigned. Add the following registry keys:
```
	[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\StrongName\Verification\*,31bf3856ad364e35]
	[HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\StrongName\Verification\*,31bf3856ad364e35]
```

## Credits

All thanks go to the developers and testers of [MPF](https://github.com/SabreTools/MPF).
This repo is just a semi-frozen branch supporting users who choose to use legacy Windows systems or prefer the "Classic" MPF with .NET Framework 4.8 experience.
