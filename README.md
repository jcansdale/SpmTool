# SpmTool

A .NET utility for converting Spektrum RC transmitter model files (SPM).

## Overview

SpmTool contains the core conversion logic plus two front-ends:

- `SpmTool`: a command-line application
- `SpmTool.WebApplication.4.0`: a legacy ASP.NET Web Forms application

The conversion engine supports DX8/DX9 conversion in the core API, while the web front-end also contains DX18 and DX7S-specific workflow logic.

Status: The hosted ASP.NET Web Forms application is currently the easiest way for most users to use SpmTool. The shared conversion engine is also available through the command-line tool, which is better suited to technically savvy users, testing, and automation.

## Features

- Convert Spektrum SPM files using a shared C# conversion engine
- Convert DX8-format models to DX9 from the CLI
- Convert DX8, DX9, DX18, and DX7S-related workflows in the web application
- Support Airplane and Helicopter model types
- Batch conversion of entire directories from the CLI
- XSLT-based conversion engine shared by the CLI and the legacy web front-end

## Projects

| Project | Description |
|---------|-------------|
| **SpmTool** | Command-line application and conversion engine |
| **SpmTool.Tests** | Unit and integration tests for the conversion logic and CLI wiring |
| **SpmTool.WebApplication.4.0** | ASP.NET Web Forms interface for online conversion |

## CLI Usage

The CLI accepts either a single `.spm` file path or a directory path.

Run from source:

```bash
dotnet run --project SpmTool/SpmTool.csproj --framework net9.0 -- path/to/model.spm
```

Or run a published executable:

```bash
SpmTool.exe path/to/model.spm
```

Convert all SPM files in a directory:

```bash
SpmTool.exe path/to/directory/
```

Current CLI behavior:

- writes output to a `DX9` subdirectory next to the input file or directory
- converts DX8-style input files to DX9 output
- skips files that are not Airplane or Helicopter models
- preserves the model slot number in the model name when it can be derived from the filename

## Programmatic API

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

- .NET 9.0 SDK for cross-platform builds and tests
- .NET Framework 4.8 targeting pack for `net48`
- MSBuild on Windows for the Web Forms application

### Build

```bash
# Build the CLI and tests
dotnet build SpmTool/SpmTool.csproj -f net9.0
dotnet build SpmTool.Tests/SpmTool.Tests.csproj -f net9.0

# Build the web application (Windows with MSBuild only)
msbuild SpmTool.WebApplication.4.0/SpmTool.WebApplication.4.0.csproj
```

## Testing

Run the cross-platform test suite:

```bash
dotnet test SpmTool.Tests/SpmTool.Tests.csproj --framework net9.0
```

This includes both conversion tests and CLI wiring integration tests.

## Web Application

The Web Forms application supports:

- single `.spm` uploads
- `.zip` uploads containing multiple `.spm` files
- target radio selection for `DX9`, `DX18`, and `DX8`
- optional removal of the slot index from the model name

This front-end is Windows-hosted and relies on legacy ASP.NET Web Forms infrastructure.

## Architecture

The conversion process uses XSLT transformations:

1. **SPM -> XML**: Parse the SPM text format into structured XML
2. **XML Transform**: Apply an XSLT stylesheet such as `DX8toDX9.xsl` or `DX9toDX8.xslt`
3. **XML -> SPM**: Convert the transformed XML back to SPM text

### Key Files

- `SpmToXml.cs` - Converts SPM format to XML
- `XmlToSpm.cs` - Converts XML back to SPM format
- `SpmConvert.cs` - High-level conversion API
- `SpmUtilities.cs` - Helper functions for model detection and parsing
- `Application.cs` - CLI entry point
- `DX8toDX9.xsl` - XSLT for DX8 to DX9 conversion
- `DX9toDX8.xslt` - XSLT for DX9 to DX8 conversion

## Current Limitations

- The CLI currently implements a DX8-to-DX9 workflow only.
- The web application contains additional radio-specific workflow logic that is not exposed through the CLI.
- The web application is a legacy ASP.NET Web Forms project and is not cross-platform.

## License

MIT. See `LICENSE`.
