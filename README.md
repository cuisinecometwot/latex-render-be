# asp.net-core-project

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download) (version 8.0)
- [TeX Live](https://tug.org/texlive/doc/texlive-en/texlive-en.html#installation)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/cuisinecometwot/latex-render-be.git
   cd latex-render-be
   ```
2. Restore the dependencies:
   ```bash
   dotnet restore
   ```
3. Update the settings (optional):
   ```bash
   nano appsettings.json
   ```
4. Update the database:
   ```bash
   dotnet tool install –global dotnet-e
   dotnet ef database update
   ```
5. Build and run the application:
   ```bash
   dotnet build
   dotnet run
   ```
