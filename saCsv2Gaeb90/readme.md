﻿# saCsv2Gaeb90

## About
### (DE) Konverter von csv-Dateiformat nach GAEB90
SaXps2Pdf-Converter ist ein einfaches Werkzeug zur Konvertierung von 
Leistungsverzeichnissen im csv-Format in eine GAEB90-Datei.  
Verwenden Sie zum Kompilieren Visual Studio 2017 oder höher.
### (EN) Converts csv-files into GAEB90-format
SaXps2Pdf-Converter is a simple tool for converting 
service lists in csv format into a GAEB90 file.  
Use Visual Studio 2017 or later to compile.  

Note:  
GAEB is an exchange format for specifications commonly used in the German construction industry. 
If you are not familiar with this format, it is not a big deal. No one outside of Germany 
would voluntarily think of messing around with it.  

Therefore, I will write the further content of this file only in German.

## Anwendung
### Bedienung
Im Ordner bin/release finden Sie eine beispielhafte csv-Datenquelle und die dazu passende Steuerdatei.
Nach dem Starten des Programm öffnen Sie die Steuerdatei mit dem Schalter "auswählen" und die Datenquelle 
mit dem Schalter "csv öffnen".  

In der Ansicht "Definition" können Sie die Steuerdatei bearbeiten und auch wieder abspeichern.  

In der Ansicht GAEB-Daten sehen Sie eine Vorschau der erzeugten GAEB-Ausgabe. Diese kann von hier aus gespeichert werden.

### Steuerdaten (Definition)
Die Beispieldatei Sample-2024-06-30_Description.ini enthält ausführliche Kommentare, die es Ihnen erleichtern
die Definitionsdaten an Ihre csv-Quelle anzupassen. Im Beispiel enthält die Quelle keine getrennten Kurz- oder
Langtexte, daher verweisen hier alle Textfelder auf die Spalte "Kurztext alt".
  

s.a. 2024-06-30
