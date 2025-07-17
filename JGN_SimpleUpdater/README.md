# JGN Simple Updater (WinGet)

Ein einfaches, modernes Windows-Tool, um alle installierten Programme mit WinGet im Hintergrund und ohne sichtbares Konsolenfenster zu aktualisieren. Die Anwendung zeigt den Fortschritt übersichtlich an und benötigt keine Installation.

## Features
- Automatische Programmsuche und -aktualisierung mit WinGet
- Fortschrittsbalken für den Gesamtfortschritt
- Statusanzeige für das aktuell installierte Paket
- Ausführliche Log-Ausgabe in einer skalierbaren TextBox
- Keine sichtbare Konsole, läuft komplett im Hintergrund
- Startet sich bei Bedarf automatisch mit Administratorrechten
- Einfache, portable Einzel-EXE (kein Setup nötig)

## Voraussetzungen
- Windows 10/11
- .NET 8 Desktop Runtime
- WinGet (Windows Package Manager) installiert und im PATH

## Installation
1. Release-EXE herunterladen oder selbst mit Visual Studio/`dotnet publish` bauen.
2. EXE ausführen (bei Bedarf als Administrator).

## Nutzung
- Auf "Update All" klicken, um alle verfügbaren Updates zu installieren.
- Der Fortschritt wird im Balken und im Log angezeigt.
- Über "Über" gibt es Programminfos.

## Lizenz
MIT License – siehe LICENSE-Datei.

---
**Autor:** Jan Geiger Networking 