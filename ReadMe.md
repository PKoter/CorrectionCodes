# Projekt aplikacji testującej algorytmy detekcji i korekty błedów transmisji

lp. | projekty | platforma | stos
-|-|-|-
1.|CorrectionCodes | .NET Framework 4.7.1 | WPF
2.|CorrectionCodes.Core | .NET Standard 2.0 | dll
3.|Tests | .NET Core 2.0 / .NET Standard 2.0 | Testy jednostkowe dla 2.

### Uruchomienie
Przydatne komendy przy pracy bez środowiska Visual Studio (VS Code / inny edytor of choice):
 - dotnet build CorrectionCodes.Core/Tests - buduje projekty
 - dotnet restore CorrectionCodes.Core/Tests (opcjonalny - przy błędach braku pakietów)
 - dotnet test Tests - uruchamia testy w projekcie

Komendy wykonujemy w folderze z plikiem solucji (.sln)

### Wymagane: 
 - .NET Core SDK 2.0