# Säkerhetsdesign – Fastighetsskötsel API

## 1. Syfte

Fastighetsskötsel API är ett ASP.NET Core Web API för hantering av felanmälningar.

API:t ska användas av två typer av användare:

- Resident – boende
- PropertyManager – fastighetsskötare

Lösningen ska implementeras i Azure med fokus på säkerhet i molnutveckling.

Säkerhetsdesignen utgår bland annat från principerna:

- Least Privilege
- Defense in Depth
- Secure by Design
- Secure by Default

Säkerhet ska beaktas redan från designfasen och vara en integrerad del av lösningen, snarare än något som läggs till efter att funktionaliteten är färdig.

---

## 2. Användarroller

### Resident

En Resident representerar en boende och ska kunna:

- skapa en felanmälan
- hämta en egen felanmälan
- uppdatera beskrivningen i en egen felanmälan

En Resident ska inte kunna:

- hämta andra boendes felanmälningar
- ändra status
- ändra vem som skapade en felanmälan
- ändra när felanmälan skapades
- radera en felanmälan

### PropertyManager

En PropertyManager representerar en fastighetsskötare och ska kunna:

- hämta alla felanmälningar
- hämta en specifik felanmälan
- uppdatera felanmälningar
- ändra status enligt definierade statusövergångar
- radera felanmälningar

---

## 3. Behörighetsmodell

| HTTP-metod | Endpoint | Resident | PropertyManager |
|---|---|---|---|
| GET | `/api/faultreports` | Nej | Ja |
| GET | `/api/faultreports/{id}` | Egna | Ja |
| POST | `/api/faultreports` | Ja | Nej |
| PUT | `/api/faultreports/{id}` | Egna | Ja |
| DELETE | `/api/faultreports/{id}` | Nej | Ja |

Åtkomst till en specifik felanmälan ska inte enbart baseras på att användaren är autentiserad.

För en Resident ska API:t även kontrollera att den aktuella felanmälan tillhör användaren.

Detta innebär att auktorisering sker på flera nivåer:

1. Användaren måste vara autentiserad.
2. Användaren måste ha rätt applikationsroll.
3. För resurser som ägs av en Resident ska API:t kontrollera ägarskapet.
4. För operationer på enskilda fält ska API:t säkerställa att användaren endast kan ändra tillåtna värden.

---

## 4. Datamodell för felanmälan

En felanmälan ska initialt innehålla följande information:

| Fält | Beskrivning |
|---|---|
| `Id` | Unik identifierare för felanmälan |
| `CreatedBy` | Identifierare för användaren som skapade felanmälan |
| `Description` | Beskrivning av felet |
| `Status` | Aktuell status |
| `CreatedAt` | Datum och tid då felanmälan skapades |

### CreatedBy

`CreatedBy` ska vara av typen `Guid` och baseras på Microsoft Entra ID:s `oid`-claim.

Klienten ska inte själv kunna ange vilken användare som är skapare av en felanmälan.

API:t hämtar istället användarens identitet från den autentiserade access-token och använder denna när felanmälan skapas.

Det förhindrar att en användare försöker skapa en felanmälan som tillhör en annan användare.

Ägarskapet används även vid senare åtkomst till felanmälan.

### CreatedAt

`CreatedAt` ska sättas av API:t när felanmälan skapas.

Klienten ska inte kunna ange eller ändra skapelsedatumet.

### Id

`Id` ska genereras av databasen och identifiera den enskilda felanmälan.

Ett giltigt `Id` ska inte i sig innebära att en användare får tillgång till felanmälan.

---

## 5. Status

En felanmälan följer en definierad statuslivscykel:

`Ny → Pågående → Åtgärdad`

Följande statusar används:

- `Ny`
- `Pågående`
- `Åtgärdad`

När en Resident skapar en felanmälan ska status automatiskt sättas till `Ny` av API:t.

En Resident ska inte kunna ändra status.

En PropertyManager ska kunna ändra status, men endast enligt de definierade statusövergångarna:

| Från | Till | Tillåtet |
|---|---|---|
| Ny | Pågående | Ja |
| Pågående | Åtgärdad | Ja |
| Ny | Åtgärdad | Nej |
| Pågående | Ny | Nej |
| Åtgärdad | Pågående | Nej |
| Åtgärdad | Ny | Nej |

Statusen ska därför inte behandlas som ett fritt textfält som en användare med rätt roll kan ändra till valfritt värde.

Den kontrollerade statuslivscykeln är ett exempel på Least Privilege och Secure by Design.

---

## 6. Säkerhetsprinciper

### 6.1 Least Privilege

Varje användarroll ska endast få de behörigheter som krävs för att utföra sina arbetsuppgifter.

En Resident får exempelvis inte tillgång till alla felanmälningar bara för att användaren är autentiserad.

En Resident ska även endast kunna ändra de fält som är relevanta för dennes uppgift.

Systemstyrda värden som `Id`, `CreatedBy` och `CreatedAt` ska inte kunna ändras av klienten.

Även en PropertyManager ska endast kunna utföra de operationer som är definierade för rollen.

Azure-resurser ska också tilldelas minsta möjliga behörighet som krävs för respektive uppgift.

### 6.2 Defense in Depth

Lösningen ska använda flera säkerhetslager så att säkerheten inte är beroende av en enda kontroll.

Planerade säkerhetslager är bland annat:

- Microsoft Entra ID för identitet
- Access tokens för autentisering mot API:t
- Rollbaserad auktorisering
- Kontroll av ägarskap till enskilda felanmälningar
- Validering av tillåtna statusövergångar
- Azure RBAC
- Managed Identity
- säker åtkomst till Azure SQL Database
- krypterad kommunikation

Om ett säkerhetslager skulle kringgås ska ytterligare lager fortfarande kunna förhindra obehörig åtkomst.

### 6.3 Secure by Design

Säkerhet ska byggas in i lösningens arkitektur och API-design från början.

Exempel:

- `CreatedBy` hämtas från den autentiserade identiteten istället för från klientens request.
- `CreatedAt` sätts av servern.
- Status sätts automatiskt till `Ny`.
- Statusövergångar är definierade och begränsade.
- Resident får endast åtkomst till egna felanmälningar.
- Användare får endast de operationer som krävs för deras roll.
- Systemstyrda fält kan inte manipuleras av klienten.

Säkerhet ska därmed vara en del av designen av funktionaliteten och inte enbart läggas till som ett separat säkerhetslager efteråt.

### 6.4 Secure by Default

Lösningen ska ha säkra standardvärden och standardbeteenden.

Exempel:

- En ny felanmälan får automatiskt status `Ny`.
- `CreatedBy` sätts automatiskt utifrån den autentiserade användaren.
- `CreatedAt` sätts automatiskt av API:t.
- Användare får inte åtkomst till resurser om behörighet saknas.
- Resident får inte automatiskt åtkomst till andra användares felanmälningar.
- Klienten får inte automatiskt möjlighet att ändra systemstyrda fält.

Utgångsläget ska alltså vara att åtkomst och förändringar är begränsade, och att ytterligare behörighet måste ges explicit.

---

## 7. Identitetshantering och behörigheter

Microsoft Entra ID ska användas för autentisering av användare.

API:t ska validera access tokens som utfärdats av Microsoft Entra ID.

Två applikationsroller ska användas:

- `Resident`
- `PropertyManager`

Rollerna ska användas för att styra åtkomsten till API:ts endpoints.

Entra ID:s `oid`-claim ska användas för att identifiera den autentiserade användaren och koppla användaren till de felanmälningar som denne har skapat.

Rollbaserad auktorisering ska kombineras med kontroll av resursägarskap.

Azure RBAC ska användas separat för att styra åtkomst till Azure-resurser. Applikationsrollerna `Resident` och `PropertyManager` ska inte blandas ihop med Azure RBAC-roller.

---

## 8. Azure

Den preliminära Azure-arkitekturen består av:

- Microsoft Entra ID
- Azure App Service
- Azure SQL Database
- Managed Identity

App Service ska använda Managed Identity för åtkomst till Azure-resurser där detta är möjligt, istället för att lagra credentials direkt i applikationen.

Azure-resurser ska tilldelas minsta möjliga behörighet enligt Least Privilege.

Nätverksåtkomst till databasen ska begränsas så långt det är möjligt inom projektets omfattning.

Kommunikation mellan klient, API och Azure-tjänster ska använda krypterade anslutningar.

Den slutliga konfigurationen dokumenteras när Azure-resurserna implementeras.

---

## 9. Kostnadshänsyn

Projektet kommer i första hand att använda användarens privata Azure-prenumeration.

Innan Azure-resurser skapas ska kostnad och tillgängliga kostnadsfria kvoter kontrolleras.

Den aktuella prenumerationen har kostnadsfria tjänster i 12 månader till och med 2027-02-07.

Vid kontroll av prenumerationen var den faktiska kostnaden 0 kr.

Azure SQL Database S0 finns bland de kostnadsfria tjänsterna och är därför en kandidat för projektets databas.

Ingen Azure-resurs ska skapas enbart för att den finns tillgänglig. Resurser ska endast användas när de behövs för lösningen.

---

## 10. Säkerhetsområden som ska analyseras

Följande säkerhetsprinciper och säkerhetsområden ska analyseras i den slutliga rapporten.

Målet är att implementera relevanta säkerhetsåtgärder inom varje område där det är rimligt inom projektets omfattning. Om ett område inte implementeras fullt ut ska rapporten beskriva hur lösningen skulle kunna göras säkrare inom området.

### Säkerhetsprinciper

- Least Privilege
- Defense in Depth
- Secure by Design
- Secure by Default

### Säkerhetsområden

- Identitetshantering
- Behörigheter
- Nätverk
- Kryptering
- Hemlighetshantering
- Loggning
- Övervakning

För varje område ska rapporten beskriva:

1. Vad som har implementerats.
2. Varför säkerhetsåtgärden valdes.
3. Hur åtgärden bidrar till säkerheten.
4. Eventuella begränsningar.
5. Möjliga förbättringar eller ytterligare säkerhetsåtgärder.