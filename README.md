# JGN Simple Updater (WinGet) v1.0.0.3

Ein einfaches, modernes Windows-Tool, um alle installierten Programme mit WinGet im Hintergrund und ohne sichtbares Konsolenfenster zu aktualisieren. Die Anwendung zeigt den Fortschritt übersichtlich an und benötigt keine Installation.

## ✨ Features

### 🔄 Update-Management
- **Automatische Programmsuche und -aktualisierung** mit WinGet
- **Fortschrittsbalken** für den Gesamtfortschritt
- **Detaillierte Statusanzeige** für das aktuell installierte Paket
- **Abschlussfenster** mit Übersicht der erfolgreich aktualisierten und fehlgeschlagenen Apps
- **Update-Tracking** für erfolgreiche und fehlgeschlagene Updates

### 🎨 Benutzerfreundlichkeit
- **Ausführliche Log-Ausgabe** in einer skalierbaren TextBox
- **Echtzeit-Status-Updates** während Download, Verifizierung und Installation
- **JGN-Design** mit charakteristischem hellgrünen Update-Button
- **Keine sichtbare Konsole**, läuft komplett im Hintergrund

### 🔧 Technische Features
- **Automatischer Versionscheck** beim Starten (GitHub Integration)
- **Sichere Prozess-Beendigung** beim Schließen des Fensters
- **Startet sich bei Bedarf automatisch** mit Administratorrechten
- **Einfache, portable Einzel-EXE** (kein Setup nötig)

## 📋 Voraussetzungen
- Windows 10/11
- .NET 8 Desktop Runtime
- WinGet (Windows Package Manager) installiert und im PATH
- Internetverbindung für Versionscheck

## 🚀 Installation
1. Release-EXE von GitHub herunterladen oder selbst mit Visual Studio/`dotnet publish` bauen
2. EXE ausführen (bei Bedarf als Administrator)

## 💻 Nutzung
1. **Updates starten**: Auf "Update All" klicken, um alle verfügbaren Updates zu installieren
2. **Fortschritt verfolgen**: Der Fortschritt wird im Balken und im Log angezeigt
3. **Status überwachen**: Detaillierte Status-Informationen in der Statusanzeige
4. **Ergebnis prüfen**: Nach Abschluss erscheint ein Fenster mit der Update-Übersicht
5. **Programminfos**: Über "Über" gibt es Programminfos

## 🔄 Automatische Updates
- **Versionscheck**: Beim Starten wird automatisch geprüft, ob eine neue Version verfügbar ist
- **Update-Benachrichtigung**: Bei einer neuen Version erscheint ein Dialog mit Download-Link
- **GitHub Integration**: Direkte Verbindung zum GitHub Repository für Updates

## 🛡️ Sicherheit
- **Sichere Beendigung**: Der winget-Prozess wird ordnungsgemäß beendet beim Schließen
- **Fehlerbehandlung**: Robuste Fehlerbehandlung für alle Update-Phasen
- **Keine Hintergrundprozesse**: Keine Zombie-Prozesse nach Programmende

## 📄 Lizenz
MIT License – siehe LICENSE-Datei.

---

**Autor:** Jan Geiger Networking  
