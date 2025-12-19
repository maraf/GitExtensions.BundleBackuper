# GitExtensions.BundleBackuper
GIT bundles is a great way to create backups of local branches. This extension for [GitExtensions](https://github.com/gitextensions/gitextensions) creates item in top menu containg all bundles at specified path. Clicking bundle item maps this bundle as remote. Beside this restore operation, it also contains button to create bundle/backup between current branch head and last commit pushed commit.

![Preview](/assets/screenshot.png)

## Requirements

- **Git Extensions 6.0.2+**: This plugin is compatible with Git Extensions 6.0.2 and later
- **.NET 9.0 Desktop Runtime**: Required by Git Extensions 6.x ([Download](https://dotnet.microsoft.com/download/dotnet/9.0))

## Installation

Install via the Git Extensions Plugin Manager:
1. Open Git Extensions
2. Go to Tools → Settings → Plugins
3. Click "Plugin Manager"
4. Search for "GitExtensions.BundleBackuper"
5. Click Install

## Compatibility

| Plugin Version | Git Extensions Version | .NET Runtime |
|---------------|------------------------|--------------|
| 6.0.0+        | 6.0.2+                | .NET 9.0     |
| 5.0.x         | 3.x                   | .NET 4.6.1   |

## Building from Source

```bash
# Prerequisites
- .NET 9.0 SDK or later

# Build
dotnet build src/GitExtensions.BundleBackuper/GitExtensions.BundleBackuper.csproj

# Package
dotnet pack src/GitExtensions.BundleBackuper/GitExtensions.BundleBackuper.csproj
```

### Icons

Some icons by Yusuke [Kamiyamane](http://p.yusukekamiyamane.com).