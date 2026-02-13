# Auto HDR Force

A Windows GUI app that lets you force enable Auto HDR on a per-game basis by writing Direct3D behavior flags to the registry.

![.NET Framework 4](https://img.shields.io/badge/.NET_Framework-4.0-blue) ![License](https://img.shields.io/badge/License-GPLv3-green)

## Features

- **Visual game selection** — Browse and pick game executables with a file dialog
- **Game list** — See all configured games and their current HDR status at a glance
- **Enable / Disable Auto HDR** — One-click toggle per game
- **10-bit color support** — Optional 10-bit color output toggle
- **Remove entries** — Clean up games you no longer want configured
- **No admin required** — All settings are stored under `HKEY_CURRENT_USER`

## Download

Grab the latest **AutoHDRForce.exe** from the [Releases](../../releases) page. No installation needed — just run it.

## How It Works

Windows Auto HDR can be forced on a per-application basis via registry keys under:

```
HKEY_CURRENT_USER\SOFTWARE\Microsoft\Direct3D\
```

Each game gets an `ApplicationN` subkey with a `Name` value (the exe path) and a `D3DBehaviors` value that controls HDR behavior:

| Setting | D3DBehaviors Value |
|---|---|
| Auto HDR only | `BufferUpgradeOverride=1` |
| Auto HDR + 10-bit | `BufferUpgradeOverride=1;BufferUpgradeEnable10Bit=1` |

This app provides a GUI to manage those registry entries instead of editing them manually or using the command line.

## Building From Source

The app is a single C# file that compiles with the .NET Framework 4 compiler built into Windows — no SDK install needed:

```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:AutoHDRForce.exe AutoHDRForce.cs
```

## Requirements

- Windows 10 or 11
- .NET Framework 4.0 (pre-installed on all modern Windows)
- HDR-capable display

## Credits

This is a GUI rewrite of [ledoge/autohdr_force](https://github.com/ledoge/autohdr_force), which is a command-line C utility for the same purpose. The original project by **ledoge** figured out the registry keys and Direct3D behavior flags that make per-app Auto HDR forcing possible.

## License

[GNU General Public License v3.0](LICENSE)
