# Just Drive

Een WPF desktopapplicatie voor het beheren van autoverhuur, gebouwd met C# en MySQL.

---

## Over de applicatie

Just Drive is een autoverhuurplatform dat klanten verbindt met verhuurbedrijven. Klanten kunnen auto's bekijken, vergelijken en reserveren, terwijl bedrijven hun vloot en reservaties beheren. Admins bewaken het volledige platform.

---

## Functionaliteiten

### Klant
- Registreren en inloggen met beveiligde wachtwoordhashing (SHA256)
- Wachtwoord vergeten met e-mailverificatiecode
- Auto's bekijken met filters (SUV, Sport, Elektrisch, Minivan)
- Auto's zoeken op merk of type
- Tot 3 auto's naast elkaar vergelijken
- Een auto reserveren met datumkiezer (gereserveerde datums geblokkeerd)
- Auto's toevoegen aan favorieten
- Actieve, toekomstige en vorige reservaties bekijken
- Toekomstige reservaties annuleren
- Bevestigings- en annuleringsmails ontvangen
- Schaderapport indienen met schadeniveau
- Profiel bewerken (naam, e-mail, adres, postcode, stad, wachtwoord)

### Bedrijf
- Dashboard met statistieken (totaal auto's, actieve reservaties, schaderaporten, omzet)
- Vloot beheren (toevoegen, bewerken, verwijderen van auto's)
- Autofoto's uploaden
- Alle reservaties bekijken met zoek- en filterfunctie
- Klanten en hun reservatiegeschiedenis bekijken
- Schaderaporten bekijken
- Reservatiedata exporteren naar CSV

### Admin
- Alle gebruikers beheren
- Alle auto's van alle bedrijven beheren
- Alle reservaties bekijken
- Alle schaderaporten bekijken

---

### Repository

1. Repository 
```bash
git clone https://github.com/IliasBa/Project_JustDrive.git
cd Project_JustDrive
```
---

### Standaard testaccounts

Gegenereede accounts gebruiken het wachtwoord: "Test1234"
--> zowel company als customers

admin: "no-reply@projectinspirationlab.com"
wachtwoord : "admin"

---
### Database

databasenaam: "dbjustdrive"

---

## Databaseschema

| Tabel | Beschrijving |
|-------|-------------|
| `user` | Alle gebruikers (klanten, bedrijven, admins) |
| `customer` | Klantspecifieke gegevens |
| `company` | Bedrijfsspecifieke gegevens |
| `car` | Auto-aanbod |
| `carname` | Automerken en modellen (genormaliseerd) |
| `reservation` | Autoreservaties |
| `damagereport` | Schaderaporten gekoppeld aan reservaties |
| `favorite` | Favoriete auto's van klanten |

---
