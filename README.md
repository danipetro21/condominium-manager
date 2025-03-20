# 🏢 Condominium Manager

Un'applicazione web moderna per la gestione completa dei condomini, sviluppata con ASP.NET Core 8.0 e Blazor.

## Caratteristiche Principali

### 🔒 Autenticazione e Sicurezza
- Autenticazione ASP.NET Core Identity
- Autorizzazione basata su ruoli (RBAC)
- Protezione CSRF
- Password hashing con BCrypt
- Refresh token per sessioni sicure

### ⚙️ Backend
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server 2022
- Blazor Server
- MudBlazor per l'interfaccia utente

## Email Service
- mailersend.com

### 📊 Funzionalità Core
- Gestione completa delle spese condominiali
- Sistema di notifiche in tempo reale
- Generazione automatica di report PDF
- Gestione file con upload multiplo
- Sistema di comunicazioni condominiali
- Calcolo automatico delle quote condominiali
- Export dati in vari formati (CSV, Excel, PDF)

## 🚀 Requisiti di Sistema

- Docker Desktop
- Visual Studio Code con estensioni:
  - Docker
  - Dev Containers

## 🛠️ Configurazione Iniziale

1. Clona il repository:
   ```bash
   git clone https://github.com/yourusername/condominium-manager.git
   cd condominium-manager
   ```

2. Apri il progetto in VS Code:
   ```bash
   code .
   ```

3. Quando richiesto, clicca su "Reopen in Container"

4. Attendi che il container venga creato e avviato

### 📦 Dati di Seed
Durante il build del container, vengono automaticamente caricati alcuni dati

### 🔄 Gestione Migrazioni
Per gestire le migrazioni del database, utilizza i seguenti comandi:

```bash

# Applica le migrazioni al database
dotnet ef database update

# Reimposta il database (elimina e ricrea)
dotnet ef database drop --force
dotnet ef database update
```

## 🔑 Credenziali di Accesso

### Amministratore
- **Email**: admin@cem.com
- **Password**: Admin123!

### Gestori di Condominio
- **Email**: manager1@cem.com
- **Password**: Manager123!

- **Email**: manager2@cem.com
- **Password**: Manager123!

- **Email**: manager3@cem.com
- **Password**: Manager123!

### Condomini di Esempio
1. **Residenza del Sole**
   - Indirizzo: Via Roma 123
   - Città: Milano
   - Provincia: MI
   - CAP: 20100
   - Gestore: John Doe

2. **Villa Verde**
   - Indirizzo: Via Garibaldi 456
   - Città: Roma
   - Provincia: RM
   - CAP: 00100
   - Gestore: Jane Smith

3. **Palazzo Moderno**
   - Indirizzo: Via Dante 789
   - Città: Torino
   - Provincia: TO
   - CAP: 10100
   - Gestore: Mike Johnson

## 🌐 Accesso all'Applicazione

- **Frontend**: https://localhost:5001

## 🗄️ Database

- **Server**: localhost,1433
- **Database**: cem
- **Username**: sa
- **Password**: YourStrong@Passw0rd

### 📊 Struttura del Database

#### Tabelle Principali

##### ApplicationUsers (Eredita da IdentityUser)
- Id (PK)
- UserName
- Email
- PhoneNumber
- FirstName
- LastName
- CreatedAt
- UpdatedAt
- IsActive

##### Condominiums
- Id (PK)
- Name (varchar(200))
- Address (varchar(500))
- City (varchar(100))
- Province (varchar(2))
- PostalCode (varchar(5))
- CreationDate
- CreatedAt
- UpdatedAt

##### Expenses
- Id (PK)
- Description (varchar(500))
- Amount (decimal)
- Date
- Category (enum: Manutenzione, Pulizia, Energia, Assicurazione, Altro)
- Status (enum: Pending, Approved, Rejected)
- CreatedAt
- ApprovedAt
- CreatedById (FK)
- ApprovedById (FK)
- CondominiumId (FK)

##### Notifications
- Id (PK)
- Title
- Message
- Type (enum: ExpenseApproved, ExpenseRejected, PaymentDue, SystemNotification)
- CreatedAt
- ReadAt
- UserId (FK)
- ExpenseId (FK)

##### AppFiles
- Id (PK)
- FileName
- ContentType
- FilePath
- UploadDate
- FileSize
- UploadedById (FK)
- EntityType
- EntityId

#### Relazioni Principali
- ApplicationUsers -> Condominiums (Many-to-Many)
- ApplicationUsers -> Expenses (One-to-Many) come CreatedBy
- ApplicationUsers -> Expenses (One-to-Many) come ApprovedBy
- ApplicationUsers -> Notifications (One-to-Many)
- ApplicationUsers -> AppFiles (One-to-Many)
- Condominiums -> Expenses (One-to-Many)
- Expenses -> Notifications (One-to-Many)
- Expenses -> AppFiles (One-to-Many)

#### Indici
- ApplicationUsers: Email (Unique)
- Expenses: Date, Category, Status
- Notifications: UserId, CreatedAt
- AppFiles: EntityType, EntityId

#### Enumerazioni
##### ExpenseCategory
- Manutenzione
- Pulizia
- Energia
- Assicurazione
- Altro

##### ExpenseStatus
- Pending
- Approved
- Rejected

##### NotificationType
- ExpenseApproved
- ExpenseRejected
- PaymentDue
- SystemNotification

## 🛡️ Certificati HTTPS

Il certificato HTTPS viene generato automaticamente durante il build del container. Se ricevi un avviso di sicurezza nel browser, è normale in ambiente di sviluppo.

## 🔧 Comandi Utili

```bash
# Avvia l'applicazione
docker-compose up

# Ferma l'applicazione
docker-compose down

# Ricostruisci i container
docker-compose up --build

# Visualizza i log
docker-compose logs -f
```