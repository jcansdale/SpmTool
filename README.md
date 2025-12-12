# SpmTool

A .NET utility for converting Spektrum RC transmitter model files (SPM) between DX8 and DX9 formats.

## Overview

SpmTool enables conversion of Spektrum radio control transmitter model configuration files between older DX8 and newer DX9 formats. This is useful for migrating model setups when upgrading transmitters or sharing configurations between different Spektrum radio systems.

## Features

- Convert DX8 SPM files to DX9 format
- Convert DX9 SPM files to DX8 format
- Support for Airplane and Helicopter model types
- Batch conversion of entire directories
- XML intermediate format for debugging and manual editing
- Web application interface for online conversions

## Projects

| Project | Description |
|---------|-------------|
| **SpmTool** | Command-line application for file conversion |
| **SpmTool.Tests** | Unit tests for the conversion logic |
| **SpmTool.WebApplication.4.0** | ASP.NET web interface for online conversion |

## Usage

### Command Line

Convert a single file:
```bash
SpmTool.exe path/to/model.spm
```

Convert all SPM files in a directory:
```bash
SpmTool.exe path/to/directory/
```

Converted files are output to a `DX9` subdirectory.

### Programmatic API

```csharp
using SpmTool;

// Convert DX8 to DX9
string dx8Spm = File.ReadAllText("model.spm");
string dx9Spm = SpmConvert.DX8To(dx8Spm);

// Convert DX9 to DX8
string dx9Spm = File.ReadAllText("model.spm");
string dx8Spm = SpmConvert.DX9To(dx9Spm);

// Convert SPM to XML for inspection
string xml = SpmToXml.Convert(spmContent);

// Convert XML back to SPM
string spm = XmlToSpm.Convert(xmlContent);
```

## Building

### Requirements

- .NET 9.0 SDK (for cross-platform builds)
- .NET Framework 4.8 (Windows only, for web application)

### Build

```bash
# Build main project and tests (cross-platform)
dotnet build SpmTool/SpmTool.csproj
dotnet build SpmTool.Tests/SpmTool.Tests.csproj

# Build web application (Windows with MSBuild only)
msbuild SpmTool.WebApplication.4.0/SpmTool.WebApplication.4.0.csproj
```

## Testing

Run the unit tests:

```bash
dotnet test SpmTool.Tests/SpmTool.Tests.csproj
```

## Architecture

The conversion process uses XSLT transformations:

1. **SPM → XML**: Parse the binary/text SPM format into structured XML
2. **XML Transform**: Apply XSLT stylesheet (`DX8toDX9.xsl` or `DX9toDX8.xslt`)
3. **XML → SPM**: Convert the transformed XML back to SPM format

### Key Files

- `SpmToXml.cs` - Converts SPM format to XML
- `XmlToSpm.cs` - Converts XML back to SPM format
- `SpmConvert.cs` - High-level conversion API
- `SpmUtilities.cs` - Helper functions for model detection and parsing
- `DX8toDX9.xsl` - XSLT for DX8 to DX9 conversion
- `DX9toDX8.xslt` - XSLT for DX9 to DX8 conversion

## License

See repository for license information.
