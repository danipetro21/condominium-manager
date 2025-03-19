# Gestione Spese Condominiali

### Piattaforma per amministratori di condominio per gestire spese e pagamenti con reportistica in pdf e notifiche tramite invio mail.

### Requisiti:

- Due ruoli con funzionalità diverse, Admin e Gestore di condominio
- CRUD per registrazione spese (descrizione, importo, data, categoria)
- Associazione Spese al condominio e calcolo delle quote
- Generazione PDF riepilogativi (opz)
- Dashboard per admin di gestione delle spese
- Pagina per gestore di condominio con lo storico delle proprie spese
- Notifiche Mail per rate in scadenza
 

### Funzionalità accessorie:

- Ogni Gestore carica le proprie spese:
   o Può allegare ricevute o fatture

- Un Amministratore approva o rifiuta la spesa caricata
 o Viene generata una notifica in entrambi i casi
 

### Codebase

- Framework Blazor con utilizzo di MudBlazor
- Database: Sql server express
- .Net 8
- Git Flow


export PATH="$PATH:$HOME/.dotnet/tools/"